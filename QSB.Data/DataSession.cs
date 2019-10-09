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
using NHibernate;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Cfg;
using QSB.Data.TableObject;
using QSB.Common;
using NHibernate.Criterion;
using QSB.Data.ViewObject;

namespace QSB.Data
{
    /// <summary>
    /// Non database specific code
    /// </summary>
    public abstract class DataSession
    {
        protected object _lockObject;
        protected ISession _DbSession;

        public virtual bool IsInTransaction
        {
            get
            {
                return _DbSession.Transaction.IsActive;
            }
        }

        public virtual void CommitTransaction()
        {
            _DbSession.Transaction.Commit();
        }

        public virtual void BeginTransaction()
        {
            _DbSession.Transaction.Begin();
        }

        // Close all open player sessions
        public virtual void CleanupDatabase()
        {
            bool wasInTransaction = true;
            if (!IsInTransaction)
            {
                wasInTransaction = false;
                _DbSession.Transaction.Begin();
            }
            foreach (PlayerSession pSession
                in _DbSession.CreateCriteria(typeof(PlayerSession)).Add(Expression.IsNull("SessionEnd")).List<PlayerSession>())
            {
                pSession.SessionEnd = DateTime.UtcNow;
                _DbSession.SaveOrUpdate(pSession);
            }
            if (!wasInTransaction)
                CommitTransaction();
        }

        public virtual IList<PlayerSession> GetInSessionPlayersByServer(int pServerId)
        {
            IList<PlayerSession> sessions = _DbSession.CreateCriteria(typeof(PlayerSession))
                .Add(Expression.IsNull("SessionEnd"))
                .Add(Expression.Eq("ServerId", pServerId))
                .List<PlayerSession>();

            foreach (PlayerSession session in sessions)
            {
                session.LastAlias = GetPlayer(session.LastAliasId);
            }

            return sessions;
        }

        public virtual UserAccess GetAccess(string pUserName)
        {
            IList<UserAccess> user = _DbSession.CreateCriteria(typeof(UserAccess))
                .Add(Expression.Eq("Name", pUserName))
                .List<UserAccess>();

            if (user.Count > 0)
                return user[0];

            return null;
        }

        public virtual void SetUserActivity(UserAccess pUserAccess)
        {
            bool wasInTransaction = true;
            if (!IsInTransaction)
            {
                wasInTransaction = false;
                _DbSession.Transaction.Begin();
            }
            _DbSession.SaveOrUpdate(pUserAccess);
            if (!wasInTransaction)
            {
                _DbSession.Transaction.Commit();
            }
        }

        public virtual Player GetPlayer(int pAliasId)
        {
            IList<Player> players = _DbSession.CreateCriteria(typeof(Player))
                .Add(Expression.Eq("AliasId", pAliasId))
                .List<Player>();

            if (players.Count > 0)
                return players[0];
            return null;
        }

        public virtual IList<Player> GetAliases(int pPlayerId)
        {
            return _DbSession.CreateCriteria(typeof(Player))
                .Add(Expression.Eq("PlayerId", pPlayerId))
                .List<Player>();
        }

        public virtual void DeleteServer(GameServer pServer)
        {
            bool wasInTransaction = true;
            if (!IsInTransaction)
            {
                wasInTransaction = false;
                _DbSession.Transaction.Begin();
            }
            _DbSession.Delete(pServer);
            if (!wasInTransaction)
                CommitTransaction();
        }

        public virtual GameServer GetServer(int pServerId)
        {
            IList<GameServer> servers = _DbSession.CreateCriteria(typeof(GameServer)).Add(Expression.Eq("ServerId", pServerId)).List<GameServer>();

            if (servers.Count == 1)
                return servers[0] as GameServer;

            if (servers.Count > 1)
                throw new Exception("Duplicate server found in Database");
            return null;
        }

        public virtual IList<PlayerDetail> GetPlayerDetail(int pPlayerId)
        {
            return _DbSession.CreateCriteria(typeof(PlayerDetail)).Add(Expression.Eq("PlayerId", (int)pPlayerId)).List<PlayerDetail>();
        }

        public virtual IList<GameServer> GetServersByGame(Game pGameId)
        {
            return _DbSession.CreateCriteria(typeof(GameServer)).Add(Expression.Eq("GameId", (int)pGameId)).List<GameServer>();
        }

        public virtual IList<PlayerSessionHourlySummery> GetHourlySummeryReport(DateTime pDateTime)
        {
            return _DbSession.CreateCriteria(typeof(PlayerSessionHourlySummery))
                .Add(Expression.Sql("SessionDate = date('" + pDateTime.ToString("yyyy-MM-dd") + "')"))
                .List<PlayerSessionHourlySummery>();
        }

        public virtual IList<PlayerSessionHourlySummery> GetHourlySummeryForPlayer(DateTime pDateTime, int pPlayerId)
        {
            return _DbSession.CreateCriteria(typeof(PlayerSessionHourlySummery))
                .Add(Expression.Sql("SessionDate = date('" + pDateTime.ToString("yyyy-MM-dd") + "')"))
                .Add(Expression.Eq("PlayerId", pPlayerId))
                .List<PlayerSessionHourlySummery>();
        }

