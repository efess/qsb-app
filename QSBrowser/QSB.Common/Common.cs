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
using System.Xml;
using System.IO;
using System.Net;

namespace QSB.Common
{
    /// <summary>
    /// Enum for describing the Status of a Game Server
    /// </summary>
    public enum ServerStatus
    {
        Running,
        NotResponding,
        NotFound,
        QueryError
    }

    /// <summary>
    /// Enum of all supported Games
    /// </summary>
    public enum Game
    {
        NetQuake,
        QuakeWorld,
        Quake2,
        Quake3,
        Quake4
    }


    /// <summary>
    /// Popular world game regions
    /// </summary>
    public enum Region
    {
        UnitedStates,
        Brazil,
        Europe
    }

    public static class Common
    {
        public static double UnixNow
        {
            get
            {
                return DateTime.UtcNow.UnixTime();
            }
        }
    }

    /// <summary>
    /// This is a hack for now.
    /// </summary>
    public static class GlobalOptions
    {
    }

    public static class XmlHelper
    {
        /// <summary>
        /// Creates an XML node
        /// </summary>
        public static XmlNode GetElementNode(XmlDocument pXd, string pNodeName, string pNodeText)
        {
            XmlNode node = pXd.CreateNode(XmlNodeType.Element, pNodeName, pXd.NamespaceURI);
            node.InnerText = pNodeText;

            return node;
        }
    }

    public static class WebHelper
    {
        public static void PingServer()
        {
            WebClient http = new WebClient();
            string Result = http.DownloadString("http://servers.quakeone.com");
        }
    }

    public static class Streamhelper
    {
        private static int BufferSize = 4096;
        public static byte[] ReadAllBytesFromStream(Stream pStream)
        {
            int bytesRead = 0;
            int totalBytes = 0;
            byte[] buffer = new byte[BufferSize];
            MemoryStream tempStream = new MemoryStream();

            do
            {
                bytesRead = Math.Min(((int)pStream.Length) - totalBytes, BufferSize);

                bytesRead = pStream.Read(buffer, totalBytes, bytesRead);
                tempStream.Write(buffer,totalBytes,bytesRead);
                totalBytes += bytesRead;

            } while (bytesRead > 0 && totalBytes < pStream.Length);

            byte[] returnBytes = new byte[totalBytes];

            // read stream from the memory stream to the array
            tempStream.Read(returnBytes, 0, totalBytes);
            return returnBytes;
        }

        public static string ReadAllBytesFromStreamToString(Stream pStream)
        {
            return Encoding.ASCII.GetString(ReadAllBytesFromStream(pStream));
        }

        public static void WriteAllBytesToStream(Stream pStream, byte[] pBytes)
        {
            int offset = 0;
            int length = pBytes.Length;
            int byteCount = 0;

            while (offset < length)
            {
                byteCount = Math.Min(length - offset, BufferSize);

                pStream.Write(pBytes, offset, byteCount);

                offset += byteCount;
            }
        }

        public static void WriteStringToStream(Stream pStream, string pText)
        {
            WriteAllBytesToStream(pStream, Encoding.ASCII.GetBytes(pText));
        }
    }

    public static class StringHelper
    {
        /// <summary>
        /// Returns if string resembles an IP Address (to support Partial IP addresses)
        /// </summary>
        /// <param name="pString"></param>
        /// <returns></returns>
        public static bool ReasonableIPAddress(string pString)
        {
            //Allowed:
            // 19.293.xxx.xxx
            // 19.293.234.xxx
            // 19.124.491.343
            string[] str = pString.Split(new char[] { '.' });
            if (str.Length == 4)
            {
                for (int i = 0; i < 2; i++)
                {
                    int testInt = -1;
                    if (!int.TryParse(str[i], out testInt))
                    {
                        return false;
                    }

                }
                return true;
            }

            return false;
        }
    
        /// <summary>
        /// Returns if IP address is a valid IP Address
        /// </summary>
        /// <param name="pString"></param>
        /// <returns></returns>
        public static bool IsIPAddress(string pString)
        {
            string[] str = pString.Split(new char[] { '.' });
            if (str.Length == 4)
            {
                for (int i = 0; i < 4; i++)
                {
                    int testInt = -1;
                    if (!int.TryParse(str[i], out testInt)
                        || (testInt < 0 && testInt > 255))
                        return false;
                }
                return true;
            }

            return false;
        }
    } 
}