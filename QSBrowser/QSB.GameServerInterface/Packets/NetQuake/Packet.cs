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
    internal abstract class Packet
    {
        /// <summary>
        /// Size of Subpacket
        /// </summary>
        protected int Size { get; private set; }
        /// <summary>
        /// Total size of entire packet
        /// </summary>
        protected abstract int TotalSize { get; }
        /// <summary>
        /// Subpacket calls in order to apply to byte array
        /// </summary>
        /// <param name="pBytes"></param>
        protected abstract void InternalGetPacket(byte[] pBytes);
        /// <summary>
        /// Subpacket calls in order to interpret byte array
        /// </summary>
        /// <param name="pBytes"></param>
        protected abstract void InternalSetPacket(byte[] pBytes);

        protected static string GetNullTerminatedString(byte[] pBytes, int pOffset)
        {
            int thisOffset = pOffset;
            StringBuilder sb = new StringBuilder();
            
            for (int i = 0; pBytes[thisOffset] != 0x00; i++, thisOffset++)
            {
                sb.Append((char)pBytes[thisOffset]);
            }

            return sb.ToString();
        }

        protected static byte[] GetNullTerminatedBytes(byte[] pBytes, int pOffset)
        {
            int thisOffset = pOffset;
            int length = 0;

            while (thisOffset < pBytes.Length && 
                pBytes[thisOffset] != 0x00)
            {
                length++;
                thisOffset++;
            }

            byte[] bytes = new byte[length];
            thisOffset = pOffset;

            for (int i = 0; pBytes[thisOffset] != 0x00; i++, thisOffset++)
            {
                bytes[i] = pBytes[thisOffset];
            }

            return bytes;
        }

        protected static void SetStringInBytes(byte[] pBytes, string pString, int pOffset)
        {
            Encoding.ASCII.GetBytes(pString, 0, pString.Length, pBytes, pOffset);
        }
    }
}

