using System;
using System.Text;

namespace Archon
{
    public class TextEntry : IEntry
    {
        private string _data;
        private Timestamp _timestamp;

        public string ToArchonJson()
        {
            return new StringBuilder()
                .Append("{\n")
                .Append("\t\"type\": \"note\",\n")
                .Append($"\t\"timestamp\": \"{_timestamp.ToString()}\",\n")
                .Append($"\t\"data\": \"{_data}\"\n")
                .Append("}")
                .ToString();
        }

        public TextEntry(string note, Timestamp timestamp)
        {
            _data = note;
            _timestamp = timestamp;
        }
    }
}
