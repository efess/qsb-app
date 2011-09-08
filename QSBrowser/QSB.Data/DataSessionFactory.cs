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

namespace QSB.Data
{
    /// <summary>
    /// Wrapper around NHibernate's session factory in order to open a new session, and instantiate correct DataSession object
    /// </summary>
    public class DataSessionFactory : IDataSessionFactory
    {
        ISessionFactory _sessionFactory;
        DataBridge.DatabaseEngine _databaseEngine;

        public DataSessionFactory(ISessionFactory pSessionFactory, DataBridge.DatabaseEngine pDatabaseEngine)
        {
            _sessionFactory = pSessionFactory;
            _databaseEngine = pDatabaseEngine;
        }

        public IDataSession GetDataSession()
        {
            switch (_databaseEngine)
            {
                case DataBridge.DatabaseEngine.MySql:
                    return new MySqlDataSession(_sessionFactory.OpenSession());
                case DataBridge.DatabaseEngine.SQLite:
                    return new SQLiteDataSession(_sessionFactory.OpenSession());
                default:
                    throw new Exception("Database Engine not supported");
            }            
        }
    }
}
