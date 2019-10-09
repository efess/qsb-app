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
using QSB.Data.TableObject;

namespace QSB.Server.Test
{
    public class PlayerData
    {
        public static Player JoePlayer = new Player()
            {
                PlayerId = 1,
                AliasId = 1,
                PlayerNumber = 0,
                GameId = 0,
                IdentifyIPAddress = 0,
                Alias = "Joe",
                AliasBytes = Encoding.UTF8.GetBytes("Joe"),
                IPAddress = "private"
            };

        public static Player DavePlayer = new Player()
        {
            PlayerId = 1,
            AliasId = 1,
            PlayerNumber = 0,
            GameId = 0,
            IdentifyIPAddress = 0,
            Alias = "Dave",
            AliasBytes = Encoding.UTF8.GetBytes("Dave"),
            IPAddress = "private"
        };
              
        public static Player JanePlayer = new Player()
            {
                PlayerId = 2,
                AliasId = 2,
                PlayerNumber = 0,
                GameId = 0,
                IdentifyIPAddress = 0,
                Alias = "Jane",
                AliasBytes = Encoding.UTF8.GetBytes("Jane"),
                IPAddress = "private"
            };

        public static Player Q2Player = new Player()
            {
                PlayerId = 4,
                AliasId = 4,
                PlayerNumber = 0,
                GameId = 3,
                IdentifyIPAddress = 0,
                Alias = "Q2Player",
                AliasBytes = Encoding.UTF8.GetBytes("Q2Player"),
                IPAddress = "private"
                
            };

        public static GameServer TestServer = new GameServer()
        {
            DNS = "test.test.com",
            GameId = 0,
            Location = "RightHere",
            LastQuery = DateTime.UtcNow.AddDays(-1),
            LastQuerySuccess = DateTime.UtcNow.AddDays(-1),
            QueryInterval = 15,
            CustomName = "QSB Test Server",
            ServerId = 999,
            Region = "United States",
            ModificationCode = "DM",
            NextQuery = DateTime.UtcNow.AddDays(-1),
            Port = 10000
        };
    }
}
