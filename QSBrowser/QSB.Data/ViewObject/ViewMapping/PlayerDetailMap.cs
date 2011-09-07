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
using FluentNHibernate.Mapping;


namespace QSB.Data.ViewObject.TableMapping
{
    public class PlayerDetailMap : ClassMap<PlayerDetail>
    {
        public PlayerDetailMap()
        {
            Table("vPlayerDetail");
            ReadOnly();
            Id(x => x.Alias);
            Map(x => x.GameId);
            Map(x => x.PlayerId);
            Map(x => x.AliasLastSeen);
            Map(x => x.AliasPlayerId);
            Map(x => x.AliasBytes);
            Map(x => x.LastSeen);
            Map(x => x.LastMap);
            Map(x => x.LastServer);
            Map(x => x.year_playtime_sum);
            Map(x => x.year_frags_sum);
            Map(x => x.year_FPM);
            Map(x => x.month_playtime_sum);
            Map(x => x.month_frags_sum);
            Map(x => x.month_FPM);
            Map(x => x.week_playtime_sum);
            Map(x => x.week_frags_sum);
            Map(x => x.week_FPM);
            Map(x => x.day_playtime_sum);
            Map(x => x.day_frags_sum);
            Map(x => x.day_FPM);
            Map(x => x.playtime_sum);
            Map(x => x.frags_sum);
            Map(x => x.FPM);
        }
    }
}
