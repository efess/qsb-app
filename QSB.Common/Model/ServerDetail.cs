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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace QSB.Common.Model
{
    public class ServerDetail
    {
        /// <summary>
        /// Internal server Identification
        /// </summary>
        public int ServerId { get; private set; }
        /// <summary>
        /// Server Name received from server query
        /// </summary>
        public string ServerName { get; private set; }
        /// <summary>
        /// Server Name specified inside of the database defined at setup
        /// </summary>
        public string CustomName { get; private set; }
        /// <summary>
        /// Server Name specified inside of the database defined at setup (short version)
        /// </summary>
        public string CustomNameShort { get; private set; }
        /// <summary>
        /// Modification Name inside of the database defined at setup
        /// </summary>
        public string CustomModificationName { get; private set; }
        /// <summary>
        /// Internet reachable HostName of server used by querying process
        /// </summary>
        public string DNS { get; private set; }
        /// <summary>
        /// Port server instance listens on
        /// </summary>
        public int Port { get; private set; }
        /// <summary>
        /// IP Address resolved at query time
        /// </summary>
        public string IpAddress { get; private set;}
        /// <summary>
        /// Game type of server instance
        /// </summary>
        public Game GameId { get; private set; }
        /// <summary>
        /// Public server website
        /// </summary>
        public string PublicSiteUrl { get; private set; }
        /// <summary>
        /// Public location of custom maps (if any)
        /// </summary>
        public string MapDownloadUrl { get; private set; }
        /// <summary>
        /// Specific geographic location of server
        /// </summary>
        public string Location { get; private set; }
        /// <summary>
        /// More generic geographic location of server
        /// </summary>
        public string Region { get; private set; }
        /// <summary>
        /// Modification Code or Type defined at setup
        /// </summary>
        public string ModificationCode { get; private set; }
        /// <summary>
        /// Grouping identifier
        /// </summary>
        public string Category { get; private set; }
        /// <summary>
        /// Current running map on server
        /// </summary>
        public string Map { get; private set; }
        /// <summary>
        /// XML String representing Settings received from server
        /// </summary>
        public string ServerSettings { get; private set; }
        /// <summary>
        /// Modification type received from server
        /// </summary>
        public string Modification { get; private set; }
        /// <summary>
        /// Timestamp of last query
        /// </summary>
        public DateTime Timestamp { get; private set; }
        /// <summary>
        /// Number of maximum players the server supports received from server
        /// </summary>
        public int MaxPlayers { get; private set; }
        /// <summary>
        /// List of current players
        /// </summary>
        public List<PlayerDetail> Players { get; private set; }
        /// <summary>
        /// Status of server from last query
        /// </summary>
        public ServerStatus CurrentStatus { get; private set; }

        public ServerDetail(int pServerId, string pServerName, string pCustomName, string pCustomNameShort,
            string pDNS, int pPort, int pGameId, string pPublicSiteUrl, string pMapDownloadUrl,
            string pLocation, string pRegion, string pModificationCode, string pCategory,
            string pMap, string pServerSettings, string pModification, DateTime pTimStamp,
            int pMaxPlayers, string pPlayerData, int pCurrentStatus, string pIpAddress, string pCustomModificationName)
        {
            ServerId = pServerId;
            ServerName = pServerName;
            CustomName = pCustomName;
            CustomNameShort = pCustomNameShort;
            DNS = pDNS;
            Port = pPort;
            GameId = (Game)pGameId;
            PublicSiteUrl = pPublicSiteUrl;
            MapDownloadUrl = pMapDownloadUrl;
            Location = pLocation;
            Region = pRegion;
            ModificationCode = pModificationCode;
            Category = pCategory;
            Map = pMap;
            ServerSettings = pServerSettings;
            Modification = pModification;
            Timestamp = pTimStamp;
            MaxPlayers = pMaxPlayers;
            Players = ProcessPlayerData(pPlayerData);
            CurrentStatus = (ServerStatus)pCurrentStatus;
            IpAddress = pIpAddress;
            CustomModificationName = pCustomModificationName;
	    }

        private List<PlayerDetail> ProcessPlayerData(string playerJson)
        {
            var playerDetails = new List<PlayerDetail>();

            if (!string.IsNullOrEmpty(playerJson))
            {
                try
                {
                    playerDetails = JsonConvert.DeserializeObject<List<PlayerDetail>>(playerJson);
                } catch { }
            }

            return playerDetails;
        }
    }
}
