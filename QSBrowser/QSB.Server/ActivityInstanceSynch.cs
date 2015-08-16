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
                
                // If there are now players here, start a new match
                if (pSnapshot.Players.Count > 1
                    && pActivity.MatchStart == null)
                    pActivity.NewMatch();
                
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
            if (pActivity.MatchStart.HasValue &&
                (playerResetCount > 1
                || pActivity.ServerSnapshot.Mod != pSnapshot.Mod
                || pActivity.ServerSnapshot.CurrentMap != pSnapshot.CurrentMap
                || pSnapshot.Players.Count < 2))
            {
                // Create a ServerMatch record
                ServerMatch serverMatch = new ServerMatch();
                serverMatch.Modification = pActivity.ServerSnapshot.Mod;
                serverMatch.Map = pActivity.ServerSnapshot.CurrentMap;
                serverMatch.MatchStart = pActivity.MatchStart.Value;
                serverMatch.ServerId = _gameServer.ServerId;
                serverMatch.MatchEnd = DateTime.UtcNow;
                _dataSession.AddServerMatch(serverMatch);


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
                    pActivity.NewMatch();
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

            ///////OLD WAY::::::::::

            //// Determine which players have left, and update players that havne't
            //for(int i = pActivity.PlayerActivities.Count-1; i >= 0; i--)
            //{
            //    PlayerActivity oldPlayer = pActivity.PlayerActivities[i];
            //    int index = pSnapshot.Players.IndexOf(oldPlayer.PlayerSnap);
            //    if (index >= 0)
            //    {
            //        PlayerSnapshot playerInfo = pSnapshot.Players[index];
            //        oldPlayer.UpdatePlayer(playerInfo
            //            ,_dataSession.GetOrCreatePlayer(playerInfo.PlayerName,playerInfo.IpAddress,playerInfo.PlayerNameBytes, _gameServer.GameId)
            //            ,false
            //        );

            //        if(oldPlayer.IsScoreReset)
            //            playerResetCount++;
            //    }
            //    else
            //    {
            //        _dataSession.AddUpdatePlayerSession(oldPlayer.EndSession());

            //        pActivity.PlayerMatchGhosts.Add(oldPlayer);
            //        pActivity.PlayerGhostSnapshots.Add(oldPlayer.PlayerSnap);
            //        pActivity.PlayerActivities.Remove(oldPlayer);
            //    }
            //}

            // Iterate through snapshot finding players not accounted for
            //foreach (PlayerSnapshot pInfo in pSnapshot.Players)
            //{
            //    // See if old snapshot contains player
            //    if (!pActivity.ServerSnapshot.Players.Contains(pInfo))
            //    {
            //        bool foundGhost = false;
            //        // Check ghosts for player
            //        foreach (PlayerActivity ghost in pActivity.PlayerMatchGhosts)
            //        {
            //            if (ghost.PlayerSnap.Equals(pInfo))
            //            {
            //                foundGhost = true;
            //                pActivity.PlayerMatchGhosts.Remove(ghost);
            //                pActivity.PlayerActivities.Add(ghost);
            //                ghost.UpdatePlayer(pInfo
            //                    ,_dataSession.GetOrCreatePlayer(pInfo.PlayerName, pInfo.IpAddress, pInfo.PlayerNameBytes, _gameServer.GameId)
            //                    ,true
            //                    );
            //                break;

            //            }
            //        }
            //        if (!foundGhost)
            //        {
            //            pActivity.PlayerActivities.Add(new PlayerActivity(pInfo,
            //                _dataSession.GetOrCreatePlayer(pInfo.PlayerName, pInfo.IpAddress, pInfo.PlayerNameBytes, _gameServer.GameId)
            //                ));
            //        }
            //    }
            //}

            //if (pActivity.PlayerActivities.Count != pSnapshot.Players.Count)
            //    throw new Exception("Player count mismatch");
            
            pActivity.UpdateSnapshot(pSnapshot);

            // DEBUG
            //if (pActivity.PlayerMatchGhosts.Count > 0)
            //    System.Diagnostics.Debug.WriteLine("Ghosts in " + _gameServer.DNS);

            //foreach (PlayerActivity activity in pActivity.PlayerMatchGhosts)
            //{
            //    System.Diagnostics.Debug.WriteLine(activity.PlayerSnap.PlayerName.ToString() + "  < GHOST ");
            //}

            //if (pActivity.PlayerActivities.Count > 0)
            //    System.Diagnostics.Debug.WriteLine("Players here");

            //foreach (PlayerActivity activity in pActivity.PlayerActivities)
            //{
            //    System.Diagnostics.Debug.WriteLine(activity.PlayerSnap.PlayerName.ToString() + "   ");
            //}

            ////// END GIGANTIC OLD UPDATE CODE ///////

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

            if (pSnapshot.Players.Count > 1)
                serverActivity.NewMatch();

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
