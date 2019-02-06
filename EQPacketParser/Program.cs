using PacketRipper.Extensions;
using PacketRipper.Net;
using PacketRipper.OpCodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PacketRipper
{
    class Program
    {
        private static List<ParsedPacket> totalPackets = new List<ParsedPacket>();
        private static Dictionary<ushort, byte[]> frags = new Dictionary<ushort, byte[]>();
        private static int fragsLength = 0;
        private static int fragsMaxLength = 0;

        private static void ProcessPacket(byte[] prefix, byte[] payload, IPAddress destination)
        {
            if (prefix[0] == NetworkOpCodes.OP_Sixty)
            {
                // ignore these for now.
                return;
            }
            else if (prefix[1] == NetworkOpCodes.OP_Oversized)
            {
                // Get the sequence
                var sequence = (ushort)IPAddress.HostToNetworkOrder((short)BitConverter.ToUInt16(payload, 0));

                // Cut the sequence
                payload = payload.Skip(2).ToArray();

                // if we have no fragments, get the size.
                if (0 == fragsMaxLength)
                {
                    fragsMaxLength = IPAddress.HostToNetworkOrder(BitConverter.ToInt32(payload, 0));
                    // cut the size.
                    payload = payload.Skip(4).ToArray();
                }

                // store the packet based on sequence #, then trim the sequence # from the data.
                frags.Add(sequence, payload);

                fragsLength += payload.Length;

                // Do we have the whole thing?
                if (fragsLength >= fragsMaxLength)
                {
                    // We have it all. Combine.
                    var total = new List<byte>();
                    foreach (var pp in frags.OrderBy(a => a.Key))
                    {
                        total.AddRange(pp.Value);
                    }
                    totalPackets.Add(new ParsedPacket(total, destination));
                    fragsLength = 0;
                    fragsMaxLength = 0;
                    frags.Clear();
                }
            }
            else if (prefix[1] == NetworkOpCodes.OP_Packet)
            {
                // store the full packet.
                totalPackets.Add(new ParsedPacket(prefix, payload, destination));
            }
            else if (prefix[1] == NetworkOpCodes.OP_Ack)
            {
                // store the full packet.
                totalPackets.Add(new ParsedPacket(prefix, payload, destination));
            }
            else if (prefix[1] == NetworkOpCodes.OP_Combined)
            {
                // Drop the last 2 bytes. we don't care about the CRC.
                payload = payload.Take(payload.Length - 2).ToArray();

                // Get the size.
                var combinedSize = payload.Length;
                var runner = 0;
                // Unwrap
                while (runner < combinedSize)
                {
                    var pSize = payload[runner];
                    runner++;
                    var tmpPacket = payload.Skip(runner).Take(pSize).ToArray();
                    if (tmpPacket[1] != NetworkOpCodes.OP_Ack)
                    {
                        var pre = tmpPacket.Take(2).ToArray();
                        var pay = tmpPacket.Skip(2).ToArray();

                        ProcessPacket(pre, pay, destination);
                    }

                    runner += pSize;
                }
            }
        }

        private static OpCodeManager ParseArguments(string[] args)
        {
            if (args.Length != 2)
                return null;

            var target = args[1].Split(':');

            switch (target[0])
            {
                case "/l":
                    return new OpCodeManager(true, target[1]);
                case "/e":
                    return new OpCodeManager(false, target[1]);
                default:
                    return null;
            }
        }

        static async Task Main(string[] args)
        {
            var opCodeManager = default(OpCodeManager);

            if (null == (opCodeManager = ParseArguments(args)))
            {
                Console.WriteLine(@"
Usage: PacketRipper.exe <file to parse> </l:patchdate|/e:expansion>
    /l:date         - Use live opcodes for the specified date
    /e:expansion    - Use emu opcodes from the specified expansion

Examples:

    EQPacketParser.exe ..\..\captures\mycapture.txt /e:RoF2
    EQPacketParser.exe ..\..\captures\mycapture.txt /l:20190118
");
                return;
            }

            // Parse the file.
            var fileContents = await new IO.File.Parser().Parse(args[0]);

            foreach (var row in fileContents)
            {
                var prefix = row.Payload.Take(2).ToArray();
                var payload = row.Payload.Skip(2).ToArray();
                var result = Compression.SharpZip.Inflate(ref payload);

                ProcessPacket(prefix, payload, row.Destination);
            }

            var i = -1;
            foreach (var packet in totalPackets)
            {
                i++;
                if (packet.Payload.Length <= 4) continue;// || packet.Payload[0] == 252 || packet.Payload[1] == 21) continue;

                // If this is an application packet (that came from a combined packet) it will start with an opcode.
                var opCodeStartPosition = 0;
                if (packet.Payload[0] == 0)
                {
                    opCodeStartPosition = 4;
                }

                opCodeManager.OpCodes.TryGetValue(BitConverter.ToUInt16(packet.Payload, opCodeStartPosition), out string opCodeName);

                // Ignore these for the moment.
                if (opCodeName == "OP_FloatListThing") continue;

                Console.WriteLine($@"
Idx: {i}
Size:{packet.Payload.Length}
From: {packet.Direction}
OpCode: {opCodeName}::{packet.Payload.Skip(opCodeStartPosition).Take(2).ToArray().ByteArrayToHexString()}
Bytes: {string.Join(",", packet.Payload)}
Hex: {packet.Payload.ByteArrayToHexString()}
String: {System.Text.Encoding.ASCII.GetString(packet.Payload)}
-------------------------------------------------------------------------------");
            }
        }
    }
}
