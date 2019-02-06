using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace PacketRipper
{
    public class OpCodeManager
    {
        public Dictionary<ushort, string> OpCodes;

        public OpCodeManager(bool isLive, string patch)
        {
            OpCodes = new Dictionary<ushort, string>();

            if (isLive)
            {
                ParseOpCodeXml($@".\OpCodes\Live\{patch}\world.xml");
                ParseOpCodeXml($@".\OpCodes\Live\{patch}\zone.xml");
            }
            else
            {
                ParseOpCodeXml($@".\OpCodes\Emu\{patch}.xml");
            }
        }

        private void ParseOpCodeXml(string fileName)
        {
            var xml = new XmlDocument();
            xml.Load(fileName);
            var nodeList = xml.SelectNodes("seqopcodes/opcode");
            foreach (XmlNode op in nodeList)
            {
                var seq = Convert.ToUInt16(op.Attributes["id"].Value, 16);
                if (OpCodes.ContainsKey(seq))
                {
                    OpCodes[seq] += $", {op.Attributes["name"].Value}";
                }
                else
                {
                    OpCodes.Add(seq, op.Attributes["name"].Value);
                }
            }
        }
    }
}
