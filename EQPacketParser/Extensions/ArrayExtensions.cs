using System;

namespace PacketRipper.Extensions
{
    public static class ArrayExtensions
    {
        public static int FindIndex(this string[] array, string criteria)
        {
            for (var i = 0; i < array.Length; i++)
            {
                if (0 == string.Compare(array[i], criteria, true))
                {
                    return i;
                }
            }

            throw new IndexOutOfRangeException($"A column named '{criteria}' could not be found.");
        }
    }
}
