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
using System.Timers;
using QSB.Data;
using QSB.Data.TableObject;
using QSB.Common;
using QSB.GameServerInterface;
using System.Threading;
using QSB.Data.ViewObject;

namespace QSB.Server
{
    public class ServerManager
    {
        /// <summary>
        /// Database Storage object
        /// </summary>
        IDataSessionFactory _dataStoreFactory = null;

        /// <summary>
        /// Main constructor to initialize members
        /// </summary>
        /// <param name="pDatabasePath">Absolute path of SQLite database file</param>
        public ServerManager(IDataSessionFactory pDataSessionFactory)
        {
            if (pDataSessionFactory == null)
                throw new ArgumentNullException("ServerManager requires a datasession");
             
            _dataStoreFactory = pDataSessionFactory;
        }

        /// <summary>
        /// Writes a bunch of historical summery report to the rollup table
        /// </summary>
        /// <param name="pDateTime">Day to process</param>
        public void SaveBatchDaysHistoricalSummery(DateTime pStartDate, DateTime pEndDate, Action<string> messageOut)
        {
            var dataSession = _dataStoreFactory.GetDataSession();

            DateTime processDate = pStartDate;
                
            while(processDate <= pEndDate)
            {
                // If this table contains a single record for this day, this day was already processed. 
                // This table will ALWAYS get processed a day at a time in a whole batch.
                if (dataSession.GetSingleHourlyLogEntry(processDate).Count > 0)
                    return;

                dataSession.BeginTransaction();
                var summeryReport = dataSession.GetHourlySummeryReport(processDate);
                if (summeryReport.Count > 0)
                {
                    foreach (PlayerSessionHourlySummery summeryRecord in summeryReport)
                    {
                        dataSession.AddHistoricalHourlyLog(summeryRecord.ConvertToHistoricalHourlyLog());
                    }
                }
                else
                {
                    // record a dummy record?
                }
                dataSession.CommitTransaction();
                if (messageOut != null)
                    messageOut("processed hourly summery for day " + processDate.ToShortDateString());
                processDate = processDate.AddDays(1);
            }
        }

        /// <summary>
        /// Writes the day's historical summery report to the rollup table
        /// </summary>
        /// <param name="pDateTime">Day to process</param>
        public void SavePreviousDaysHistoricalSummery()
        {
            var dataSession = _dataStoreFactory.GetDataSession();
            

            // Get Yesterday's date
            DateTime yesterday = DateTime.UtcNow.AddDays(-1).Date;
                
            // If this table contains a single record for this day, this day was already processed. 
            // This table will ALWAYS get processed a day at a time in a whole batch.
            if (dataSession.GetSingleHourlyLogEntry(yesterday).Count > 0)
                return;

            dataSession.BeginTransaction();
            IList<PlayerSessionHourlySummery> summeryReport = dataSession.GetHourlySummeryReport(yesterday);
            if (summeryReport.Count > 0)
            {
                foreach (PlayerSessionHourlySummery summeryRecord in summeryReport)
                {
                    dataSession.AddHistoricalHourlyLog(summeryRecord.ConvertToHistoricalHourlyLog());
                }
            }
            else
            {
                // record a dummy record?
            }

            
            dataSession.CommitTransaction();
        }


        /// <summary>
        /// Writes a bunch of historical summery report to the rollup table
        /// </summary>
        /// <param name="pDateTime">Day to process</param>
        public void SaveBatchDaysServerStats(DateTime pStartDate, DateTime pEndDate, Action<string> messageOut)
        {
            var dataSession = _dataStoreFactory.GetDataSession();

            DateTime processDate = pStartDate;
            var servers = dataSession.GetServers();

            while (processDate <= pEndDate)
            {
                dataSession.BeginTransaction();

                foreach (var server in servers)
                {
                    dataSession.ProcessStats(server.ServerId, processDate);
                }

                dataSession.CommitTransaction();
                if (messageOut != null)
                    messageOut("processed server stats for day " + processDate.ToShortDateString());
                processDate = processDate.AddDays(1);
            }
        }

        /// <summary>
        /// Writes the day's historical summery report to the rollup table
        /// </summary>
        /// <param name="pDateTime">Day to process</param>
        public void SavePreviousDaysServerStats()
        {
            var dataSession = _dataStoreFactory.GetDataSession();

            // Get Yesterday's date
            DateTime yesterday = DateTime.UtcNow.AddDays(-1).Date;
            var servers = dataSession.GetServers();
            dataSession.BeginTransaction();

            foreach (var server in servers)
            {
                dataSession.ProcessStats(server.ServerId, yesterday);
            }
                        
            dataSession.CommitTransaction();
        }

    }
}
