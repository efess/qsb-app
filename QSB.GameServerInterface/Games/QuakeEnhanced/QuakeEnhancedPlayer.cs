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
using QSB.GameServerInterface.Games.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QSB.GameServerInterface.Games.QuakeEnhanced
{
    public class QuakeEnhancedPlayer : PlayerSnapshot, IClothed, IPlayerType
    {
        public int ShirtColor { get; internal set; }
        public int PantColor { get; internal set; }
        public PlayerType PlayerType { get; internal set; }

        public QuakeEnhancedPlayer(byte[] pPlayerBytes, string pIpAddress, string pPlayerName, int pPlayerNumber)
            : base(pPlayerBytes, pIpAddress, pPlayerName, pPlayerNumber)
        {
        }
    }
}
