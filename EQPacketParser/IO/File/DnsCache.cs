using System.Collections.Generic;
using System.Net;

namespace PacketRipper.IO.File
{
    public class DnsCache
    {
        private static Dictionary<string, IPAddress> Cache = new Dictionary<string, IPAddress>();

        public static IPAddress Get(string value)
        {
            if (!Cache.ContainsKey(value))
            {
                if (value == "0.0.0.0")
                {
                    Cache.Add(value, IPAddress.Parse(value));
                }
                else
                {
                    Cache.Add(value, Dns.GetHostAddresses(value)[0]);
                }
            }

            return Cache[value];
        }
    }
}
