/*
Copyright 2009-2011 Joe Lukacovic

This file is part of QSBrowser.

QSBrowser is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

QSBrowser is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with QSBrowser.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QSB.GameServerInterface.Packets.NetQuake;
using System.Drawing;

namespace QSB.GameServerInterface.Games.NetQuake
{
    public class NetQuake : IServerInfoProvider
    {
        private int _port;
        private string _serverAddress;

        public ServerSnapshot GetServerInfo(string pServerAddress, int pServerPort)
        {
            _port = pServerPort;
            _serverAddress = pServerAddress;

            UdpUtility udp = new UdpUtility(pServerAddress, pServerPort);
            ServerSnapshot serverInfo = new ServerSnapshot();
            serverInfo.Players = new PlayerSnapshots();

            byte[] bytesReceived = udp.SendBytes(new ServerInfoRequest().GetPacket());
            if (bytesReceived == null)
                throw new Exception("Communications exception");

            ReplyPacket packet = GetReplyPacket(bytesReceived);

            if (!(packet is ServerInfoReply))
                throw new Exception("Communications exception");

            SetServerInfo(packet as ServerInfoReply, serverInfo);
            
            string ruleName = string.Empty;

            do
            {
                bytesReceived = udp.SendBytes(new RuleInfoRequest(ruleName).GetPacket());
                packet = GetReplyPacket(bytesReceived);

                if (!(packet is RuleInfoReply))
                    throw new Exception("Communications exception");

                RuleInfoReply reply = packet as RuleInfoReply;
                if (string.IsNullOrEmpty(reply.RuleName))
                    break;

                AddServerRule(reply, serverInfo);
                ruleName = reply.RuleName;

            } while (!string.IsNullOrEmpty(ruleName.Trim()));

            int playerCount = serverInfo.CurrentPlayerCount;

            for (int i = 0; i < playerCount; i++)
            {
                bytesReceived = udp.SendBytes(new PlayerInfoRequest(i).GetPacket());
                packet = GetReplyPacket(bytesReceived);

                if (!(packet is PlayerInfoReply))
                    throw new Exception("Communications exception");

                PlayerInfoReply reply = packet as PlayerInfoReply;
                if (reply.PlayerName == null || reply.PlayerName.Length == 0)
                    break;

                AddPlayerInfo(reply, serverInfo);
            }

            serverInfo.Port = udp.RemotePort;
            serverInfo.IpAddress = udp.RemoteIpAddress;

            return serverInfo;
        }

        private ReplyPacket GetReplyPacket(byte[] pBytes)
        {
            byte controlByte = ReplyPacket.GetControlByte(pBytes);
            ReplyPacket reply = null;

            switch (controlByte)
            {
                case QuakeNetworkPacket.CCREP_SERVER_INFO:
                    reply = new ServerInfoReply();
                    reply.SetPacket(pBytes);
                    return reply;

                case QuakeNetworkPacket.CCREP_RULE_INFO:
                    reply = new RuleInfoReply();
                    reply.SetPacket(pBytes);
                    return reply;

                case QuakeNetworkPacket.CCREP_PLAYER_INFO:
                    reply = new PlayerInfoReply();
                    reply.SetPacket(pBytes);
                    return reply;
            }
            return reply;
        }

        private void SetServerInfo(ServerInfoReply pReplyPacket, ServerSnapshot pServerInfo)
        {
            pServerInfo.Port = _port;
            pServerInfo.IpAddress = pReplyPacket.Address;
            pServerInfo.ServerName = pReplyPacket.HostName;
            pServerInfo.CurrentMap = pReplyPacket.MapName;
            pServerInfo.CurrentPlayerCount = (int)pReplyPacket.CurrentPlayers;
            pServerInfo.MaxPlayerCount = (int)pReplyPacket.MaxPlayers;
            pServerInfo.ServerVersion = pReplyPacket.GameProtocol.ToString();
        }

        private void AddServerRule(RuleInfoReply pReplyPacket, ServerSnapshot pServerInfo)
        {
            pServerInfo.ServerSettings.Add(new ServerSetting(pReplyPacket.RuleName, pReplyPacket.RuleValue));
        }

        private void AddPlayerInfo(PlayerInfoReply pReplyPacket, ServerSnapshot pServerInfo)
        {
            NetQuakePlayer playerInfo = new NetQuakePlayer(
                pReplyPacket.PlayerName,
                pReplyPacket.Address,
                PlayerBytesToString(pReplyPacket.PlayerName),
                pReplyPacket.PlayerNumber);

            // Frags are -99 (observer)
            if (pReplyPacket.FragCount > 65000)
                playerInfo.Frags = -99;
            else
                playerInfo.Frags = pReplyPacket.FragCount;

            playerInfo.PantColor = (int)pReplyPacket.PantColor;
            playerInfo.ShirtColor = (int)pReplyPacket.ShirtColor;
            playerInfo.PlayTime = TimeSpan.FromSeconds(pReplyPacket.PlayTime);

            pServerInfo.Players.Add(playerInfo);
        }

        public string PlayerBytesToString(byte[] pPlayerName)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in pPlayerName)
            {
                sb.Append(ConvertByteToChar(b));
            }
            return sb.ToString();
        }

        public char ConvertByteToChar(byte pByte)
        {
            if (pByte >= 0x20 && pByte < 0x80)
                return (char)pByte;
            if (pByte >= 0xA0 && pByte <= 0xFF)
                return (char)(pByte - 128);

            switch (pByte)
            {
                case 0x0d:
                case 0x8d:
                    return '>';
                case 0x00:
                case 0x05:
                case 0x0e:
                case 0x0f:
                case 0x0c:
                case 0x85:
                case 0x8e:
                case 0x8f:
                case 0x9c:
                    return (char)183; // Middle dot, product of sign
                case 0x10:
                case 0x90:
                    return '[';
                case 0x11:
                case 0x91:
                    return ']';
                case 0x12:
                case 0x92:
                    return '0';
                case 0x13:
                case 0x93:
                    return '1';
                case 0x14:
                case 0x94:
                    return '2';
                case 0x15:
                case 0x95:
                    return '3';
                case 0x96:
                case 0x16:
                    return '4';
                case 0x17:
                case 0x97:
                    return '5';
                case 0x18:
                case 0x98:
                    return '6';
                case 0x19:
                case 0x99:
                    return '7';
                case 0x1A:
                case 0x9A:
                    return '8';
                case 0x1B:
                case 0x9B:
                    return '9';
            }
            return ' ';
        }

        private string ColorTranslator(int pQuakeColorNum)
        {
            switch (pQuakeColorNum)
            {
                case 0:
                    return "White";
                case 1:
                    return "Brown";
                case 2:
                    return "Blue";
                case 3:
                    return "Green";
                case 4:
                    return "Red";
                case 5:
                    return "Tan";
                case 6:
                    return "Pink";
                case 7:
                    return "LightBrown";
                case 8:
                    return "Purple";
                case 9:
                    return "Violet";
                case 10:
                    return "LightViolet";
                case 11:
                    return "Teal";
                case 12:
                    return "Yellow";
                case 13:
                    return "Blue";
                default:
                    return "Unknown";
            }
        }
    }
}
