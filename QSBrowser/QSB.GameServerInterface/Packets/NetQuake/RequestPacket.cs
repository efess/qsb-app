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

namespace QSB.GameServerInterface.Packets.NetQuake
{
    internal abstract class RequestPacket : QuakeNetworkPacket
    {
        private const int PACKET_SIZE = 1;

        protected byte Command;

        protected new int Size
        {
            get
            {
                return base.Size + PACKET_SIZE;
            }
        }

        protected override void InternalGetPacket(byte[] pBytes)
        {
            pBytes[base.Size] = Command;
            base.InternalGetPacket(pBytes);
        }

        internal abstract byte[] GetPacket();
    }
}
