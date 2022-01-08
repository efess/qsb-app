using Org.BouncyCastle.Tls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using Org.BouncyCastle.Security;
using QSB.GameServerInterface.Dtls;

namespace QSB.GameServerInterface.Games.QuakeEnhanced
{
    //  = new byte[] { 0x7a, 0x55, 0xb9, 0x80, 0xc4, 0x9b, 0x9a, 0xe4, 0xd4, 0xff, 0xdb, 0x35, 0x13, 0xc2, 0xe4, 0x71, 0xd9, 0x4b, 0x14, 0xc2, 0x20, 0x77, 0xed, 0x86 };
    public class DtlsUtility
    {
        private byte[] _psk;
        private byte[] _pskId;
        private DatagramTransport dtlsTransport;

        public DtlsUtility(byte[] psk, byte[] pskId, string serverAddress, int port)
        {
            _psk = psk;
            _pskId = pskId;

            var client = new PskDtlsClient(new BasicTlsPskIdentity(_pskId, _psk));
            var transport = new UdpTransport(serverAddress, port);

            DtlsClientProtocol dtls = new DtlsClientProtocol();
            dtlsTransport = dtls.Connect(client, transport);
        }

        public void Send(byte[] tosend)
        {
            dtlsTransport.Send(tosend, 0, tosend.Length);
        }
        public byte[] SendReceive(byte[] tosend)
        {
            dtlsTransport.Send(tosend, 0, tosend.Length);
            return Receive();
        }

        public byte[] Receive()
        {
            var buffer = new byte[2048];
            var received = dtlsTransport.Receive(buffer, 0, 2048, 3000);
            if (received > 0)
            {
                return buffer.Take(received).ToArray();
            }
            return new byte[] { };
        }
    }
}
