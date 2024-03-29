﻿/*
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
using System.Text;
using QSB.GameServerInterface.Packets.NetQuake;
using Newtonsoft.Json;
using QSB.GameServerInterface.Games.Common;

namespace QSB.GameServerInterface.Games.QuakeEnhanced
{
    public class QuakeEnhanced : IServerInfoProvider
    {
        private int _port;
        private string _host;
        private byte[] _pskId = Encoding.UTF8.GetBytes("id-quake-ex-dtls");
        private Parameters _serverParams;

        private byte getHexValue(char hex)
        {
            int val = (int)hex;
            return (byte)(val - (val < 58 ? 48 : (val < 97 ? 55 : 87)));
        }

        private INetCommunicate GetNetUtility(string pServerAddress, int pServerPort, Parameters  serverParams)
        {
            if (!string.IsNullOrEmpty(serverParams.Engine) && serverParams.Engine.ToLower() == "fte")
            {
                return new UdpUtility(pServerAddress, pServerPort);
            } 
            else
            {
                var psk = Environment.GetEnvironmentVariable("QE_PSK");
                if (string.IsNullOrEmpty(psk))
                {
                    throw new ArgumentException("Quake Enhanced needs a PSK for traffic encryption");
                }

                return new DtlsUtility(StringToBytes(psk), _pskId, pServerAddress, pServerPort);
            }
        }

        // I'm sure there's a one liner for this...
        private byte[] StringToBytes ( string byteString)
        {
            var byteLength = byteString.Length / 2;
            byte[] bytes = new byte[byteLength];

            var charArray = byteString.ToCharArray();
            for(int i = 0,
                j = 0; i < byteLength; i++, j += 2)
            {
                bytes[i] = (byte)(getHexValue(charArray[j]) << 4 | getHexValue(charArray[j + 1]));
            }
            return bytes;
        }

        public QuakeEnhanced(Parameters parameters)
        {
            _serverParams = parameters;
        }

        public ServerSnapshot GetServerInfo(string pServerAddress, int pServerPort)
        {
            _port = pServerPort;
            _host = pServerAddress;
            var net = GetNetUtility(pServerAddress, pServerPort, _serverParams);
            ServerSnapshot serverInfo = new ServerSnapshot();
            serverInfo.Players = new PlayerSnapshots();

            byte[] bytesReceived = net.SendBytes(new ServerInfoRequest().GetPacket());
            if (bytesReceived == null)
                throw new Exception("Communications exception");

            ReplyPacket packet = GetReplyPacket(bytesReceived);

            if (!(packet is ServerInfoReply))
                throw new Exception("Communications exception");

            SetServerInfo(packet as ServerInfoReply, serverInfo);
            
            string ruleName = string.Empty;

            do
            {
                bytesReceived = net.SendBytes(new RuleInfoRequest(ruleName).GetPacket());
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

            for (int i = 0, j = 0; i < playerCount; i++, j++)
            {
                try
                { 
                    bytesReceived = net.SendBytes(new PlayerInfoRequest(j).GetPacket()); 
                }
                catch
                {
                    Console.WriteLine("My player hack isn't working ..");
                    continue;
                }

                packet = GetReplyPacket(bytesReceived);

                if (!(packet is PlayerInfoReply))
                    throw new Exception("Communications exception");

                PlayerInfoReply reply = packet as PlayerInfoReply;
                if (reply.Address == "") i--; // this is a hack to account for the playerCount issue when bots are on the server.
                if (reply.PlayerName == null || reply.PlayerName.Length == 0)
                    break;

                AddPlayerInfo(reply, serverInfo);
            }

            serverInfo.Port = pServerPort;
            serverInfo.IpAddress = pServerAddress; // udp.RemoteIpAddress;

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
            QuakeEnhancedPlayer playerInfo = new QuakeEnhancedPlayer(
                pReplyPacket.PlayerName,
                pReplyPacket.Address == "LOCAL" ? 
                    "HOST" : 
                    pReplyPacket.Address == "" ? "BOT" : "private", // LOCAL for host, maybe port for players?
                PlayerBytesToString(pReplyPacket.PlayerName),
                pReplyPacket.PlayerNumber);


            playerInfo.Frags = (short)pReplyPacket.FragCount;

            playerInfo.PantColor = (int)pReplyPacket.PantColor;
            playerInfo.ShirtColor = (int)pReplyPacket.ShirtColor;
            playerInfo.PlayerType = pReplyPacket.Address ==  "LOCAL" ?
                    PlayerType.Host  :
                    pReplyPacket.Address == "" ? PlayerType.Bot : PlayerType.Normal;
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
