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
using System.Xml;
using QSB.Common;

namespace QSB.GameServerInterface
{
    /// <summary>
    /// Information object representing information captured from a Server query
    /// </summary>
    public class ServerSnapshot
    {
        /// <summary>
        /// IpAddress of server
        /// </summary>
        public string IpAddress { get;  set; }
        /// <summary>
        /// TCP/IP listening port of server
        /// </summary>
        public int Port { get;  set; }
        /// <summary>
        /// Server's name
        /// </summary>
        public string ServerName { get;  set; }
        /// <summary>
        /// Current map server is hosting
        /// </summary>
        public string CurrentMap { get;  set; }
        /// <summary>
        /// Current number of players
        /// </summary>
        public int CurrentPlayerCount { get;  set; }
        /// <summary>
        /// Maximum number of players
        /// </summary>
        public int MaxPlayerCount { get;  set; }
        /// <summary>
        /// Server version
        /// </summary>
        public string ServerVersion { get;  set; }
        /// <summary>
        /// Game modification server is hosting
        /// </summary>
        public string Mod { get;  set; }
        /// <summary>
        /// Current server status
        /// </summary>
        public ServerStatus Status { get;  set;}
        /// <summary>
        /// ServerSettings in Key/Value format
        /// </summary>
        public List<ServerSetting> ServerSettings { get;  set; }
        /// <summary>
        /// Collection of PlayerInfos representing player entities
        /// currently on the server
        /// </summary>
        public PlayerSnapshots Players { get;  set; }

        public ServerSnapshot()
        {
            Players = new PlayerSnapshots();
            ServerSettings = new List<ServerSetting>();
        }

        /// <summary>
        /// Server settings in Setting/Value XML string
        /// </summary>
        public string ServerSettingsXml
        {
            get
            {
                XmlDocument xd = new XmlDocument();

                XmlNode rootNode = xd.AppendChild(XmlHelper.GetElementNode(xd, "Settings", string.Empty)); 

                foreach(ServerSetting setting in ServerSettings)
                {
                    XmlNode settingNode = rootNode.AppendChild(XmlHelper.GetElementNode(xd, "Setting",""));

                    settingNode.AppendChild(XmlHelper.GetElementNode(xd,"Setting",setting.Setting));
                    settingNode.AppendChild(XmlHelper.GetElementNode(xd,"Value", setting.Value));
                }

                return xd.OuterXml;
            }
        }
    }
}
