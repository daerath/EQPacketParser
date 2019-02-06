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
                ParseLiveOpCodeFile($@".\OpCodes\Live\{patch}\world.xml");
                ParseLiveOpCodeFile($@".\OpCodes\Live\{patch}\zone.xml");
            }
            else
            {
                ParseEmuOpCodeFile(patch);
            }
        }

        private void ParseEmuOpCodeFile(string patchName)
        {
            OpCodes = new Dictionary<ushort, string>();

            var lines = File.ReadAllLines($@".\OpCodes\Emu\{patchName}.txt");
            foreach (var r in lines)
            {
                var parts = r.Split(new char[] { '=' });
                OpCodes.Add(Convert.ToUInt16(parts[1], 16), parts[0]);
            }
        }

        private void ParseLiveOpCodeFile(string fileName)
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
