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
using NHibernate;

namespace QSB.Data.TableObject
{
    public class PlayerMatch
    {
        public virtual int PlayerMatchId { get; set; }
        public virtual int ServerMatchId { get; set; }
        public virtual int PlayerId { get; set; }
        public virtual int AliasId { get; set; }
        public virtual int PantColor { get; set; }
        public virtual int ShirtColor { get; set; }
        public virtual string Model { get; set; }
        public virtual string Skin { get; set; }
        public virtual int Latency { get; set; }
        public virtual int Frags { get; set; }
        public virtual DateTime? PlayerMatchStart { get; set; }
        public virtual DateTime? PlayerMatchEnd { get; set; }
    }
}
