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
using FluentNHibernate.Mapping;

namespace QSB.Data.ViewObject
{
    public class PlayerSessionHourlySummeryMap : ClassMap<PlayerSessionHourlySummery>
    {
        public PlayerSessionHourlySummeryMap()
        {
            ReadOnly();
            Table("vPlayerSessionHourlySummery");

            CompositeId()
                .KeyProperty(x => x.PlayerId)
                .KeyProperty(x => x.ServerId)
                .KeyProperty(x => x.SessionDate);
            
            Map(x => x.HourZeroSum);
            Map(x => x.HourOneSum);
            Map(x => x.HourTwoSum);
            Map(x => x.HourThreeSum);
            Map(x => x.HourFourSum);
            Map(x => x.HourFiveSum);
            Map(x => x.HourSixSum);
            Map(x => x.HourSevenSum);
            Map(x => x.HourEightSum);
            Map(x => x.HourNineSum);
            Map(x => x.HourTenSum);
            Map(x => x.HourElevenSum);
            Map(x => x.HourTwelveSum);
            Map(x => x.HourThirteenSum);
            Map(x => x.HourFourteenSum);
            Map(x => x.HourFifteenSum);
            Map(x => x.HourSixteenSum);
            Map(x => x.HourSeventeenSum);
            Map(x => x.HourEighteenSum);
            Map(x => x.HourNineteenSum);
            Map(x => x.HourTwentySum);
            Map(x => x.HourTwentyOneSum);
            Map(x => x.HourTwentyTwoSum);
            Map(x => x.HourTwentyThreeSum);
        }
    }
}
