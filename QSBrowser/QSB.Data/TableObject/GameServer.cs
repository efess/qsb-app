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
using NHibernate;

namespace QSB.Data.TableObject
{
    public class GameServer
    {
        public virtual int ServerId {get; set;}
        public virtual int GameId {get; set;}
        public virtual bool AntiWallHack { get; set; }
        public virtual int Port { get; set; }
        public virtual string DNS { get; set;}
        public virtual string Region { get; set; }
        /// <summary>
        /// Physical location of server
        /// </summary>
        public virtual string Location {get; set;}
        /// <summary>
        /// Seconds between each query interval (Minimum of 10 seconds)
        /// </summary>
        public virtual int QueryInterval { get; set; }
        public virtual int FailedQueryAttempts { get; set; }
        public virtual DateTime? LastQuery { get; set; }
        public virtual int QueryResult { get; set; }
        public virtual DateTime? NextQuery { get; set; }
        /// <summary>
        /// Last time a server was online or successfully queried
        /// </summary>
        public virtual DateTime? LastQuerySuccess { get; set; }
        
        public virtual string MapDownloadUrl { get; set; }
        public virtual string PublicSiteUrl { get; set; }
        public virtual string ModificationCode { get; set; }
        public virtual string CustomName { get; set; }
        public virtual string CustomNameShort { get; set; }

        public virtual bool Active { get; set; }

        public override string ToString()
        {
            return DNS + ":" + Port;
        }

    }
}
