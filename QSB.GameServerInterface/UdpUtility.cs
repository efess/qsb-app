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
using System.Net.Sockets;
using System.Net;

namespace QSB.GameServerInterface
{
    internal class UdpUtility : INetCommunicate
    {
        private string _address;
        private int _port;

        private int _receiveTimeOut = 3000; // 3 seconds
        private int _sendTimeOut = 3000;
        private UdpClient _client;

        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);

        internal UdpUtility(string pAddress, int pPort)
        {
            _address = pAddress;
            _port = pPort;

            _client = new UdpClient(_address, _port);
            _client.Client.ReceiveTimeout = _receiveTimeOut;
            _client.Client.SendTimeout = _sendTimeOut;
        }

        public byte[] SendBytes(byte[] pSendBytes)
        {
            int i = _client.Send(pSendBytes, pSendBytes.Length);
            byte[] receiveBytes = _client.Receive(ref sender);
            return receiveBytes;            
        }

        public string RemoteIpAddress
        {
            get { return ((IPEndPoint)_client.Client.RemoteEndPoint).Address.ToString(); }
        }

        public int RemotePort
        {
            get { return ((IPEndPoint)_client.Client.RemoteEndPoint).Port; }
        }
    }
}
