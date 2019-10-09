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
using System.IO;
using QSB.Data;

namespace QSB.Server
{
    public class ServerActivity
    {
        /// <summary>
        /// Data saved in the database
        /// </summary>
        public GameServer DbGameServer { get; private set; }

        /// <summary>
        /// Info queried from server
        /// </summary>
        public QSB.GameServerInterface.ServerSnapshot ServerSnapshot { get; private set; }
        public DateTime SnapshotTime { get; private set; }
        public ServerStatus Status { get; private set; }

        public List<PlayerActivity> PlayerActivities { get; private set; }
        public List<PlayerActivity> PlayerMatchGhosts { get; private set; }

        public DateTime? MatchStart { get; internal set;}

        /// <summary>
        /// Creates a new instance of ServerActivity provided current server snapshot and DatabaseSessions
        /// </summary>
        /// <param name="pServerInfo"></param>
        /// <param name="pGameServer"></param>
        /// <param name="pDbPlayerSessions"></param>
        internal ServerActivity(QSB.GameServerInterface.ServerSnapshot pServerSnaphot, GameServer pGameServer)
        {
            PlayerActivities = new List<PlayerActivity>();
            PlayerMatchGhosts = new List<PlayerActivity>();
            ServerSnapshot = pServerSnaphot;
            DbGameServer = pGameServer;
        }

        internal void UpdateSnapshot(QSB.GameServerInterface.ServerSnapshot pServerSnaphot)
        {
            ServerSnapshot = pServerSnaphot;
        }

        internal void NewMatch()
        {
            MatchStart = DateTime.UtcNow;

            // Reset Ghosts collection
            PlayerMatchGhosts.Clear();

            foreach (PlayerActivity playerActivity in PlayerActivities)
            {
                playerActivity.NewMatch();
            }
        }

        internal void NoMatches()
        {
            MatchStart = null;
            // Reset Ghosts collection
            PlayerMatchGhosts.Clear();

            foreach (PlayerActivity playerActivity in PlayerActivities)
            {
                playerActivity.NoMatch();
            }
        }

        internal void UpdateStatus(ServerStatus pStatus)
        {
            Status = pStatus;
            SnapshotTime = DateTime.UtcNow;
        }

        public PlayerActivity FindActivity(int pPlayerId)
        {
            return PlayerActivities.Find(act => act.Session.PlayerId == pPlayerId);
        }

        public PlayerActivity FindGhost(int pPlayerId)
        {
            return PlayerMatchGhosts.Find(act => act.Session.PlayerId == pPlayerId);
        }
    }
}
