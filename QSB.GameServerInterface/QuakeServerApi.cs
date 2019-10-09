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
using QSB.GameServerInterface.Packets;
using QSB.Common;
using System.Net.Sockets;

namespace QSB.GameServerInterface
{
    public interface IServerInterface
    {
        Exception LastError { get; }
        ServerSnapshot GetServerInfo(string pServerAddress, int pServerPort, Game pGame);
    }

    public class ServerInterface : IServerInterface
    {
        public Exception LastError { get; private set; }

        /// <summary>
        /// Call out to server and retreive information 
        /// </summary>
        /// <param name="pServerAddress">IP address or DNS of server</param>
        /// <param name="pServerPort">TCP/IP port to communicate on</param>
        /// <param name="pGame">Which game type server runs</param>
        /// <returns>ServerInfo object containing gathered informatino</returns>
        public ServerSnapshot GetServerInfo(string pServerAddress, int pServerPort, Game pGame)
        {
            IServerInfoProvider infoProvider = null;

            switch (pGame)
            {
                case Game.NetQuake:
                    infoProvider = new QSB.GameServerInterface.Games.NetQuake.NetQuake();
                    break;
                case Game.QuakeWorld:
                    infoProvider = new QSB.GameServerInterface.Games.QuakeWorld.QuakeWorld();
                    break;
                case Game.Quake2:
                    infoProvider = new QSB.GameServerInterface.Games.Quake2.Quake2();
                    break;
                case Game.Quake3:
                    infoProvider = new QSB.GameServerInterface.Games.Quake3.Quake3();
                    break;
                default:
                    throw new NotSupportedException("Game is not supported at this time");

            }
            ServerSnapshot serverInfo = new ServerSnapshot();
            try
            {
                serverInfo = infoProvider.GetServerInfo(pServerAddress, pServerPort);
                serverInfo.Status = ServerStatus.Running;
            }
            catch (SocketException ex)
            {
                LastError = ex;
                if (ex.SocketErrorCode == SocketError.HostNotFound)
                    serverInfo.Status = ServerStatus.NotFound;
                else //ex.SocketErrorCode == SocketError.TimedOut
                    serverInfo.Status = ServerStatus.NotResponding;
            }
            catch (Exception ex)
            {
                LastError = new ServerQueryParseException(ex);
                serverInfo.Status = ServerStatus.QueryError;
            }

            return serverInfo;
        }
    }
}
