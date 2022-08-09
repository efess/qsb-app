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
using System.Text;
using System.Linq;
using QSB.Data;
using QSB.GameServerInterface;
using QSB.Data.TableObject;
using QSB.GameServerInterface.Games.NetQuake;
using QSB.Common;

namespace QSB.Server
{
    public class ActivityInstanceSynch
    {
        private IDataSession _dataSession;
        private GameServer _gameServer;

        public ActivityInstanceSynch(IDataSession pDataSession, GameServer pGameServer)
        {
            _dataSession = pDataSession;
            _gameServer = pGameServer;
        }

        public void SynchExistingActivity(ServerActivity pActivity, ServerSnapshot pSnapshot)
        {
            int playerResetCount = 0;
            List<Player> activeDbPlayers = new List<Player>();

            // If there are now players here, start a new match
            if (pSnapshot.Players.Where(player => player.Frags != -99).Count() > 1
                && pActivity.CurrentMatch == null)
            {
                pActivity.NewMatch();
                _dataSession.AddUpdateServerMatch(pActivity.CurrentMatch);
            }

            foreach (PlayerSnapshot playerSnapShot in pSnapshot.Players)
            {
                // Get player from database. DB Layer determines PlayerId (used for Comparisons)
                Player dbPlayer = _dataSession.GetOrCreatePlayer(
                    playerSnapShot.PlayerName,
                    playerSnapShot.IpAddress,
                    playerSnapShot.PlayerNameBytes,
                    _gameServer.GameId);
                
                playerSnapShot.PlayerId = dbPlayer.PlayerId;

                activeDbPlayers.Add(dbPlayer);
                
                // Find player currently in game. If this occurs more than once, duplicate player in game exists.
                PlayerActivity activePlayer = pActivity.FindActivity(dbPlayer.PlayerId);
                if(activePlayer != null)
                {
                    System.Diagnostics.Debug.WriteLine("Player Is Here: " + activePlayer.ToString());
                    activePlayer.UpdatePlayer(playerSnapShot, dbPlayer, false);

                    if (activePlayer.IsScoreReset)
                        playerResetCount++;
                }
                else
                {
                    // Not found, add new player activity

                    System.Diagnostics.Debug.WriteLine("Player just joined: " + dbPlayer.Alias);
                    // Check ghosts for player
                    PlayerActivity ghostPlayer = pActivity.FindGhost(dbPlayer.PlayerId);
                    if(ghostPlayer != null)
                    {
                        pActivity.PlayerMatchGhosts.Remove(ghostPlayer);
                        pActivity.PlayerActivities.Add(ghostPlayer);

                        if (pSnapshot.Players.Count > 1)
                            ghostPlayer.NewMatch();

                        ghostPlayer.UpdatePlayer(playerSnapShot
                            , dbPlayer
                            , true
                        );               
         
                    }
                    else
                    {
                        // Ghost not found, add new Activity
                        PlayerActivity newPlayer = new PlayerActivity(
                            playerSnapShot,
                            dbPlayer);

                        // If two or more players are in this game, start a match for this player.
                        if (pSnapshot.Players.Count > 1)
                            newPlayer.NewMatch();

                        pActivity.PlayerActivities.Add(newPlayer);
                    }
                }
            }

            // Probably game/match change here
            if (pActivity.CurrentMatch != null &&
                (playerResetCount > 1
                || pActivity.ServerSnapshot.Mod != pSnapshot.Mod
                || pActivity.ServerSnapshot.Mode != pSnapshot.Mode
                || pActivity.ServerSnapshot.CurrentMap != pSnapshot.CurrentMap
                || pSnapshot.Players.Where(p => p.Frags >= -1).Count() < 2))
            {
                // Create a ServerMatch record
                ServerMatch serverMatch = pActivity.CurrentMatch;
                serverMatch.Mode = pActivity.ServerSnapshot.Mode;
                serverMatch.Modification = pActivity.ServerSnapshot.Mod;
                serverMatch.Map = pActivity.ServerSnapshot.CurrentMap;
                serverMatch.ServerId = _gameServer.ServerId;
                serverMatch.MatchEnd = DateTime.UtcNow;
                _dataSession.AddUpdateServerMatch(serverMatch);


                foreach (PlayerActivity playerActivity in pActivity.PlayerActivities)
                {
                    if(playerActivity.CurrentMatch != null)
                        SavePlayerMatch(playerActivity, serverMatch.ServerMatchId);
                }

                foreach (PlayerActivity playerActivity in pActivity.PlayerMatchGhosts)
                {
                    if (playerActivity.CurrentMatch != null)
                        SavePlayerMatch(playerActivity, serverMatch.ServerMatchId);
                }
                
                // Reset match information
                if (pSnapshot.Players.Count > 1)
                {
                    pActivity.NewMatch();
                    _dataSession.AddUpdateServerMatch(pActivity.CurrentMatch);
                }
                else
                    pActivity.NoMatches();
            }

            // Find players that have left
            for (int i = pActivity.PlayerActivities.Count - 1;
                i >= 0; i--)
            {
                PlayerActivity activePlayer = pActivity.PlayerActivities[i];

                Player dbPlayer = activeDbPlayers.Find(player => player.PlayerId == activePlayer.Session.PlayerId);

                if (dbPlayer == null)
                {
                    pActivity.PlayerActivities.RemoveAt(i);

                    System.Diagnostics.Debug.WriteLine("Player left: " + activePlayer.ToString());
                    _dataSession.AddUpdatePlayerSession(activePlayer.EndSession());

                    pActivity.PlayerMatchGhosts.Add(activePlayer);
                }
                else
                {
                    if (activePlayer.Session.SessionDate != DateTime.UtcNow.Date)
                    {
                        _dataSession.AddUpdatePlayerSession(activePlayer.EndDaySession(dbPlayer));
                    }
                }
            }

            pActivity.UpdateSnapshot(pSnapshot);

            // Save Sessions
            foreach (PlayerActivity player in pActivity.PlayerActivities)
            {
                PlayerSession session = player.Session;
                if (session.ServerId == 0)
                    session.ServerId = _gameServer.ServerId;

                System.Diagnostics.Debug.WriteLine("SessionId: " + session.SessionId.ToString() + " ThreadId: " + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString());
                _dataSession.AddUpdatePlayerSession(session);
            }
        }

