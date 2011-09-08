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
    internal class RuleInfoReply : ReplyPacket
    {
        internal string RuleName { get; private set; }
        internal string RuleValue { get; private set; }

        protected override int TotalSize
        {
            get { throw new NotImplementedException(); }
        }
        protected new int Size
        {
            get { throw new NotImplementedException(); }
        }

        public RuleInfoReply()
        {
        }

        internal override void SetPacket(byte[] pBytes)
        {
            int byteOffset = base.Size;

            // This will be larger if contains a valid rule.
            if (pBytes.Length > base.Size)
            {
                RuleName = Packet.GetNullTerminatedString(pBytes, byteOffset);
                byteOffset += RuleName.Length + 1;
                RuleValue = Packet.GetNullTerminatedString(pBytes, byteOffset);
                byteOffset += RuleValue.Length + 1;
            }

            base.InternalSetPacket(pBytes);
        }
    }
}
