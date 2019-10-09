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
using System.Collections;

namespace QSB.GameServerInterface.Packets
{
       
    /// <summary>
    /// Class for handling server info response based on Quake2's
    /// 
    /// This is messy. Needs cleanup.
    /// </summary>
    public abstract class QWStatusPacketBase
    {
        private const byte SLASH_DELIMITER = 0x5c;
        private const byte NEWLINE_DELIMITER = 0x0a;

        internal Hashtable ServerSettings;
        internal virtual byte[] StatusRequest
        {
            get
            {
                // 0xFFFFFFFFstatus\n
                return new byte[] { 0xff, 0xff, 0xff, 0xff,
                    0x73, 0x74, 0x61, 0x74, 0x75, 0x73, 0x0a , 0x00 };
            }
        }

        protected int NextNewLineIndex(byte[] pBytes, int pStartIndex)
        {
            if (pStartIndex >= pBytes.Length)
                return -1;

            for (int i = pStartIndex; i < pBytes.Length; i++)
            {
                if (pBytes[i] == NEWLINE_DELIMITER)
                    return i;
            }
            return -1;
        }

        protected bool PlayerWalker(byte[] pBytes, int pOffset, out byte[] pPlayerBytes)
        {
            int i = pOffset;
            int tempOffset;
            byte[] returnByte;
            int length = 0;
            bool existsNextPlayer = true;
            do 
            {
                i++;
                if(i == pBytes.Length)
                    existsNextPlayer = false;

            }while (i < pBytes.Length && pBytes[i] != NEWLINE_DELIMITER);

            length = i - pOffset - 1;
            returnByte = new byte[length];

            for (i = 0, tempOffset = pOffset + 1; i < length; i++, tempOffset++) // i = 1; skips first character which is newline
            {
                returnByte[i] = pBytes[tempOffset];
            }
            pPlayerBytes = returnByte;

            return existsNextPlayer;
        }

        protected bool SettingWalker(byte[] pBytes, int pOffset, out int pLength)
        {
            // \key\value 
            string key = string.Empty;
            string value = string.Empty;
            bool onValue = false;
            bool existsNextSetting = false;

            StringBuilder sb = new StringBuilder();
            int i;
            for(i = pOffset + 1;;i++)
            {
                if (i >= pBytes.Length)
                    break;

                if (pBytes[i] == SLASH_DELIMITER && !onValue)
                {
                    key = sb.ToString();
                    sb = new StringBuilder();
                    onValue = true;
                    continue;
                }
                else if(pBytes[i] == SLASH_DELIMITER)
                {
                    value = sb.ToString();
                    existsNextSetting = true;
                    break;
                }
                else if (pBytes[i] == NEWLINE_DELIMITER)
                {
                    value = sb.ToString();
                    existsNextSetting = false;
                    break;
                }
                sb.Append((char)pBytes[i]);
            }
            ServerSettings.Add(key, value);
            pLength = i;
            return existsNextSetting;

        }
    }
}