        public ServerActivity CreateNewActivity(ServerSnapshot pSnapshot)
        {
            // Retrieve sessions that haven't been closed in order to re-initialize state
            IList<PlayerSession> lostSessions = _dataSession.GetInSessionPlayersByServer(_gameServer.ServerId);

            ServerActivity serverActivity = new ServerActivity(pSnapshot, _gameServer);
            List<Player> activeDbPlayers = new List<Player>();

            // Synchronize 
            foreach(PlayerSnapshot snapShot in pSnapshot.Players)
            {                
                Player dbPlayer = _dataSession.GetOrCreatePlayer(
                    snapShot.PlayerName,
                    snapShot.IpAddress,
                    snapShot.PlayerNameBytes,
                    _gameServer.GameId);

                snapShot.PlayerId = dbPlayer.PlayerId;

                bool sessionFound = false;
                for(int i = lostSessions.Count - 1; i >= 0; i--)
                {
                    PlayerSession session = lostSessions[i];

                    if(session.LastAlias.PlayerId == dbPlayer.PlayerId)
                    {
                        sessionFound = true;
                        lostSessions.RemoveAt(i);
                        session.LastAlias = dbPlayer;
                        serverActivity.PlayerActivities.Add(new PlayerActivity(snapShot, session));
                        _dataSession.AddUpdatePlayerSession(session);
                        break;
                    }
                }
                if (!sessionFound)
                {
                    PlayerActivity playerActivity = new PlayerActivity(snapShot, dbPlayer);
                    serverActivity.PlayerActivities.Add(playerActivity);
                    _dataSession.AddUpdatePlayerSession(playerActivity.Session);
                }
            }
            
            // Lost Sessions will be filtered from active sessions.
            // Now it contains in-active sessions that must be closed.
            foreach (PlayerSession session in lostSessions)
            {
                session.SessionEnd = DateTime.UtcNow;
                _dataSession.AddUpdatePlayerSession(session);
            }
            if (pSnapshot.Players.Where(player => player.Frags != -99).Count() > 1)
            {
                serverActivity.NewMatch();
                _dataSession.AddUpdateServerMatch(serverActivity.CurrentMatch);
            }

            ServerCache.Cache.AddSnapshot(_gameServer.ServerId, serverActivity);
            return serverActivity;
        }

        private void SavePlayerMatch(PlayerActivity pActivity, int pServerMatchId)
        {
            PlayerSnapshot snapShot = pActivity.PlayerSnap;
            PlayerMatch playerMatch = pActivity.CurrentMatch;

            playerMatch.AliasId = pActivity.Session.LastAlias.AliasId;
            playerMatch.PlayerId = pActivity.Session.PlayerId;
            playerMatch.ServerMatchId = pServerMatchId;
            
            if (playerMatch.PlayerMatchEnd == null)
                playerMatch.PlayerMatchEnd = DateTime.UtcNow;

            _dataSession.AddPlayerMatch(playerMatch);
        }
    }
}
