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

namespace QSB.Data.ViewObject
{
    public class PlayerMatches
    {
        public virtual int MatchId { get; set; }
        public virtual int GameId { get; set; }
        public virtual DateTime? MatchStart { get; set; }
        public virtual string HostName { get; set; }
        public virtual int Port { get; set; }
        public virtual string ServerName { get; set; }
        public virtual int ServerId { get; set; }
        public virtual string Map { get; set; }
        public virtual string Modification { get; set; }
        public virtual string Alias { get; set; }
        public virtual int PlayerId { get; set; }
        public virtual int Frags { get; set; }
        public virtual int PantColor { get; set; }
        public virtual int ShirtColor { get; set; }
        public virtual string Skin { get; set; }
        public virtual string Model { get; set; }
        public virtual DateTime? PlayerJoinTime { get; set; }
        public virtual int PlayerStayDuration { get; set; }
    }
}
