using System;
using System.Text;

namespace Archon
{
    public class TextEntry : IEntry
    {
        private string data;
        private Timestamp timestamp;

        public string SerializeToArchonJSON()
        {
            return new StringBuilder()
                .Append("{")
                .Append("\"type\":\"note\", ")
                .Append($"\"timestamp\":\"{timestamp.ToString()}\", ")
                .Append($"\"data\":\"{data}\"")
                .Append("}")
                .ToString();
        }

        public TextEntry(string note, Timestamp timestamp)
        {
            this.data = note;
            this.timestamp = timestamp;
        }
    }
}
