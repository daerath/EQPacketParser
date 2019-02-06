using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace PacketRipper.Net
{
    public class ParsedPacket
    {
        private static readonly string _localIpv4Address = GetLocalIPAddress();

        public IPAddress Destination { get; set; }
        public byte[] Payload { get; set; }

        public string Direction
        {
            get
            {
                if (Destination.ToString() == _localIpv4Address)
                    return "Server";

                return "Client";
            }
        }

        public ParsedPacket(List<byte> payload, IPAddress destination)
        {
            Payload = payload.ToArray();
            Destination = destination;
        }

        public ParsedPacket(byte[] prefix, byte[] payload, IPAddress destination)
        {
            Payload = prefix.Concat(payload).ToArray();
            Destination = destination;
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("IPv4 address could not be found.");
        }
    }
}
