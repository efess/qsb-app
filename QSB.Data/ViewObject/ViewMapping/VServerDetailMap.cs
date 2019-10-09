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
    public class VServerDetailMap : ClassMap<VServerDetail>
    {
        public VServerDetailMap()
        {
            ReadOnly();
            Table("vServerDetail");

            Id(x => x.ServerDataId);
            Map(x => x.ServerId);
            Map(x => x.ServerName);
            Map(x => x.CustomName);
            Map(x => x.CustomNameShort);
            Map(x => x.CustomModificationName);
            Map(x => x.DNS);
            Map(x => x.Port);
            Map(x => x.GameId);
            Map(x => x.PublicSiteUrl);
            Map(x => x.MapDownloadUrl);
            Map(x => x.Location);
            Map(x => x.Region);
            Map(x => x.ModificationCode);
            Map(x => x.Category);
            Map(x => x.Map);
            Map(x => x.ServerSettings);
            Map(x => x.Modification);
            Map(x => x.PlayerData);
            Map(x => x.Timestamp);
            Map(x => x.MaxPlayers);
            Map(x => x.CurrentStatus);
            Map(x => x.IpAddress);
        }
    }
}
