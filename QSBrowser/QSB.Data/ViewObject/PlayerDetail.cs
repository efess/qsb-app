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

namespace QSB.Data.ViewObject
{
    public class PlayerDetail
    {
        public virtual int GameId { get; set; }
        public virtual string Alias { get; set; }
        public virtual DateTime? AliasLastSeen { get; set; }
        public virtual int AliasPlayerId { get; set; } 
        public virtual byte[] AliasBytes { get; set; }
        public virtual int PlayerId { get; set; }
        public virtual DateTime LastSeen { get; set; }
        public virtual string LastMap { get; set; }
        public virtual string LastServer { get; set; }
        public virtual long year_playtime_sum { get; set; }
        public virtual long year_frags_sum { get; set; }
        public virtual double year_FPM { get; set; }
        public virtual long month_playtime_sum { get; set; }
        public virtual long month_frags_sum { get; set; }
        public virtual double month_FPM { get; set; }
        public virtual long week_playtime_sum { get; set; }
        public virtual long week_frags_sum { get; set; }
        public virtual double week_FPM { get; set; }
        public virtual long day_playtime_sum { get; set; }
        public virtual long day_frags_sum { get; set; }
        public virtual double day_FPM { get; set; }
        public virtual long playtime_sum { get; set; }
        public virtual long frags_sum { get; set; }
        public virtual double FPM { get; set; }
    }
}
