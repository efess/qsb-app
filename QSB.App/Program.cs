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
using System.Configuration;
using QSB.Data;
using System.Reflection;
using QSB.Common;
using QSB.Common.TaskScheduling;

namespace QSB.App
{
    class Program
    {
        static Status debugOutput;
        static ScheduleDispatcher programDispatcher;

        static void Main(string[] args)
        {            
            string connectionString = Environment.GetEnvironmentVariable("DB_CONN_STRING") ??
                ConfigurationSettings.AppSettings["ConnectionString"] as string;
            string stringDatabaseEngine = Environment.GetEnvironmentVariable("DB_ENGINE") ??
                ConfigurationSettings.AppSettings["DatabaseEngine"] as string;

            QSB.Data.DataBridge.DatabaseEngine databaseEngine = DataBridge.DatabaseEngine.SQLite;
            debugOutput = new Status(1000,
                System.IO.Path.Combine(
                    System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    "debug"));
            try
            {
                databaseEngine = (DataBridge.DatabaseEngine)Enum.Parse(typeof(DataBridge.DatabaseEngine), stringDatabaseEngine);
            }
            catch
            {
                if (string.IsNullOrEmpty(stringDatabaseEngine))
                {
                    WriteMessage("No DatabaseEngine key/value entry in Config.");
                }
                else
                {
                    WriteMessage("Invalid DatabaseEngine specified in Config: " + stringDatabaseEngine);
                }
                string acceptableValues = "Acceptable DatabaseEngine values are: ";
                foreach (string str in Enum.GetNames(typeof(QSB.Data.DataBridge.DatabaseEngine)))
                {
                    acceptableValues += "[" + str + "]";
                }
                WriteMessage(acceptableValues);
                return;
            }


            if (string.IsNullOrEmpty(connectionString))
            {
                WriteMessage("No ConnectionString key/value entry in Config.");
                return;
            }

            // Set Paths

            IDataSessionFactory dataSessionFactory = null;
            try
            {
                dataSessionFactory = DataBridge.InitializeDatabase(connectionString, databaseEngine);
            }
            catch (Exception ex)
            {
                WriteMessage("Error initializing database: " + ex.ToString());
                return;
            }

            WriteMessage("Initialized " + databaseEngine + " database");


            var argsCollection = new List<List<string>>();
            List<string> tempSubArgsCollection = null;

            for(int i = 0; i < args.Length; i++)
            {
                if(args[i].StartsWith("-") && args[i].Length > 1)
                {
                    tempSubArgsCollection = new List<string>();
                    argsCollection.Add(tempSubArgsCollection);

                    tempSubArgsCollection.Add(args[i].Remove(0,1));
                }
                else
                {
                    if(tempSubArgsCollection != null)
                        tempSubArgsCollection.Add(args[i]);
                }
            }

            // Process any command line arguments
            foreach(var consolidatedArgs in argsCollection)
            {
                DateTime startDate;
                DateTime endDate;
                switch(consolidatedArgs[0].ToUpper())
                {
                    case "SB":
                    case "SUMMERYBATCH":
                        if(consolidatedArgs.Count != 3)
                            Console.WriteLine("Invalid number of args for SummeryBatch");
                            if(DateTime.TryParse(consolidatedArgs[1], out startDate))
                                if(DateTime.TryParse(consolidatedArgs[2], out endDate))
                                {
                                    DoSummeryBatch(dataSessionFactory, startDate, endDate);
                                    return;
                                }
                            Console.WriteLine("Bad dates for SummeryBatch");
                        return;

                    case "STATS":
                    case "PS":
                    case "PROCESSSTATS":
                        if (consolidatedArgs.Count != 3)
                            Console.WriteLine("Invalid number of args for SummeryBatch");
                        
                        if (DateTime.TryParse(consolidatedArgs[1], out startDate))
                            if (DateTime.TryParse(consolidatedArgs[2], out endDate))
                            {
                                DoProcessStats(dataSessionFactory, startDate, endDate);
                                return;
                            }
                        Console.WriteLine("Bad dates for SummeryBatch");
                        return;
                    default:
                        break;
                }
            }

            StartScheduler(dataSessionFactory);
        }

        private static void DoSummeryBatch(IDataSessionFactory pDataSessionFactory, DateTime startDate, DateTime endDate)
        {
            // For Reporting and Rollup tables - scheduled task(s)
            var serverManager = new QSB.Server.ServerManager(pDataSessionFactory);
            serverManager.SaveBatchDaysHistoricalSummery(startDate, endDate, WriteMessage);
        }

        private static void DoProcessStats(IDataSessionFactory pDataSessionFactory, DateTime startDate, DateTime endDate)
        {
            // For Reporting and Rollup tables - scheduled task(s)
            var serverManager = new QSB.Server.ServerManager(pDataSessionFactory);
            serverManager.SaveBatchDaysServerStats(startDate, endDate, WriteMessage);
        }

        private static void WriteMessage(string pMessage)
        {
            Console.WriteLine("[" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "] " + pMessage);
            debugOutput.AddStatus(pMessage);
        }

        private static void StartScheduler(IDataSessionFactory pDataSessionFactory)
        {
            programDispatcher = new ScheduleDispatcher();

            programDispatcher.ExceptionOut += new DispatchException(
                (task, ex) =>
                {
                    WriteMessage("Task " + task + " threw an Exception: " + ex.Message + "\r\n" + ex.StackTrace);
                }
            );

            programDispatcher.MessageOut += new DispatchMessage(
                (task, message) =>
                {
                    WriteMessage("Task " + task + ": " + message);
                }
            );

            // Add Queries schedule task 
            QSB.Server.ServerQueryController queryController = new QSB.Server.ServerQueryController(
                pDataSessionFactory
                , new QSB.GameServerInterface.ServerInterface()
                );

            ScheduledTask scheduledTask = new ScheduledTask(
                queryController
                , new Task(queryController.DoQueries)
                , "Query Thread"
                , new IntervalSchedule(3));
            programDispatcher.AddTask(scheduledTask);

            // For Reporting and Rollup tables - scheduled task(s)
            QSB.Server.ServerManager serverManager = new QSB.Server.ServerManager(pDataSessionFactory);

            scheduledTask = new ScheduledTask(
                serverManager
                , new QSB.Common.TaskScheduling.Task(serverManager.SavePreviousDaysHistoricalSummery)
                , "HistoricalHourlySummery"
                , new QSB.Common.TaskScheduling.TimeOfDaySchedule(new TimeSpan(0, 1, 0)));
            programDispatcher.AddTask(scheduledTask);

            //scheduledTask = new ScheduledTask(
            //    serverManager
            //    , new QSB.Common.TaskScheduling.Task(serverManager.SavePreviousDaysServerStats)
            //    , "ProcessServerStatistics"
            //    , new QSB.Common.TaskScheduling.TimeOfDaySchedule(new TimeSpan(0, 1, 0)));
            //programDispatcher.AddTask(scheduledTask);

            WriteMessage("Starting task dispatcher");

            programDispatcher.Start();
        }
    }
}
