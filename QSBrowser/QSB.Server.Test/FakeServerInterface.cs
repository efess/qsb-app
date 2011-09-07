/*
Copyright 2009-2010 Joe Lukacovic

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

namespace QSB.Server.Test
{
    public class FakeServerInterface : IServerInterface
    {

        private string[] MapArray = new string[] // 20 elements
        {
            "dm1",
            "dm2",
            "dm3",
            "dm4",
            "dm5",
            "dm6",
            "dm7",
            "e1m1",
            "e1m2",
            "e1m3",
            "e1m4",
            "e1m5",
            "e1m6",
            "e1m7",
            "e2m1",
            "e2m2",
            "e2m3",
            "e2m4",
            "e2m5",
            "end"            
        };

        private string[] PlayerArray = new string[] // 20 elements
        {
            "ef3ss",
            "Phenom",
            "shok",
            "wolvsauce",
            "lennox",
            "krix",
            "tical",
            "badaim",
            "glitch",
            "metch",
            "mastersplinter",
            "souless",
            "Dreadful",
            "uno",
            "Baker",
            "Solecord",
            "rev",
            "Jesus",
            "Bluntz",
            "eieio"
        };

        private string[] ServerArray = new string[] // 4 elements
        {
            "speaknow.quakeone.com",
            "quake.crmod.com",
            "quake.shmack.net",
            "bigfoot.quake1.net"
        };

        public ServerSnapshot CurrentServerState;

        #region IServerInterface Members

        public Exception LastError
        {
            get { return null; }
        }

        public ServerSnapshot GetServerInfo(string pServerAddress, int pServerPort, QSB.Common.Game pGame)
        {
            return CurrentServerState;
        }

        #endregion

    }
}
