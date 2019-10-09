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
using FluentNHibernate.Mapping;

namespace QSB.Data.TableObject
{
    public class HistoricalHourlyLogMap : ClassMap<HistoricalHourlyLog>
    {
        public HistoricalHourlyLogMap()
        {
            Id(x => x.HistoricalId);
            Map(x => x.ServerId);
            Map(x => x.PlayerId);
            Map(x => x.HistoricalDate);
            Map(x => x.Hour0);
            Map(x => x.Hour1);
            Map(x => x.Hour2);
            Map(x => x.Hour3);
            Map(x => x.Hour4);
            Map(x => x.Hour5);
            Map(x => x.Hour6);
            Map(x => x.Hour7);
            Map(x => x.Hour8);
            Map(x => x.Hour9);
            Map(x => x.Hour10);
            Map(x => x.Hour11);
            Map(x => x.Hour12);
            Map(x => x.Hour13);
            Map(x => x.Hour14);
            Map(x => x.Hour15);
            Map(x => x.Hour16);
            Map(x => x.Hour17);
            Map(x => x.Hour18);
            Map(x => x.Hour19);
            Map(x => x.Hour20);
            Map(x => x.Hour21);
            Map(x => x.Hour22);
            Map(x => x.Hour23);
            Map(x => x.TotalHours);
        }
    }
}
