using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Tls;

namespace QSB.GameServerInterface.Games.QuakeEnhanced
{
    public class UdpTransport : DatagramTransport
    {
        private UdpClient _client;
        private IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        public UdpTransport(string address, int port)
        {
            _client = new UdpClient(address, port);
        }
        public void Close()
        {
            _client.Close();
        }

        public int GetReceiveLimit()
        {

            return 2048;
            throw new NotImplementedException();
        }

        public int GetSendLimit()
        {
            return 2048;
            throw new NotImplementedException();
        }

        public int Receive(byte[] buf, int off, int len, int waitMillis)
        {
            var bytes = _client.Receive(ref sender);
            
            bytes.CopyTo(buf, 0);

            return bytes.Length;
        }

        public void Send(byte[] buf, int off, int len)
        {
            _client.Send(buf, len);        }
    }
}
