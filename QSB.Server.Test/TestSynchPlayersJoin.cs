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
using NUnit.Framework;
using Moq;
using QSB.Data;
using QSB.Data.TableObject;
using QSB.GameServerInterface;
using QSB.Common;

namespace QSB.Server.Test
{
    [TestFixture]
    public class TestSynchPlayersJoin
    {
        private ServerQueryController _queryController;
        private FakeServerInterface _fakeServerInterface;
        private Mock<IDataSessionFactory> fakeDataSessionFactory;

        [SetUp]
        public void Setup()
        {
            var fakeDataSession = new Mock<IDataSession>();

            fakeDataSessionFactory = new Mock<IDataSessionFactory>();
            fakeDataSessionFactory
                .Setup(fakeDsF => fakeDsF.GetDataSession())
                .Returns(fakeDataSession.Object);

            fakeDataSession.Setup(fakeDs => fakeDs.GetOrCreatePlayer(
                It.Is<string>(str => str == PlayerData.JoePlayer.Alias),
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<int>()))
                .Returns(PlayerData.JoePlayer);

            fakeDataSession.Setup(fakeDs => fakeDs.GetOrCreatePlayer(
                It.Is<string>(str => str == PlayerData.JanePlayer.Alias),
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<int>()))
                .Returns(PlayerData.JanePlayer);


            fakeDataSession.Setup(fakeDs => fakeDs.GetOrCreatePlayer(
                It.Is<string>(str => str == PlayerData.DavePlayer.Alias),
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<int>()))
                .Returns(PlayerData.DavePlayer);

            fakeDataSession.Setup(fakeDs => fakeDs.GetInSessionPlayersByServer(It.IsAny<int>())).Returns(new List<PlayerSession>());
            _fakeServerInterface = new FakeServerInterface();
            _queryController = new ServerQueryController(fakeDataSessionFactory.Object, _fakeServerInterface);

        }

        [Test]
        public void SimpleJoin()
        {
            ServerSnapshot snapshot = new ServerSnapshot();
            snapshot.CurrentMap = "dm3";
            snapshot.IpAddress = "127.0.0.1";
            snapshot.Mod = "DM";

            // Add Joe 
            snapshot.Players.Add(new PlayerSnapshot(
                    PlayerData.JoePlayer.AliasBytes,
                    PlayerData.JoePlayer.IPAddress,
                    PlayerData.JoePlayer.Alias,
                    0)
                {
                    Frags = 3,
                    SkinName = "",
                    ModelName = ""                    
                });           

            snapshot.ServerName = "FakeServer";
            snapshot.Status = ServerStatus.Running;
            
            _fakeServerInterface.CurrentServerState = snapshot;
            _queryController.DoQuery(PlayerData.TestServer);

            ServerActivity activity = ServerCache.Cache[PlayerData.TestServer.ServerId];
            Assert.AreEqual(1, activity.PlayerActivities.Count);
            PlayerActivity playerActivity = activity.PlayerActivities[0];

            Assert.AreEqual(3, playerActivity.TotalFrags);
            Assert.IsNotNull(playerActivity.Session);
            Assert.AreEqual(playerActivity.Session.PlayerId, PlayerData.JoePlayer.PlayerId);

            // Add Jane and do query
            snapshot.Players.Add(new PlayerSnapshot(
                PlayerData.JanePlayer.AliasBytes,
                PlayerData.JanePlayer.IPAddress,
                PlayerData.JanePlayer.Alias,
                0)
            {
                Frags = 3,
                SkinName = "",
                ModelName = "" 
            });

            _queryController.DoQuery(PlayerData.TestServer);

            Assert.AreEqual(2, activity.PlayerActivities.Count);
        }

        [Test]
        public void PlayersLeaving()
        {

            ServerSnapshot snapshot = new ServerSnapshot();
            snapshot.CurrentMap = "dm3";
            snapshot.IpAddress = "127.0.0.1";
            snapshot.Mod = "DM";
            
            PlayerSnapshot jane; 
            PlayerSnapshot joe;
            // Add Joe 
            snapshot.Players.Add(joe = new PlayerSnapshot(
                    PlayerData.JoePlayer.AliasBytes,
                    PlayerData.JoePlayer.IPAddress,
                    PlayerData.JoePlayer.Alias,
                    0)
            {
                Frags = 3,
                SkinName = "",
                ModelName = ""
            });

            // Add Jane and do query
            snapshot.Players.Add(jane = new PlayerSnapshot(
                PlayerData.JanePlayer.AliasBytes,
                PlayerData.JanePlayer.IPAddress,
                PlayerData.JanePlayer.Alias,
                0)
            {
                Frags = 3,
                SkinName = "",
                ModelName = ""
            });

            _fakeServerInterface.CurrentServerState = snapshot;
            _queryController.DoQuery(PlayerData.TestServer);

            // remove jane, set joe's frags to 4
            joe.Frags = 4;
            snapshot.Players.Remove(jane);

            _queryController.DoQuery(PlayerData.TestServer);

            ServerActivity activity = ServerCache.Cache[PlayerData.TestServer.ServerId];
            Assert.AreEqual(1, activity.PlayerActivities.Count);
            PlayerActivity playerActivity = activity.PlayerActivities[0];
            Assert.AreEqual(4, playerActivity.Session.CurrentFrags);
        }

        //private void InitializeQueryController()
        //{

        //    fakeDataSession.Setup(fakeDs => fakeDs.GetOrCreatePlayer(
        //        "Jane",
        //        It.IsAny<string>(),
        //        Encoding.UTF8.GetBytes("Jane"),
        //        0))
        //        .Returns(PlayerData.JanePlayer);

        //    fakeDataSession.Setup(fakeDs => fakeDs.GetOrCreatePlayer(
        //        "Dave",
        //        It.IsAny<string>(),
        //        Encoding.UTF8.GetBytes("Dave"),
        //        0))
        //        .Returns(PlayerData.DavePlayer);
        //}
    }
}
