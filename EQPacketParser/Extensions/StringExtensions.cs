using System;
using System.Globalization;
using System.Text;

namespace PacketRipper.Extensions
{
    public static class StringExtensions
    {
        public static string ByteArrayToHexString(this byte[] ba)
        {
            var hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
            {
                if (hex.Length > 0)
                    hex.Append(",");

                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }

        public static byte[] HexStringToByteArray(this string hexString, bool stripCommas = true)
        {
            if (stripCommas)
            {
                // Strip commas.
                hexString = hexString.Replace(",", "");
            }

            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException($"The binary key cannot have an odd number of digits: {hexString}");
            }

            var data = new byte[hexString.Length / 2];
            for (int index = 0; index < data.Length; index++)
            {
                string byteValue = hexString.Substring(index * 2, 2);
                data[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return data;
        }
    }
}
