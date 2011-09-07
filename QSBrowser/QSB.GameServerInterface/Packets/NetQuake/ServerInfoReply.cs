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
    internal class ServerInfoReply : ReplyPacket
    {
        internal string Address;
        internal string HostName;
        internal string MapName;
        internal byte CurrentPlayers;
        internal byte MaxPlayers;
        internal byte GameProtocol;

        protected override int TotalSize
        {
            get { return 0; }
        }
        protected new int Size
        {
            get { throw new NotImplementedException(); }
        }

        public ServerInfoReply()
        {
        }

        protected override void InternalSetPacket(byte[] pBytes)
        {
            base.InternalSetPacket(pBytes);
        }

        internal override void SetPacket(byte[] pBytes)
        {
            int byteOffset = base.Size;

            Address = Packet.GetNullTerminatedString(pBytes, byteOffset);
            byteOffset += Address.Length + 1;
            HostName = Packet.GetNullTerminatedString(pBytes, byteOffset);
            byteOffset += HostName.Length + 1;
            MapName = Packet.GetNullTerminatedString(pBytes, byteOffset);
            byteOffset += MapName.Length + 1;

            CurrentPlayers = pBytes[byteOffset++];
            MaxPlayers = pBytes[byteOffset++];
            GameProtocol = pBytes[byteOffset++];

            base.InternalSetPacket(pBytes);
        }
    }
}
