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
    internal abstract class ReplyPacket : QuakeNetworkPacket
    {
        public const int CONTROL_BYTE_OFFSET = 4;
        private const int PACKET_SIZE = 1;

        protected byte Command;

        public static byte GetControlByte(byte[] pBytes)
        {
            if (pBytes.Length <= CONTROL_BYTE_OFFSET)
                throw new Exception("Not a valid Reply");

            return pBytes[CONTROL_BYTE_OFFSET];
        }

        protected new int Size
        {
            get
            {
                return base.Size + PACKET_SIZE;
            }
        }

        protected override void InternalSetPacket(byte[] pBytes)
        {
            Command = pBytes[base.Size];
            base.InternalSetPacket(pBytes);
        }

        internal abstract void SetPacket(byte[] pBytes);
    }
}
