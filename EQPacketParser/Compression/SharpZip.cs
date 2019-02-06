using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System;
using System.IO;

namespace PacketRipper.Compression
{
    public class SharpZip
    {
        public const byte CompressedFlag = 0x5a;
        public const byte UncompressedFlag = 0xa5;

        public static bool Inflate(ref byte[] compressedPacket)
        {
            try
            {
                // Do we have a compression indicator or not?
                var outBuffer = default(byte[]);
                if (compressedPacket.Length > 1 && compressedPacket[0] == CompressedFlag)
                {
                    // This packet is compressed.  Skip over the compression indicator and decompress the
                    // remaining data.
                    using (var ms = new MemoryStream(compressedPacket, 1, compressedPacket.Length - 1))
                    {
                        var inflater = new Inflater(false);
                        var inStream = new InflaterInputStream(ms, inflater);

                        // Create the temporary buffer.
                        var tmpBuffer = new byte[2048];

                        // Copy the decompressed data.
                        var decompressedLength = inStream.Read(tmpBuffer, 0, tmpBuffer.Length);

                        // Now copy it to the final buffer.
                        outBuffer = new byte[decompressedLength];
                        Buffer.BlockCopy(tmpBuffer, 0, outBuffer, 0, decompressedLength);
                    }
                }
                else if (compressedPacket.Length > 1 && compressedPacket[0] == UncompressedFlag)
                {
                    // This packet is not compressed.  Just remove the compression indicator.
                    outBuffer = new byte[compressedPacket.Length - 1];
                    Buffer.BlockCopy(compressedPacket, 1, outBuffer, 0, compressedPacket.Length - 1);
                }
                else
                {
                    // This packet doesn't have a compression indicator.  Don't do anything to it.
                    outBuffer = compressedPacket;
                }

                compressedPacket = outBuffer;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
