using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Serilog.Sinks.Netty
{
    public sealed class SerilogNettyClientOptions
    {
        public int BatchSize { get; set; } = 1000;

        public int PeriodMicroseconds { get; set; } = 500;

        public string Remote { get; set; }

        public int RemotePort { get; set; } = 6253;

        private IPEndPoint _remoteEndPoint;

        internal IPEndPoint RemoteEndPoint
        {
            get
            {
                if (_remoteEndPoint == null)
                {
                    var ip = Dns.GetHostAddresses(Remote)
                        .FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
                    _remoteEndPoint = new IPEndPoint(ip, RemotePort);
                }

                return _remoteEndPoint;
            }
        }
    }
}
