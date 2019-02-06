using PacketRipper.Extensions;
using System.Net;

namespace PacketRipper.IO.File
{
    public class Row
    {
        public static char[] Delimiter = new char[] { '\t' };

        public IPAddress Source { get; set; }
        public IPAddress Destination { get; set; }
        public byte[] Payload { get; set; }
        public string PayloadHex { get; set; }

        public static int SourceIndex { get; set; }
        public static int DestinationIndex { get; set; }
        public static int PayloadIndex { get; set; }

        public Row(string text)
        {
            // Split on tabs.
            var parts = text.Split(Delimiter);

            Source = DnsCache.Get(parts[SourceIndex]);
            Destination = DnsCache.Get(parts[DestinationIndex]);

            // The payload contains commas, so it gets wrapped by quotes. Remove those before we convert.
            var payloadRaw = parts[PayloadIndex].Replace("\"", "");
            Payload = payloadRaw.HexStringToByteArray();
            PayloadHex = payloadRaw;
        }
    }
}
