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

namespace QSB.GameServerInterface.Packets.NetQuake
{
    internal class PlayerInfoReply : ReplyPacket
    {
        internal byte PlayerNumber { get; set; }
        internal byte[] PlayerName { get; set; }
        internal string Address { get; set; }
        internal byte ShirtColor { get; set; }
        internal byte PantColor { get; set; }
        internal int PlayTime { get; set; }
        internal int FragCount { get; set; }

        internal override void SetPacket(byte[] pBytes)
        {
            int byteCounter = this.Size;
            PlayerNumber = pBytes[byteCounter++];

            PlayerName = Packet.GetNullTerminatedBytes(pBytes, byteCounter);
            byteCounter += PlayerName.Length + 1;

            byte colors = pBytes[byteCounter++];
            ShirtColor = (byte)((colors >> 4) & 0x0F);
            PantColor = (byte)((colors) & 0x0F);

            // Next three are unused
            byteCounter += 3;

            FragCount = (int)((pBytes[byteCounter + 1] << 8) | pBytes[byteCounter]);
            byteCounter += 4;

            PlayTime = (int)((pBytes[byteCounter + 1] << 8) | pBytes[byteCounter]);
            byteCounter += 4;

            Address = Packet.GetNullTerminatedString(pBytes, byteCounter);
        }

        protected override int TotalSize
        {
            get { throw new NotImplementedException(); }
        }
    }
}
