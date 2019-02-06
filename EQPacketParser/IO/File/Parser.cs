using PacketRipper.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PacketRipper.IO.File
{
    public class Parser
    {
        public async Task<List<Row>> Parse(string fileName)
        {
            var line = "";
            var rows = new List<Row>();

            using (var streamReader = new System.IO.StreamReader(fileName))
            {
                // Read the first row and identify the column indexes we want.
                if ((line = await streamReader.ReadLineAsync().ConfigureAwait(false)) == null)
                    throw new System.IO.InvalidDataException("The first row must contain column names.");

                // Split the line.
                var parts = line.Split(Row.Delimiter, StringSplitOptions.RemoveEmptyEntries);

                // Look in the header row to find the columns we want. This will be the first row.
                Row.SourceIndex = parts.FindIndex(nameof(Row.Source));
                Row.DestinationIndex = parts.FindIndex(nameof(Row.Destination));
                Row.PayloadIndex = parts.FindIndex(nameof(Row.Payload));

                while ((line = await streamReader.ReadLineAsync().ConfigureAwait(false)) != null)
                {
                    rows.Add(new Row(line));
                }
            }

            return rows;
        }
    }
}
