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
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;

namespace QSB.Data
{
    public class DataBridge
    {
        public enum DatabaseEngine
        {
            SQLite,
            MySql
        }

        private static ISessionFactory _sessionFactory;

        public static DatabaseEngine CurrentEngine { get; private set; }

        public static IDataSessionFactory InitializeDatabase(string pConnectionString, DatabaseEngine pEngine)
        {
            CurrentEngine = pEngine;

            switch (CurrentEngine)
            {
                case DatabaseEngine.SQLite:
                    _sessionFactory = Fluently.Configure()
                    .Database(
                        SQLiteConfiguration.Standard.ConnectionString(pConnectionString)
                    )
                    .Mappings(m =>
                        m.FluentMappings.AddFromAssemblyOf<DataBridge>())
                    .BuildSessionFactory();
                    break;

                case DatabaseEngine.MySql:
                        _sessionFactory = Fluently.Configure()
                        .ExposeConfiguration(c => c.Properties.Add("hbm2ddl.keywords", "none"))
                        .Database(
                            MySQLConfiguration.Standard.ConnectionString(pConnectionString)
                        )
                        .Mappings(m =>
                            m.FluentMappings.AddFromAssemblyOf<DataBridge>())
                        .ExposeConfiguration(cfg => new SchemaUpdate(cfg).Execute(false, true))
                        .BuildSessionFactory();
              
                    break;
            }

            return new DataSessionFactory(_sessionFactory, CurrentEngine);
        }

    }
}
