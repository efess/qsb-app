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
using QSB.Common.Model;
using System.Xml.Linq;

namespace QSB.Common.Test.Model
{
    [TestFixture]
    public class PlayerDetailTests
    {
        [Test]
        public void ProcessPlayerDataTests()
        {
//            string testPlayersData = @"<Players><Player>
//                    <PlayerId>1</PlayerId>
//                    <Name>foo</Name>
//                    <UpTime>1.23.45.12</UpTime>
//                    <CurrentFrags>4</CurrentFrags>
//                    <TotalFrags>10</TotalFrags>
//                    <FragsPerMinute>0.34</FragsPerMinute>
//                    <Model>bar</Model>
//                    <Skin>baz</Skin>
//                </Player>
//                <Player>
//                    <PlayerId>2</PlayerId>
//                    <Name>foo2</Name>
//                    <UpTime>1.23.45.12</UpTime>
//                    <CurrentFrags>5</CurrentFrags>
//                    <TotalFrags>11</TotalFrags>
//                    <FragsPerMinute>0.54</FragsPerMinute>
//                    <Model>bar2</Model>
//                    <Skin>baz2</Skin>
//                </Player>
//            </Players>";

            byte[] testByte= new byte[3]{0x24, 0x34, 0x83};

            string testPlayerData = string.Format(@"<Player>
                    <PlayerId>1</PlayerId>
                    <Name>foo</Name>
                    <NameBase64>{0}</NameBase64>
                    <UpTime>1.23:45:12</UpTime>
                    <CurrentFrags>4</CurrentFrags>
                    <TotalFrags>10</TotalFrags>
                    <FragsPerMinute>0.34</FragsPerMinute>
                    <Pant>3</Pant>
                    <Shirt>6</Shirt>
                    <Model>bar</Model>
                    <Skin>baz</Skin>
                </Player>", Convert.ToBase64String(testByte));

            var xElement = XElement.Parse(testPlayerData);
            var detail = new PlayerDetail(xElement);

            Assert.AreEqual(1, detail.PlayerId);
            Assert.AreEqual("foo", detail.Name);
            Assert.AreEqual(testByte, detail.NameBytes);
            Assert.AreEqual(new TimeSpan(1,23,45, 12), detail.UpTime);
            Assert.AreEqual(4, detail.CurrentFrags);
            Assert.AreEqual(10, detail.TotalFrags);
            Assert.AreEqual(.34d, detail.FragsPerMinute);
            Assert.AreEqual(3, detail.PantColor);
            Assert.AreEqual(6, detail.ShirtColor);
            Assert.AreEqual("bar", detail.Model);
            Assert.AreEqual("baz", detail.Skin);
        }
    }
}
