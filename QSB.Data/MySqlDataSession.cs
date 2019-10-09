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
using QSB.Data.TableObject;
using NHibernate.Criterion;
using MySql.Data.MySqlClient;

namespace QSB.Data
{
    public class MySqlDataSession : DataSession, IDataSession
    {
        internal MySqlDataSession(ISession pSession)
        {
            if (pSession == null)
                throw new ArgumentNullException("Invalid session provided to DataSession object");

            _DbSession = pSession;
        }

        public IList<GameServer> GetServersByLastQueried(DateTime pTimeNow)
        {
            return _DbSession.CreateCriteria(typeof(GameServer))
                .Add(Expression.Sql("(NextQuery is null OR NextQuery < TIMESTAMP('" + pTimeNow.ToString("yyyy-MM-dd HH:mm:ss") + "')) AND (LastQuery is null OR LastQuery < TIMESTAMPADD(SECOND,-(QueryInterval), '" + pTimeNow.ToString("yyyy-MM-dd HH:mm:ss") + "'))")).List<GameServer>();
        }

        public override void ProcessStats(int pServerId, DateTime pDate)
        {
            _DbSession.CreateSQLQuery("call spProcessServerStats(Date('" + pDate.Date.ToString("yyyy-MM-dd") + "'), " + pServerId + ")")
                .UniqueResult();
        }
    }
}
