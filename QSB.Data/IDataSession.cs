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
using QSB.Data.TableObject;
using QSB.Data.ViewObject;
using QSB.Common;

namespace QSB.Data
{

    public interface IDataSession
    {
        bool IsInTransaction { get; }
        void CommitTransaction();
        void BeginTransaction();
        void CleanupDatabase();
        IList<PlayerSession> GetInSessionPlayersByServer(int pServerId);
        UserAccess GetAccess(string pUserName);
        void SetUserActivity(UserAccess pUserAccess);
        Player GetPlayer(int pAliasId);
        IList<Player> GetAliases(int pPlayerId);
        void DeleteServer(GameServer pServer);
        GameServer GetServer(int pServerId);
        IList<PlayerDetail> GetPlayerDetail(int pPlayerId);
        IList<GameServer> GetServersByGame(Game pGameId);
        IList<GameServer> GetServersByLastQueried(DateTime pTimeNow);
        IList<GameServer> GetServers();
        GameServer GetServerByDNS(string pDNS, int pPort);
        GameServer GetServerByIPAddress(string IPAddress, int pPort);
        void AddUpdateServer(GameServer pServer);
        void AddPlayerMatch(PlayerMatch pPlayerMatch);
        void AddServerMatch(ServerMatch pServerMatch);
        void AddUpdatePlayerSession(PlayerSession pPlayer);
        void AddUpdateServerDataObject(ServerData pServerDataObject);
        Player GetOrCreatePlayer(string pAlias, string pIpAddress, byte[] pAliasBytes, int pGameId);
        int NextPlayerId();
        void FlushData();
        IList<PlayerMatches> GetLastTenPlayerMatches(int pPlayerId);
        IList<MatchDetail> GetMatchDetail(int pMatchId);
        IList<ServerData> GetServerData();
        void AddHistoricalHourlyLog(HistoricalHourlyLog pHourlyLog);
        IList<PlayerSessionHourlySummery> GetHourlySummeryForServer(DateTime pDateTime, int pServerId);
        IList<PlayerSessionHourlySummery> GetHourlySummeryForPlayer(DateTime pDateTime, int pPlayerId);
        IList<PlayerSessionHourlySummery> GetHourlySummeryReport(DateTime pDateTime);
        IList<HistoricalHourlyLog> GetSingleHourlyLogEntry(DateTime pDateTime);
        IList<VServerDetail> GetServerDetail();
        void ProcessStats(int pServerId, DateTime pDate);
    }
}
