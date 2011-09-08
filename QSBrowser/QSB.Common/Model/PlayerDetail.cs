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
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace QSB.Common.Model
{
    public class PlayerDetail
    {
        /// <summary>
        /// Text friendly name of Player
        /// </summary>
        public string Name { get; private set;}
        /// <summary>
        /// Raw byte representation of Player's name
        /// </summary>
        public byte[] NameBytes { get; private set;}
        /// <summary>
        /// How long Player has been on the server
        /// </summary>
        public TimeSpan UpTime {get; private set;}
        /// <summary>
        /// Current frag count of Player
        /// </summary>
        public int CurrentFrags { get; private set; }
        /// <summary>
        /// Total frags earned by Player while Connected
        /// </summary>
        public int TotalFrags { get; private set;}
        /// <summary>
        /// How many frags per minute earned by player
        /// </summary>
        public double FragsPerMinute { get; private set;}
        /// <summary>
        /// Player's Unique Identifier
        /// </summary>
        public int PlayerId { get; private set;}

        /// <summary>
        /// If supported, Player's shirt color
        /// </summary>
        public int ShirtColor { get; private set;}
        /// <summary>
        /// If supported, Player's pant color
        /// </summary>
        public int PantColor { get; private set;}
        /// <summary>
        /// If supported, current skin used by player
        /// </summary>
        public string Skin { get; private set;}
        /// <summary>
        /// If supported, current model used by player
        /// </summary>
        public string Model { get; private set;}

        public PlayerDetail(XElement pPlayerXml)
        {
            if (pPlayerXml.Name == "Player")
                foreach (var xElement in pPlayerXml.Elements())
                    SetField(xElement.Name.ToString(), xElement.Value);
        }

        private void SetField(string pField, string pValue)
        {
            switch (pField.ToLower())
            {
                case "playerid":
                    PlayerId = int.Parse(pValue);
                    break;
                case "namebase64":
                    NameBytes = Convert.FromBase64String(pValue);
                    break;
                case "name":
                    Name = pValue;
                    break;
                case "uptime":
                    {
                        TimeSpan lol;
                        TimeSpan.TryParse(pValue, out lol);
                        UpTime = lol;
                    }
                    break;
                case "currentfrags":
                    {
                        int lol = 0;
                        int.TryParse(pValue, out lol);
                        CurrentFrags = lol;
                    }
                    break;
                case "totalfrags":
                    {
                        int lol = 0;
                        int.TryParse(pValue, out lol);
                        TotalFrags = lol;
                    }
                    break;
                case "fragsperminute":
                    {
                        double lol = 0;
                        double.TryParse(pValue, out lol);
                        FragsPerMinute = lol;
                    }
                    break;
                case "shirt":
                    {
                        int lol = 0;
                        int.TryParse(pValue, out lol);
                        ShirtColor = lol;
                    }
                    break;
                case "pant":
                    {
                        int lol = 0;
                        int.TryParse(pValue, out lol);
                        PantColor = lol;
                    }
                    break;
                case "model":
                    Model = pValue;
                    break;
                case "skin":
                    Skin = pValue;
                    break;
            }
        }
    }
}