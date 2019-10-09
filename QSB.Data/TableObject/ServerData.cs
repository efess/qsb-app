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
using NHibernate;

namespace QSB.Data.TableObject
{
    public class ServerData
    {
        public virtual int ServerDataId { get; set; }
        public virtual int ServerId { get; set;}
        public virtual DateTime TimeStamp { get; set;}
        public virtual string Map { get; set;}
        public virtual string Modification { get; set;}
        public virtual string PlayerData { get; set; }
        public virtual string Name { get; set; }
        public virtual string IpAddress { get; set; }
        public virtual int MaxPlayers { get; set; }
        public virtual string ServerSettings { get; set; }
    }
}
