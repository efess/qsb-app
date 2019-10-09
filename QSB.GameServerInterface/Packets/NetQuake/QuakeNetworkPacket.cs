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
    internal abstract class QuakeNetworkPacket : Packet
    {
        protected const ushort PACKET_CONTROL = 0x8000;

        internal const byte CCREP_ACCEPT = 0x81;
        internal const byte CCREP_REJECT = 0x82;
        internal const byte CCREP_SERVER_INFO = 0x83;
        internal const byte CCREP_PLAYER_INFO = 0x84;
        internal const byte CCREP_RULE_INFO = 0x85;

        internal const byte CCREQ_SERVER_INFO = 0x02;
        internal const byte CCREQ_PLAYER_INFO = 0x03;
        internal const byte CCREQ_RULE_INFO = 0x04; 
        

        private const int PACKET_SIZE = 4;

        protected ushort PacketLen;
        protected ushort PacketType;

        protected new int Size
        {
            get { return PACKET_SIZE; }
        }
        protected override void InternalGetPacket(byte[] pBytes)
        {
            if (pBytes.Length < PACKET_SIZE)
                throw new Exception("Invalid byte size");

            int byteCounter = 0;

            byte[] byteType = BitConverter.GetBytes(PacketType);
            byte[] byteLen = BitConverter.GetBytes(this.TotalSize);

            pBytes[byteCounter++] = byteType[1];
            pBytes[byteCounter++] = byteType[0];
            pBytes[byteCounter++] = byteLen[1];
            pBytes[byteCounter++] = byteLen[0];
        }

        protected override void InternalSetPacket(byte[] pBytes)
        {
            if (pBytes.Length < PACKET_SIZE)
                throw new Exception("Invalid byte size");

            int byteCounter = 0;

            PacketType = BitConverter.ToUInt16(pBytes, byteCounter);
            byteCounter += 2;
            PacketLen = BitConverter.ToUInt16(pBytes, byteCounter);
        }
    }
}
