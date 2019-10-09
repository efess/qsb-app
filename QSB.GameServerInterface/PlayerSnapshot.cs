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
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Imaging;

namespace QSB.GameServerInterface
{
    /// <summary>
    /// Internal Information object representing a player entity on a server
    /// </summary>
    public class PlayerSnapshot
    {
        /// <summary>
        /// External identification of this player
        /// </summary>
        public int PlayerId { get; set; }
        /// <summary>
        /// Player's IPAddress
        /// </summary>
        public string IpAddress { get;  set; }
        /// <summary>
        /// Player's server assigned number
        /// </summary>
        public int PlayerNumber { get;  set; }
        /// <summary>
        /// Player's Name (text)
        /// </summary>
        public string PlayerName { get;  set; }
        /// <summary>
        /// Players'Name (bytes)
        /// </summary>
        public byte[] PlayerNameBytes { get;  set; }
        /// <summary>
        /// Skin player is using
        /// </summary>
        public string SkinName { get;  set; }
        /// <summary>
        /// Model Player is using
        /// </summary>
        public string ModelName { get;  set; }
        /// <summary>
        /// Ping as reported by the server
        /// </summary>
        public int Ping { get;  set; }
        /// <summary>
        /// Current score
        /// </summary>
        public int Frags { get;  set; }
        /// <summary>
        /// Current server reported playing time of player
        /// </summary>
        public TimeSpan PlayTime { get;  set; }

        /// <summary>
        /// Constructor for comparison purposes
        /// </summary>
        /// <param name="pPlayerBytes"></param>
        /// <param name="pIpAddress"></param>
        /// <param name="pPlayerName"></param>
        /// <param name="pPlayerNumber"></param>
        public PlayerSnapshot(byte[] pPlayerBytes, string pIpAddress, string pPlayerName, int pPlayerNumber)
        {
            PlayerName = pPlayerName;
            IpAddress = pIpAddress;
            PlayerNumber = pPlayerNumber;
            PlayerNameBytes = pPlayerBytes;
        }
    }
}
