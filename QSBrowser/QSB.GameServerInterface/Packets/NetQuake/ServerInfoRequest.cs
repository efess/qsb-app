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
    internal class ServerInfoRequest : RequestPacket
    {
        private const string SERVER_GAME = "QUAKE";
        private const byte SERVER_GAME_VERSION = 0x03;

        public ServerInfoRequest()
        {
            PacketType = QuakeNetworkPacket.PACKET_CONTROL;
            Command = QuakeNetworkPacket.CCREQ_SERVER_INFO;
        }

        internal override byte[] GetPacket()
        {
            byte[] packetByte = new byte[this.Size];
            InternalGetPacket(packetByte);
            return packetByte;
        }

        protected override int TotalSize
        {
            get { return this.Size; }
        }

        protected new int Size
        {
            get
            {
                return base.Size + SERVER_GAME.Length + 2; // 1 byte for string null terminator, 1 byte for Server Version
            }
        }

        protected override void InternalGetPacket(byte[] pBytes)
        {

            byte[] byteGame = Encoding.ASCII.GetBytes(SERVER_GAME);

            int byteOffset = base.Size;
            for (int i = 0; i < byteGame.Length; i++, byteOffset++)
            {
                pBytes[byteOffset] = byteGame[i];
            }

            // + 1 to skip over the string terminating null character.
            pBytes[byteOffset + 1] = SERVER_GAME_VERSION;

            base.InternalGetPacket(pBytes);
        }
    }
}
