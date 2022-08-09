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
using QSB.GameServerInterface;
using QSB.Data.TableObject;
using QSB.Common;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using Newtonsoft.Json;
using QSB.GameServerInterface.Games.Common;

namespace QSB.Server
{
    public class SnapshotInstance
    {
        private ServerActivity _activity;
        public SnapshotInstance(ServerActivity pActivity)
        {
            _activity = pActivity;
        }

        public ServerData GetServerDataObject()
        {
            ServerData serverData = new ServerData();
            serverData.ServerId = _activity.DbGameServer.ServerId;
            serverData.Map = _activity.ServerSnapshot.CurrentMap;
            serverData.Modification = _activity.ServerSnapshot.Mod;
            serverData.Mode = _activity.ServerSnapshot.Mode;
            serverData.TimeStamp = _activity.SnapshotTime;
            serverData.Name = _activity.ServerSnapshot.ServerName;
            serverData.ServerSettings = _activity.ServerSnapshot.ServerSettingsJson.Trim();
            serverData.IpAddress = _activity.ServerSnapshot.IpAddress.Trim();
            serverData.MaxPlayers = _activity.ServerSnapshot.MaxPlayerCount;
            serverData.PlayerData = GetPlayerJsonData((Game)_activity.DbGameServer.GameId);

            return serverData;
        }

        private string GetPlayerJsonData(Game pGame)
        {
            var players = _activity.ServerSnapshot.Players.Select(player =>
            {
                PlayerActivity activity = _activity.FindActivity(player.PlayerId);
                TimeSpan uptime = DateTime.UtcNow - activity.StartTime;
                var playerData = new PlayerData
                {
                    PlayerId = player.PlayerId,
                    Name = player.PlayerName,
                    NameBase64 = Convert.ToBase64String(player.PlayerNameBytes),
                    UpTime = (int)uptime.TotalSeconds,
                    JoinTime = (int)activity.StartTime.UnixTime(),
                    CurrentFrags = player.Frags,
                    TotalFrags = activity.TotalFrags,
                    FragsPerMinute = uptime.TotalMinutes > 0
                        ? Math.Round(activity.TotalFrags / ((Decimal)uptime.TotalMinutes), 2) 
                        : 0
                };

                if (player is IClothed)
                {
                    playerData.Shirt = (player as IClothed).ShirtColor;
                    playerData.Pant = (player as IClothed).PantColor;
                }

                if (!string.IsNullOrEmpty(player.ModelName))
                {
                    playerData.Model = player.ModelName;
                    playerData.Skin = player.SkinName;
                }

                return playerData;
            });
            return JsonConvert.SerializeObject(players);
        }
        private string GetPlayerXmlData(Game pGame)
        {
            var xEPlayers = new XElement("Players");
            
            XDocument document = new XDocument(
                new XDeclaration("1.0", "UTF-8", "yes"), xEPlayers);

            foreach (PlayerSnapshot playerSnap in _activity.ServerSnapshot.Players)
            {
                PlayerActivity activity = _activity.FindActivity(playerSnap.PlayerId);
                TimeSpan uptime = DateTime.UtcNow - activity.StartTime;

                var xEPlayer = 
                    new XElement("Player",
                        new XElement("PlayerId", playerSnap.PlayerId),
                        new XElement("Name",playerSnap.PlayerName),
                        new XElement("NameBase64", Convert.ToBase64String(playerSnap.PlayerNameBytes)),
                        new XElement("UpTime", uptime.TotalSeconds),
                        new XElement("JoinTime", activity.StartTime.UnixTime()),
                        new XElement("CurrentFrags", playerSnap.Frags),
                        new XElement("TotalFrags", activity.TotalFrags),
                        new XElement("FragsPerMinute", uptime.TotalMinutes > 0 ? Math.Round(activity.TotalFrags / ((Decimal)uptime.TotalMinutes), 2).ToString("##0.00") : "0.00")
                        );

                switch (pGame)
                {
                    case Game.NetQuake:
                        xEPlayer.Add(new XElement("Shirt", (playerSnap as 
                            QSB.GameServerInterface.Games.NetQuake.NetQuakePlayer).ShirtColor));
                        xEPlayer.Add(new XElement("Pant", (playerSnap as 
                            QSB.GameServerInterface.Games.NetQuake.NetQuakePlayer).PantColor));
                        break;
                    default:
                        xEPlayer.Add(new XElement("Model", playerSnap.ModelName));
                        xEPlayer.Add(new XElement("Skin", playerSnap.SkinName));
                        break;
                }

                xEPlayers.Add(xEPlayer);
            }

            using (StringWriter writer = new Utf8StringWriter())
            {
                document.Save(writer, SaveOptions.DisableFormatting);
                return writer.ToString();
            }
        }

        private class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding
            {
                get
                {
                    return Encoding.UTF8;
                }
            }
        }
    }
}