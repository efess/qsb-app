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

namespace QSB.Data.ViewObject.ViewMapping
{
    public class PlayerMatchesMap : ClassMap<PlayerMatches>
    {
        public PlayerMatchesMap()
        {
            Table("vPlayerMatches");
            ReadOnly();
            Id(x => x.MatchId);
            Map(x => x.GameId);
            Map(x => x.MatchStart);
            Map(x => x.HostName);
            Map(x => x.Port);
            Map(x => x.ServerName);
            Map(x => x.ServerId);
            Map(x => x.Map);
            Map(x => x.Modification);
            Map(x => x.Alias);
            Map(x => x.PlayerId);
            Map(x => x.Frags);
            Map(x => x.PantColor);
            Map(x => x.ShirtColor);
            Map(x => x.Skin);
            Map(x => x.Model);
            Map(x => x.PlayerJoinTime);
            Map(x => x.PlayerStayDuration);

        }
    }
}
