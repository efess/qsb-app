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
using QSB.Data.ViewObject;

namespace QSB.Data
{
    public class PlayerDetailSummery
    {
        public class Alias { 
            public string AliasName { get; private set;}
            public DateTime? AliasSeen { get; private set; }
            public byte[] AliasBytes { get; private set; }
            public int AliasPlayerId { get; private set; }
            public Alias(string pAlias, DateTime? pAliasSeen, byte[] pAliasBytes, int pAliasPlayerId)
            {
                AliasName = pAlias;
                AliasSeen = pAliasSeen;
                AliasBytes = pAliasBytes;
                AliasPlayerId = pAliasPlayerId;
            }
        }
        public int GameId { get; set; }
        public List<Alias> Aliases { get; set; }
        
        public string LastMap { get; set; }
        public DateTime LastSeen { get; set; }
        public string LastServer { get; set; }

        public long DayTimeSum { get; set; }
        public long DayFragsSum { get; set; }
        public double DayFPM { get; set; }
        public long WeekTimeSum { get; set; }
        public long WeekFragsSum { get; set; }
        public double WeekFPM { get; set; }
        public long MonthTimeSum { get; set; }
        public long MonthFragsSum { get; set; }
        public double MonthFPM { get; set; }
        public long YearTimeSum { get; set; }
        public long YearFragsSum { get; set; }
        public double YearFPM { get; set; }
        public long TotalTimeSum { get; set; }
        public long TotalFragsSum { get; set; }
        public double TotalFPM { get; set; }

        public PlayerDetailSummery(IList<PlayerDetail> pPlayerDetail)
        {
            Aliases = new List<Alias>();
            foreach (PlayerDetail detail in pPlayerDetail)
            {
                Aliases.Add(new Alias(detail.Alias, detail.AliasLastSeen, detail.AliasBytes, detail.AliasPlayerId));
            }
            
            if(pPlayerDetail.Count > 0)
            {
                LastMap = pPlayerDetail[0].LastMap;
                LastSeen = pPlayerDetail[0].LastSeen;
                LastServer = pPlayerDetail[0].LastServer;
                DayFragsSum = pPlayerDetail[0].day_frags_sum;
                DayTimeSum = pPlayerDetail[0].day_playtime_sum;
                DayFPM = pPlayerDetail[0].day_FPM;
                WeekFragsSum = pPlayerDetail[0].week_frags_sum;
                WeekTimeSum = pPlayerDetail[0].week_playtime_sum;
                WeekFPM = pPlayerDetail[0].week_FPM;
                MonthTimeSum = pPlayerDetail[0].month_playtime_sum;
                MonthFragsSum = pPlayerDetail[0].month_frags_sum;
                MonthFPM = pPlayerDetail[0].month_FPM;
                YearTimeSum = pPlayerDetail[0].year_playtime_sum;
                YearFragsSum = pPlayerDetail[0].year_frags_sum;
                YearFPM = pPlayerDetail[0].year_FPM;
                TotalTimeSum = pPlayerDetail[0].playtime_sum;
                TotalFragsSum = pPlayerDetail[0].frags_sum;
                TotalFPM = pPlayerDetail[0].FPM;
            }
        }
    }
}