        public virtual IList<PlayerSessionHourlySummery> GetHourlySummeryForServer(DateTime pDateTime, int pServerId)
        {
            return _DbSession.CreateCriteria(typeof(PlayerSessionHourlySummery))
                .Add(Expression.Sql("SessionDate = date('" + pDateTime.ToString("yyyy-MM-dd") + "')"))
                .Add(Expression.Eq("ServerId", pServerId))
                .List<PlayerSessionHourlySummery>();
        }

        public virtual GameServer GetServerByDNS(string pDNS, int pPort)
        {
            IList<GameServer> servers = _DbSession.CreateCriteria(typeof(GameServer)).Add(Expression.Eq("DNS", pDNS)).Add(Expression.Eq("Port", pPort)).List<GameServer>();
            if (servers.Count == 1)
                return servers[0] as GameServer;
            if (servers.Count > 1)
                throw new Exception("Duplicate server found in Database");
            return null;
        }

        public virtual GameServer GetServerByIPAddress(string IPAddress, int pPort)
        {
            IList<GameServer> servers = _DbSession.CreateCriteria(typeof(GameServer)).Add(Expression.Eq("IPAddress", IPAddress)).Add(Expression.Eq("Port", pPort)).List<GameServer>();
            if (servers.Count == 1)
                return servers[0] as GameServer;
            if (servers.Count > 1)
                throw new Exception("Duplicate server found in Database");
            return null;
        }
        public virtual IList<HistoricalHourlyLog> GetSingleHourlyLogEntry(DateTime pDateTime)
        {
            return _DbSession.CreateCriteria(typeof(HistoricalHourlyLog))
                .Add(Expression.Sql("HistoricalDate = date(" + pDateTime.ToString("yyyy-MM-dd") + ")"))
                .SetMaxResults(1)
                .List<HistoricalHourlyLog>();
        }

        public virtual IList<ServerData> GetServerData()
        {
            return _DbSession.CreateCriteria(typeof(ServerData))
                .List<ServerData>();
        }

        public virtual IList<VServerDetail> GetServerDetail()
        {
            return _DbSession.CreateCriteria(typeof(VServerDetail))
                .List<VServerDetail>();
        }

        public virtual void AddUpdateServer(GameServer pServer)
        {
            bool wasInTransaction = true;
            if (!IsInTransaction)
            {
                wasInTransaction = false;
                _DbSession.Transaction.Begin();
            }
            _DbSession.SaveOrUpdate(pServer);
            if (!wasInTransaction)
                CommitTransaction();
        }

        public virtual void AddHistoricalHourlyLog(HistoricalHourlyLog pHourlyLog)
        {
            bool wasInTransaction = true;
            if (!IsInTransaction)
            {
                wasInTransaction = false;
                _DbSession.Transaction.Begin();
            }

            _DbSession.SaveOrUpdate(pHourlyLog);

            if (!wasInTransaction)
                CommitTransaction();
        }

        public virtual void AddPlayerMatch(PlayerMatch pPlayerMatch)
        {

            bool wasInTransaction = true;
            if (!IsInTransaction)
            {
                wasInTransaction = false;
                _DbSession.Transaction.Begin();
            }
            _DbSession.SaveOrUpdate(pPlayerMatch);
            if (!wasInTransaction)
                CommitTransaction();
        }

        public virtual void AddServerMatch(ServerMatch pServerMatch)
        {

            bool wasInTransaction = true;
            if (!IsInTransaction)
            {
                wasInTransaction = false;
                _DbSession.Transaction.Begin();
            }
            _DbSession.SaveOrUpdate(pServerMatch);
            if (!wasInTransaction)
                CommitTransaction();
        }

        public virtual void AddUpdatePlayerSession(PlayerSession pPlayer)
        {

            bool wasInTransaction = true;
            if (!IsInTransaction)
            {
                wasInTransaction = false;
                _DbSession.Transaction.Begin();
            }
            _DbSession.SaveOrUpdate(pPlayer);
            if (!wasInTransaction)
                CommitTransaction();
        }

        public virtual void AddUpdateServerDataObject(ServerData pServerData)
        {
            bool wasInTransaction = true;
            if (!IsInTransaction)
            {
                wasInTransaction = false;
                _DbSession.Transaction.Begin();
            }

            foreach (ServerData data in _DbSession.CreateCriteria(typeof(ServerData))
                    .Add(Expression.Eq("ServerId", pServerData.ServerId))
                    .List<ServerData>())
            {
                _DbSession.Delete(data);
            }

            _DbSession.SaveOrUpdate(pServerData);

            if (!wasInTransaction)
                CommitTransaction();
        }

