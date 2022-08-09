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
using QSB.GameServerInterface;
using System.Threading;
using QSB.Common;

namespace QSB.Server
{
    public class ServerQuery
    {
        /// <summary>
        /// If server is not responding, retry this many times
        /// </summary>
        const int SERVER_NOT_RESPONDING_RETRY_COUNT = 1;

        public GameServer DbServer {get; private set;}
        public ServerSnapshot ServerSnapshot { get; private set; }
        public ManualResetEvent WaitEvent {get; private set;}

        private IServerInterface _serverInterface;

        internal ServerQuery(GameServer pGameServer, IServerInterface pServerInterface)
        {
            _serverInterface = pServerInterface;
            WaitEvent = new ManualResetEvent(false);
            DbServer = pGameServer;
        }

        // Wrapper method for use with thread pool.
        public void ThreadPoolCallback(Object threadContext)
        {
            GetServerSnapshot();
            WaitEvent.Set();
        }

        public void GetServerSnapshot()
        {
            ServerSnapshot serverInfo = null;

            int pRetryCount = 0;
            do
            {
                System.Diagnostics.Debug.WriteLine("Querying " + DbServer.DNS + ":" + DbServer.Port.ToString());
                serverInfo = _serverInterface.GetServerInfo(DbServer.DNS.Trim(), DbServer.Port, (Game)DbServer.GameId, DbServer.Parameters);
                // Retry logic:
            } while (pRetryCount++ < SERVER_NOT_RESPONDING_RETRY_COUNT
                && serverInfo.Status == ServerStatus.NotResponding);

            ServerSnapshot = serverInfo;
        }
    }
}
