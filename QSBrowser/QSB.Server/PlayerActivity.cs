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
using QSB.GameServerInterface.Games.NetQuake;

namespace QSB.Server
{
    /// <summary>
    /// Object represnting a Player's session on a server - contains server provided PlayerInfo object
    /// and other calculated values from PlayerInfo updates
    /// </summary>
    public class PlayerActivity : IEquatable<PlayerActivity>
    {
        // Record total number of frags while player is active
        private int _TotalFrags;
        
        public bool IsScoreReset { get; private set; }

        /// <summary>
        /// Time this player joined the server (more accurate than PlayerSession
        /// <remarks>The Session object's dates do not cross day boundaries, this start time does</remarks>
        /// </summary>
        public DateTime StartTime { get; private set; }

        /// <summary>
        /// Instance of the snapshot retrived from the server
        /// </summary>
        public PlayerSnapshot PlayerSnap { get; private set; }
        public PlayerSession Session { get; private set; }
        public PlayerMatch CurrentMatch { get; private set; }

        private ValueTally<int> shirtTally;
        private ValueTally<int> pantTally;
        private ValueTally<string> skinTally;
        private ValueTally<string> modelTally;
        /// <summary>
        /// Initialized a Player given a Session
        /// </summary>
        /// <param name="pPlayerSnapshot"></param>
        /// <param name="pPlayerSession"></param>
        internal PlayerActivity(PlayerSnapshot pPlayerSnapshot, PlayerSession pPlayerSession)
        {
            _TotalFrags = pPlayerSession.FragCount;

            StartTime = pPlayerSession.SessionStart ?? DateTime.UtcNow;
            Session = pPlayerSession;
            PlayerSnap = pPlayerSnapshot;
        }

        internal PlayerActivity(PlayerSnapshot pPlayerSnapshot, Player pDbPlayer)
        {

            PlayerSnap = pPlayerSnapshot;

            Session = new PlayerSession();
            Session.LastAlias = pDbPlayer;  
            Session.PlayerId = pDbPlayer.PlayerId;
            Session.LastAliasId = pDbPlayer.AliasId;
            Session.SessionStart = DateTime.UtcNow;
            Session.SessionDate = DateTime.UtcNow.Date;
            Session.FragCount = 0;
            Session.CurrentFrags = (pPlayerSnapshot.Frags > 0 ? pPlayerSnapshot.Frags : 0);
            
            StartTime = DateTime.UtcNow;
            _TotalFrags = 0;
        }

        public int TotalFrags
        {
            get { return PlayerSnap.Frags > 0 ? _TotalFrags + PlayerSnap.Frags : _TotalFrags; }
        }

        public override string ToString()
        {
            return Session.PlayerId + " " + PlayerSnap.PlayerName;
        }

        internal void NewMatch()
        {
            CurrentMatch = new PlayerMatch();
            CurrentMatch.PlayerMatchStart = DateTime.UtcNow;
            CurrentMatch.PlayerMatchEnd = null;

            shirtTally = new ValueTally<int>();
            pantTally = new ValueTally<int>();
            skinTally = new ValueTally<string>();
            modelTally = new ValueTally<string>();
        }

        internal void NoMatch()
        {
            CurrentMatch = null;
        }

        internal void UpdatePlayer(PlayerSnapshot pNewPlayerSnapshot, Player pDbPlayer, bool pResurrecting)
        {
            IsScoreReset = false;

            // Update total frags if fragcount has dropped
            if (pNewPlayerSnapshot.Frags < PlayerSnap.Frags 
                && PlayerSnap.Frags > 0
                && pNewPlayerSnapshot.Frags < 5
                && !pResurrecting
                && CurrentMatch != null)
            {
                IsScoreReset = true;

                // PlayerSnap is still previous snapshot
                CurrentMatch.Frags = PlayerSnap.Frags;
                CurrentMatch.PlayerMatchEnd = DateTime.UtcNow;

                _TotalFrags += PlayerSnap.Frags;
                Session.FragCount = _TotalFrags;
            }

            // Will happen if session has Ghosted and been resurrected
            if (pResurrecting)
            {
                StartTime = DateTime.UtcNow;
                Session = new PlayerSession();
                Session.SessionStart = DateTime.UtcNow;
                Session.SessionDate = DateTime.UtcNow.Date;
                Session.FragCount = 0;
                StartTime = DateTime.UtcNow;
            }

            if (CurrentMatch != null)
            {
                // Don't update Current Match if score is resetting, it needs to save first
                if (!IsScoreReset)
                {
                    CurrentMatch.Frags = (pNewPlayerSnapshot.Frags > CurrentMatch.Frags ? pNewPlayerSnapshot.Frags : CurrentMatch.Frags);
                    if (pNewPlayerSnapshot is NetQuakePlayer)
                    {
                        NetQuakePlayer netQuakePlayer = pNewPlayerSnapshot as NetQuakePlayer;
                        pantTally.AddValue(netQuakePlayer.PantColor);
                        shirtTally.AddValue(netQuakePlayer.ShirtColor);
                    }
                    else
                    {
                        if(pNewPlayerSnapshot.SkinName != null)
                            skinTally.AddValue(pNewPlayerSnapshot.SkinName);
                        if(pNewPlayerSnapshot.ModelName != null)
                            modelTally.AddValue(pNewPlayerSnapshot.ModelName);
                    }
                }

                CurrentMatch.Model = modelTally.MostUsed;
                CurrentMatch.Skin = skinTally.MostUsed;
                CurrentMatch.PantColor = pantTally.MostUsed;
                CurrentMatch.ShirtColor = shirtTally.MostUsed;
            }

            Session.CurrentFrags = (pNewPlayerSnapshot.Frags > 0 ? pNewPlayerSnapshot.Frags : 0);
            Session.LastAlias = pDbPlayer;
            Session.LastAliasId = pDbPlayer.AliasId;
            Session.PlayerId = pDbPlayer.PlayerId;
            Session.Latency = pNewPlayerSnapshot.Ping;

            PlayerSnap = pNewPlayerSnapshot;
        }

        internal PlayerSession EndSession()
        {
            if(CurrentMatch != null)
                CurrentMatch.PlayerMatchEnd = DateTime.UtcNow;
            Session.SessionEnd = DateTime.UtcNow;
            Session.CurrentFrags = 0;
            Session.FragCount = _TotalFrags + (PlayerSnap.Frags > 0 ? PlayerSnap.Frags : 0);

            return Session;
        }

        internal PlayerSession EndDaySession(Player pDbPlayer)
        {
            PlayerSession yesterdaySession = Session;
            yesterdaySession.SessionEnd = yesterdaySession.SessionDate.Value.AddHours(23).AddMinutes(59);
            yesterdaySession.CurrentFrags = 0;
            yesterdaySession.FragCount = _TotalFrags;

            Session = new PlayerSession();
            Session.SessionStart = DateTime.UtcNow;
            Session.SessionDate = DateTime.UtcNow.Date;
            Session.LastAlias = pDbPlayer;
            Session.LastAliasId = pDbPlayer.AliasId;
            Session.PlayerId = pDbPlayer.PlayerId;
            Session.CurrentFrags = (PlayerSnap.Frags > 0 ? PlayerSnap.Frags : 0);
            _TotalFrags = 0;

            // DO NOT set StartTime, the whole purpose is to keep record of when the player joined.
            
            return yesterdaySession;
        }

        bool IEquatable<PlayerActivity>.Equals(PlayerActivity other)
        {
            return Session.PlayerId.Equals(Session.PlayerId);
        }

    }
}