        /// <summary>
        /// Manages Player database...
        /// </summary>
        /// <param name="pAlias"></param>
        /// <param name="pIpAddress"></param>
        /// <returns></returns>
        public virtual Player GetOrCreatePlayer(string pAlias, string pIpAddress, byte[] pAliasBytes, int pGameId)
        {
            IList<Player> aliases = null;
            Player newPlayer = null;

            if (StringHelper.ReasonableIPAddress(pIpAddress))
            {
                // OK IP address, check filters

                // Check for SameIP address, Same Alias, IdentifyIP on
                aliases = _DbSession.CreateCriteria(typeof(Player))
                    .Add(Expression.Eq("AliasBytes", pAliasBytes))
                    .Add(Expression.Eq("IPAddress", pIpAddress))
                    .Add(Expression.Eq("GameId", pGameId))
                    .Add(Expression.Eq("IdentifyIPAddress", 1))
                    .List<Player>();

                if (aliases.Count > 0)
                    return aliases[0];

                // Check for SameIP address, different alias, IdentifyIP on
                aliases = _DbSession.CreateCriteria(typeof(Player))
                    .Add(Expression.Eq("IPAddress", pIpAddress))
                    .Add(Expression.Eq("GameId", pGameId))
                    .Add(Expression.Eq("IdentifyIPAddress", 1))
                    .List<Player>();

                if (aliases.Count > 0)
                {
                    Player alias = aliases[0];

                    newPlayer = new Player();
                    newPlayer.IdentifyIPAddress = alias.IdentifyIPAddress;
                    newPlayer.PlayerId = alias.PlayerId;
                    newPlayer.Alias = pAlias;
                    newPlayer.AliasBytes = pAliasBytes;
                    newPlayer.IPAddress = pIpAddress;
                    newPlayer.GameId = pGameId;
                }

                // Check for SameIP, Same Alias, IdentifyIP OFF
                aliases = _DbSession.CreateCriteria(typeof(Player))
                    .Add(Expression.Eq("AliasBytes", pAliasBytes))
                    .Add(Expression.Eq("GameId", pGameId))
                    .Add(Expression.Eq("IPAddress", pIpAddress))
                    .Add(Expression.Eq("IdentifyIPAddress", 0))
                        .List<Player>();

                if (aliases.Count > 0)
                    return aliases[0];
            }

            // If aliases are not found, search *non group by*
            if (newPlayer == null)
            {
                // Check for an AliasName match, and IdentifyByIPAddress of 0 
                aliases = _DbSession.CreateCriteria(typeof(Player))
                    .Add(Expression.Eq("AliasBytes", pAliasBytes))
                    .Add(Expression.Eq("GameId", pGameId))
                    .Add(Expression.Eq("IdentifyIPAddress", 0))
                        .List<Player>();

                if (aliases.Count > 0)
                    return aliases[0];

                // If newplayer is still null, create a new player
                newPlayer = new Player();

                // New player name never seen before
                newPlayer.Alias = pAlias;
                newPlayer.AliasBytes = pAliasBytes;
                newPlayer.IPAddress = pIpAddress;
                newPlayer.GameId = pGameId;
            }

            bool wasInTransaction = true;
            if (!IsInTransaction)
            {
                wasInTransaction = false;
                _DbSession.Transaction.Begin();
            }

            if (newPlayer.PlayerId == 0)
            {
                newPlayer.PlayerId = NextPlayerId();
            }

            _DbSession.Save(newPlayer);

            if (!wasInTransaction)
                CommitTransaction();

            return newPlayer;
        }

        public virtual int NextPlayerId()
        {
            IQuery query = _DbSession.CreateSQLQuery("SELECT ifnull(MAX(PlayerId),0) FROM Player");
            object obj = query.UniqueResult();
            if (obj is Int64)
                return Convert.ToInt32(obj) + 1;
            else throw new InvalidCastException("Cannot convert object to int");
        }

        public virtual void FlushData()
        {
            _DbSession.Flush();
        }

        /// <summary>
        /// Retrieve a list of PlayerMatches (history)
        /// </summary>
        /// <param name="pPlayerId">Identification of Player</param>
        /// <returns>List of PlayerMatches</returns>
        public virtual IList<PlayerMatches> GetLastTenPlayerMatches(int pPlayerId)
        {
            return _DbSession.CreateCriteria(typeof(PlayerMatches))
                .Add(Expression.Eq("PlayerId", pPlayerId))
                .Add(Expression.Gt("PlayerStayDuration", 60)) // Greater than 60 seconds
                .SetMaxResults(10)
                .AddOrder(new Order("PlayerJoinTime", false))
                .List<PlayerMatches>();
        }

        /// <summary>
        /// Retrieve details of a Match
        /// </summary>
        /// <param name="pMatchId">Identification of Match to retrieve</param>
        /// <returns>MatchDetail</returns>
        public virtual IList<MatchDetail> GetMatchDetail(int pMatchId)
        {
            return _DbSession.CreateCriteria(typeof(MatchDetail))
                .Add(Expression.Eq("MatchId", pMatchId))
                .Add(Expression.Gt("PlayerStayDuration", 60)) // Greater than 60 seconds
                .AddOrder(new Order("Frags", false))
                .List<MatchDetail>();
        }

        public virtual IList<GameServer> GetServers()
        {
            return _DbSession.CreateCriteria(typeof(GameServer)).List<GameServer>();
        }

        public virtual void ProcessStats(int pServerId, DateTime pDate)
        {
            throw new NotImplementedException("Not supported using this data engine");
        }
    }
}
