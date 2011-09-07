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

namespace QSB.Common.TaskScheduling
{
    public class TimeOfDaySchedule : Schedule
    {
        public TimeSpan ExecuteTimeOfDay { get; private set; }

        public TimeOfDaySchedule(TimeSpan pTimeOfDay)
        {
            LastRun = DateTime.UtcNow.AddDays(-1);
            ExecuteTimeOfDay = pTimeOfDay;
        }

        public override bool NeedsExecution()
        {
            DateTime checkDate = DateTime.UtcNow;

            if (LastRun.Date < checkDate.Date
                && checkDate.TimeOfDay > ExecuteTimeOfDay)
            {
                return true;
            }

            return false;
        }
    }
}
