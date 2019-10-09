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
using QSB.GameServerInterface.Packets.Quake2;
using System.Collections;
using System.Net.Sockets;

namespace QSB.GameServerInterface.Games.Quake2
{
    class Quake2 : IServerInfoProvider
    {
        private const string Q2_SETTING_HOSTNAME = "hostname";
        private const string Q2_SETTING_MAP = "mapname";
        private const string Q2_SETTING_VERSION = "version";
        private const string Q2_SETTING_MAXPLAYERS = "maxclients";
        private const string Q2_SETTING_MOD = "gamedir";

        private string _address;
        private int _port;

        #region IServerInfoProvider Members

        public ServerSnapshot GetServerInfo(string pServerAddress, int pServerPort)
        {
            _address = pServerAddress;
            _port = pServerPort;

            UdpUtility udp = new UdpUtility(pServerAddress, pServerPort);
            Q2ServerStatus q2Server = new Q2ServerStatus();

            byte[] receivedBytes = udp.SendBytes(Q2ServerStatus.StatusRequest);

            if (receivedBytes == null || receivedBytes.Length == 0)
                throw new SocketException((int)SocketError.NoData);

            q2Server.ParseBytes(receivedBytes);

            ServerSnapshot info = GetServerInfo(q2Server);
            info.Port = udp.RemotePort;
            info.IpAddress = udp.RemoteIpAddress;
            return info;
        }

        #endregion

        private ServerSnapshot GetServerInfo(Q2ServerStatus pStatus)
        {
            ServerSnapshot sInfo = new ServerSnapshot();
            try
            {
                if (pStatus.ServerSettings.Contains(Q2_SETTING_HOSTNAME)) sInfo.ServerName = pStatus.ServerSettings[Q2_SETTING_HOSTNAME].ToString();
                if (pStatus.ServerSettings.Contains(Q2_SETTING_MAXPLAYERS)) sInfo.MaxPlayerCount = int.Parse(pStatus.ServerSettings[Q2_SETTING_MAXPLAYERS].ToString());
                if (pStatus.ServerSettings.Contains(Q2_SETTING_MAP)) sInfo.CurrentMap = pStatus.ServerSettings[Q2_SETTING_MAP].ToString();
                if (pStatus.ServerSettings.Contains(Q2_SETTING_MOD)) sInfo.Mod = pStatus.ServerSettings[Q2_SETTING_MOD].ToString();
                if (pStatus.ServerSettings.Contains(Q2_SETTING_VERSION)) sInfo.ServerVersion = pStatus.ServerSettings[Q2_SETTING_VERSION].ToString();
            }
            catch
            {
                throw new FormatException("Q2 Server data was not in correct format");
            }

            sInfo.CurrentPlayerCount = pStatus.CurrentPlayers.Count;

            try
            {
                foreach (Q2PlayerStatus playerStatus in pStatus.CurrentPlayers)
                {
                    PlayerSnapshot playerInfo = new PlayerSnapshot(
                        playerStatus.PlayerNameBytes, 
                        "",
                        playerStatus.PlayerName,
                        0);
                    
                    playerInfo.Frags = int.Parse(playerStatus.Frags);
                    playerInfo.Ping = int.Parse(playerStatus.Ping);

                    sInfo.Players.Add(playerInfo);
                }
            }
            catch
            {
                throw new FormatException("Q2 Player data was not in correct format");
            }

            foreach (DictionaryEntry entry in pStatus.ServerSettings)
            {
                sInfo.ServerSettings.Add(new ServerSetting(entry.Key.ToString(), entry.Value.ToString()));
            }

            return sInfo;
        }
    }
}
