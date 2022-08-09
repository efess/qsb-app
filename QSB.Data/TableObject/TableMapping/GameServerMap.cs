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

namespace QSB.Data.TableObject.TableMapping
{
    public class GameServerMap : ClassMap<GameServer>
    {
        public GameServerMap()
        {
            Id(x => x.ServerId);
            Map(x => x.GameId);
            Map(x => x.Port);
            Map(x => x.DNS);
            Map(x => x.Location);
            Map(x => x.QueryInterval);
            Map(x => x.QueryResult);
            Map(x => x.LastQuery);
            Map(x => x.Region);
            Map(x => x.NextQuery);
            Map(x => x.LastQuerySuccess);
            Map(x => x.FailedQueryAttempts);
            Map(x => x.PublicSiteUrl);
            Map(x => x.MapDownloadUrl);
            Map(x => x.CustomName);
            Map(x => x.CustomNameShort);
            Map(x => x.ModificationCode);
            Map(x => x.Active);
            Map(x => x.ApiKey);
            Map(x => x.Parameters);
        }
    }
}

