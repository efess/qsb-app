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
using QSB.Common.Model;

namespace QSB.Data.ViewObject
{
    public class VServerDetail
    {
        public virtual int ServerId { get; set;}
        public virtual string ServerName { get; set;}
        public virtual string CustomName { get; set;}
        public virtual string CustomNameShort { get; set;}
        public virtual string CustomModificationName { get; set; }
        public virtual string DNS { get; set;}
        public virtual int Port { get; set;}
        public virtual int GameId { get; set; }
        public virtual string PublicSiteUrl { get; set;}
        public virtual string MapDownloadUrl { get; set;}
        public virtual string Location { get; set;}
        public virtual string Region { get; set;}
        public virtual string ModificationCode { get; set;}
        public virtual string Category { get; set;}
        public virtual string Map { get; set;}
        public virtual string ServerSettings { get; set;}
        public virtual string Modification { get; set;}
        public virtual string PlayerData { get; set;}
        public virtual DateTime Timestamp { get; set;}
        public virtual int MaxPlayers { get; set;}
        public virtual int CurrentStatus { get; set; }
        public virtual string IpAddress { get; set; }
        public virtual int ServerDataId { get; set; }

        public virtual ServerDetail GetServerDetail()
        {
            return new ServerDetail(ServerId,
                ServerName,
                CustomName,
                CustomNameShort,
                DNS,
                Port,
                GameId,
                PublicSiteUrl,
                MapDownloadUrl,
                Location,
                Region,
                ModificationCode,
                Category,
                Map,
                ServerSettings,
                Modification,
                Timestamp,
                MaxPlayers,
                PlayerData,
                CurrentStatus,
                IpAddress,
                CustomModificationName);
        }
    }
}
