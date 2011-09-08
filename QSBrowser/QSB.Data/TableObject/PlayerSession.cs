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
using QSB.Common;

namespace QSB.Data.TableObject
{
    public class PlayerSession
    {
        public virtual int SessionId { get; set;}
        public virtual int ServerId {get; set;}
        public virtual int PlayerId {get; set;}
        public virtual string Map {get; set;}
        public virtual DateTime? SessionStart {get; set;}
        public virtual DateTime? SessionEnd {get; set;}
        public virtual int FragCount {get; set;}
        public virtual string ShirtColor {get; set;}
        public virtual string PantColor {get; set;}
        public virtual int Latency { get; set; }
        public virtual int LastAliasId { get; set; }
        public virtual int CurrentFrags { get; set; }
        public virtual DateTime? SessionDate { get; set; }

        public virtual Player LastAlias { get; set; }
    }
}
