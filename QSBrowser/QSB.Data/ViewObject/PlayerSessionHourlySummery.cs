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
using QSB.Data.TableObject;

namespace QSB.Data.ViewObject
{
    public class PlayerSessionHourlySummery
    {
        public virtual DateTime SessionDate { get; set; }
        public virtual int PlayerId { get; set; }
        public virtual int ServerId { get; set; }
        public virtual int HourZeroSum { get; set; }
        public virtual int HourOneSum { get; set; }
        public virtual int HourTwoSum { get; set; }
        public virtual int HourThreeSum { get; set; }
        public virtual int HourFourSum { get; set; }
        public virtual int HourFiveSum { get; set; }
        public virtual int HourSixSum { get; set; }
        public virtual int HourSevenSum { get; set; }
        public virtual int HourEightSum { get; set; }
        public virtual int HourNineSum { get; set; }
        public virtual int HourTenSum { get; set; }
        public virtual int HourElevenSum { get; set; }
        public virtual int HourTwelveSum { get; set; }
        public virtual int HourThirteenSum { get; set; }
        public virtual int HourFourteenSum { get; set; }
        public virtual int HourFifteenSum { get; set; }
        public virtual int HourSixteenSum { get; set; }
        public virtual int HourSeventeenSum { get; set; }
        public virtual int HourEighteenSum { get; set; }
        public virtual int HourNineteenSum { get; set; }
        public virtual int HourTwentySum { get; set; }
        public virtual int HourTwentyOneSum { get; set; }
        public virtual int HourTwentyTwoSum { get; set; }
        public virtual int HourTwentyThreeSum { get; set; }
        
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public virtual HistoricalHourlyLog ConvertToHistoricalHourlyLog()
        {
            HistoricalHourlyLog hourlyLog = new HistoricalHourlyLog();
            hourlyLog.PlayerId = PlayerId;
            hourlyLog.ServerId = ServerId;
            hourlyLog.Hour0 = HourZeroSum;
            hourlyLog.Hour1 = HourOneSum;
            hourlyLog.Hour2 = HourTwoSum;
            hourlyLog.Hour3 = HourThreeSum;
            hourlyLog.Hour4 = HourFourSum;
            hourlyLog.Hour5 = HourFiveSum;
            hourlyLog.Hour6 = HourSixSum;
            hourlyLog.Hour7 = HourSevenSum;
            hourlyLog.Hour8 = HourEightSum;
            hourlyLog.Hour9 = HourNineSum;
            hourlyLog.Hour10 = HourTenSum;
            hourlyLog.Hour11 = HourElevenSum;
            hourlyLog.Hour12 = HourTwelveSum;
            hourlyLog.Hour13 = HourThirteenSum;
            hourlyLog.Hour14 = HourFourteenSum;
            hourlyLog.Hour15 = HourFifteenSum;
            hourlyLog.Hour16 = HourSixteenSum;
            hourlyLog.Hour17 = HourSeventeenSum;
            hourlyLog.Hour18 = HourEighteenSum;
            hourlyLog.Hour19 = HourNineteenSum;
            hourlyLog.Hour20 = HourTwentySum;
            hourlyLog.Hour21 = HourTwentyOneSum;
            hourlyLog.Hour22 = HourTwentyTwoSum;
            hourlyLog.Hour23 = HourTwentyThreeSum;
            hourlyLog.HistoricalDate = SessionDate;
            hourlyLog.TotalHours = HourZeroSum
                + HourOneSum
                + HourTwoSum
                + HourThreeSum
                + HourFourSum
                + HourFiveSum
                + HourSixSum
                + HourSevenSum
                + HourEightSum
                + HourNineSum
                + HourTenSum
                + HourElevenSum
                + HourTwelveSum
                + HourThirteenSum
                + HourFourteenSum
                + HourFifteenSum
                + HourSixteenSum
                + HourSeventeenSum
                + HourEighteenSum
                + HourNineteenSum
                + HourTwentySum
                + HourTwentyOneSum
                + HourTwentyTwoSum
                + HourTwentyThreeSum; // phew.

            return hourlyLog;
        }
    }
}

