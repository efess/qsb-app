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
using System.Threading;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;

namespace QSB.Common.TaskScheduling
{
    public delegate void DispatchMessage(string pTaskName, string pMessage);
    public delegate void DispatchException(string pTaskName, Exception pException);

    public class ScheduleDispatcher
    {
        public DispatchMessage MessageOut { get; set; }
        public DispatchException ExceptionOut { get; set; }

        private const int INTERVAL_CHECKSCHEDULES = 1000; // MS
        private DateTime _lastSchedulesCheck;
        private Thread _thread;
        private bool _stop;

        public DateTime ServerStart { get; private set; }
        private List<ScheduledTask> _scheduledtasks;

        public ScheduleDispatcher()
        {
            // Default...
            _thread = new Thread(new ThreadStart(Run));
            _scheduledtasks = new List<ScheduledTask>();
        }

        public bool IsStarted
        {
            get { return _thread.IsAlive; }
        }

        public void AddTask(ScheduledTask pTask)
        {
            pTask.MessageOut += new DispatchMessage((task, message) => PublishMessage(task, message));
            _scheduledtasks.Add(pTask);
        }

        public bool IsRunning
        {
            get
            {
                if (!_stop && IsStarted)
                    return true;
                return false;
            }
        }

        public void Start()
        {
            if (_thread == null)
                _thread = new Thread(new ThreadStart(Run));
            ServerStart = DateTime.UtcNow;

            _stop = false;
            _thread.Start();
        }

        public void Stop()
        {
            _stop = true;
            Thread.Sleep(INTERVAL_CHECKSCHEDULES * 2000);

            if (_thread.IsAlive)
                // After some time, FORCE IT.
                _thread.Abort();
        }

        public void Run()
        {
            while (!_stop)
            {
                DateTime checkDate = DateTime.Now;

                for(int i = 0; i < _scheduledtasks.Count; i++)
                {
                    ScheduledTask task = _scheduledtasks[i];

                    if (!task.IsExecuting && task.NeedsExecution())
                    {
                        try
                        {
                            new Action(() => task.Execute()).BeginInvoke((ar) =>
                            {
                                Action result = (ar as AsyncResult).AsyncDelegate as Action;
                                if (result != null)
                                    result.EndInvoke(ar);
                            }, null);
                        }
                        catch (Exception pException)
                        {
                            PublishException(task.Name, pException);
                        }
                    }
                }

                _lastSchedulesCheck = checkDate;
                Thread.Sleep(INTERVAL_CHECKSCHEDULES);
            }
        }

        private void PublishMessage(string pTaskName, string pMessage)
        {
            if (MessageOut != null)
            {
                MessageOut.Invoke(pTaskName, pMessage);
            }
        }
        private void PublishException(string pTaskName, Exception pException)
        {
            if (ExceptionOut != null)
            {
                ExceptionOut.Invoke(pTaskName, pException);
            }
        }
    }
}
