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

namespace QSB.Data.TableObject
{
    public class HistoricalHourlyLog
    {
        public virtual int HistoricalId { get; set; }
        public virtual int ServerId { get; set; }
        public virtual int PlayerId { get; set; }
        public virtual DateTime HistoricalDate { get; set; }
        public virtual int Hour0 { get; set; }
        public virtual int Hour1 { get; set; }
        public virtual int Hour2 { get; set; }
        public virtual int Hour3 { get; set; }
        public virtual int Hour4 { get; set; }
        public virtual int Hour5 { get; set; }
        public virtual int Hour6 { get; set; }
        public virtual int Hour7 { get; set; }
        public virtual int Hour8 { get; set; }
        public virtual int Hour9 { get; set; }
        public virtual int Hour10 { get; set; }
        public virtual int Hour11 { get; set; }
        public virtual int Hour12 { get; set; }
        public virtual int Hour13 { get; set; }
        public virtual int Hour14 { get; set; }
        public virtual int Hour15 { get; set; }
        public virtual int Hour16 { get; set; }
        public virtual int Hour17 { get; set; }
        public virtual int Hour18 { get; set; }
        public virtual int Hour19 { get; set; }
        public virtual int Hour20 { get; set; }
        public virtual int Hour21 { get; set; }
        public virtual int Hour22 { get; set; }
        public virtual int Hour23 { get; set; }
        public virtual int TotalHours { get; set; }
    }
}
