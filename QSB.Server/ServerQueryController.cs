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
using QSB.Data;
using QSB.Data.TableObject;
using QSB.GameServerInterface;
using System.Threading;
using QSB.Common;

namespace QSB.Server
{
    public class ServerQueryController
    {
        /// <summary>
        /// Maximum number of server queries to make at one time
        /// </summary>
        const int MAX_CONCURRENT_QUERIES = 20;
        private IDataSessionFactory _dataSessionFactory;
        private IServerInterface _serverInterface;

        /// <summary>
        /// Timestamp the server was started
        /// </summary>
        public DateTime StartTime { get; private set; }
        public ServerQueryController(IDataSessionFactory pSessionFactory, IServerInterface pServerInterface)
        {
            if (pSessionFactory == null)
                throw new ArgumentNullException("Must provide DataSessionFactory object");

            _serverInterface = pServerInterface;
            _dataSessionFactory = pSessionFactory;
        }
        
        /// <summary>
        /// Perform a query and process query result
        /// </summary>
        /// <param name="pServer"></param>
        /// <returns></returns>
        public void DoQuery(GameServer pServer)
        {
            var dbSession = _dataSessionFactory.GetDataSession();
            ServerQuery query = new ServerQuery(pServer, _serverInterface);

            dbSession.BeginTransaction();
            
            query.GetServerSnapshot();
            ProcessQueryResult(query, dbSession);

            if (dbSession.IsInTransaction)
                dbSession.CommitTransaction();

            dbSession.FlushData();
        }

        /// <summary>
        /// Perform all necessary queries for servers needing it
        /// Queries are performed asynchronously
        /// </summary>
        public void DoQueries()
        {
            var dbSession = _dataSessionFactory.GetDataSession();
            IList<GameServer> GameServers = dbSession.GetServersByLastQueried(DateTime.UtcNow);

            ManualResetEvent[] waitEvents;

            int counter = 0;
            while (counter < GameServers.Count)
            {
                int serverQueryCount = Math.Min(GameServers.Count - counter, MAX_CONCURRENT_QUERIES);
                waitEvents = new ManualResetEvent[serverQueryCount];
                List<ServerQuery> queries = new List<ServerQuery>();

                for (int i = 0; i < serverQueryCount; i++, counter++)
                { 
                    ServerQuery query = new ServerQuery(GameServers[counter], _serverInterface);
                    waitEvents[i] = query.WaitEvent;
                    ThreadPool.QueueUserWorkItem(query.ThreadPoolCallback);
                    queries.Add(query);
                }

                WaitHandle.WaitAll(waitEvents);


                dbSession.BeginTransaction();
                foreach (ServerQuery query in queries)
                {
                    ProcessQueryResult(query, dbSession);
                }
                if (dbSession.IsInTransaction)
                    dbSession.CommitTransaction();

                dbSession.FlushData();
            }
        }

        /// <summary>
        /// Processes a server Query Response, and handles grunt work of keeping the cache up to date
        /// </summary>
        /// <param name="pQuery"></param>
        public void ProcessQueryResult(ServerQuery pQuery, IDataSession pDataSession)
        {
            ServerActivity activity = ServerCache.Cache[pQuery.DbServer.ServerId];

            // Reload server data in case anything changed
            var gameServer = pDataSession.GetServer(pQuery.DbServer.ServerId);

            gameServer.LastQuery = DateTime.UtcNow;

            if (pQuery.ServerSnapshot.Status != ServerStatus.Running)
            {
                if (activity != null && activity.ServerSnapshot != null)
                    activity.UpdateStatus(pQuery.ServerSnapshot.Status);

                // Use old DB object to report on the DNS actually used (in case it changed)
                System.Diagnostics.Debug.WriteLine("Query to " + pQuery.DbServer.DNS + ":" + pQuery.DbServer.Port.ToString() + " failed");

                // Server query issue
                switch (pQuery.ServerSnapshot.Status)
                {
                        // Connected to the server, no response however
                    case ServerStatus.NotResponding:
                        if (gameServer.FailedQueryAttempts > 10)
                            gameServer.NextQuery = DateTime.UtcNow.AddMinutes(15);
                        break;
                    case ServerStatus.NotFound:
                        // Couldn't connect or find the server
                        if (gameServer.FailedQueryAttempts > 10)
                            gameServer.NextQuery = DateTime.UtcNow.AddHours(2);
                        break;
                    case ServerStatus.QueryError:
                        // Couldn't find the server
                        if (gameServer.FailedQueryAttempts > 10)
                            gameServer.NextQuery = DateTime.UtcNow.AddHours(2);
                        break;
                }

                gameServer.FailedQueryAttempts++;
                gameServer.QueryResult = (int)pQuery.ServerSnapshot.Status;
                pDataSession.AddUpdateServer(gameServer);
                return;
            }

            // Server is running
            ActivityInstanceSynch activitySynchronizer = new ActivityInstanceSynch(pDataSession, pQuery.DbServer);

            if (activity == null)
            {
                activity = activitySynchronizer.CreateNewActivity(pQuery.ServerSnapshot);
                activity.UpdateStatus(pQuery.ServerSnapshot.Status);
            }
            else
            {
                activity.UpdateStatus(pQuery.ServerSnapshot.Status);
                activitySynchronizer.SynchExistingActivity(activity, pQuery.ServerSnapshot);
            }

            pQuery.DbServer.LastQuerySuccess = DateTime.UtcNow;
            pQuery.DbServer.FailedQueryAttempts = 0;
            pQuery.DbServer.QueryResult = (int)pQuery.ServerSnapshot.Status;

            pQuery.DbServer.NextQuery = null; // This is awkward. I do this to make the query only run off LastQuery + QueryInterval.

            pDataSession.AddUpdateServerDataObject(new SnapshotInstance(activity).GetServerDataObject());
            pDataSession.AddUpdateServer(pQuery.DbServer);
        }
    }
}
