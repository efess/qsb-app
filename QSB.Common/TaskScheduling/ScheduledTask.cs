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
using System.Collections.Generic;
using System;

namespace QSB.Common.TaskScheduling
{
    public delegate void Task();

    public class ScheduledTask
    {
        public string Name { get; private set; }
        public Object TaskObject { get; private set; }
        public Task TaskMethod { get; private set; }
        public List<Schedule> Schedules { get; private set; }
        public bool IsExecuting { get; private set; }

        public DispatchMessage MessageOut { get; set; }

        // Default constructor
        public ScheduledTask(Task pTaskMethod, string pName, Schedule pInitialSchedule)
        {
            Name = pName;
            Schedules = new List<Schedule>();
            Schedules.Add(pInitialSchedule);

            TaskMethod = pTaskMethod;
        }

        public ScheduledTask(Object pTaskObject, Task pTaskMethod, string pName, Schedule pInitialSchedule)
            : this(pTaskMethod, pName, pInitialSchedule)
        {
            TaskObject = pTaskObject;
        }

        public bool NeedsExecution()
        {
            for (int i = 0; i < Schedules.Count; i++) 
                if (Schedules[i].NeedsExecution()) 
                    return true; 

            return false;
        }

        public void Execute()
        {
            if (IsExecuting) return;
            IsExecuting = true;

            for (int i = 0; i < Schedules.Count; i++)
                Schedules[i].LastRun = DateTime.UtcNow;

            try
            {
                TaskMethod.Invoke();
                PublishMessage("Execution complete");  
            }
            catch(Exception ex)
            {
                PublishMessage("Error encountered executing: " + Name + Environment.NewLine + "Exception Detail: " + ex.ToString());  
            }
            finally
            {
                IsExecuting = false;
            }
         
        }

        private void PublishMessage(string pMessage)
        {
            if (MessageOut != null)
            {
                MessageOut.Invoke(this.Name, pMessage);
            }
        }
    }
}
