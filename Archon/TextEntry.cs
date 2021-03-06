using System;
using System.IO;
using System.Text.Json;

namespace Archon
{
    public class TextEntry : IEntry
    {
        private string _data;
        private Timestamp _timestamp;

        private const string _jsonType = "note";

        public void AddToJsonWriter(Utf8JsonWriter jsonWriter)
        {
            jsonWriter.WriteStartObject();

            jsonWriter.WriteString("type", _jsonType);
            jsonWriter.WriteString("timestamp", _timestamp.ToString());
            jsonWriter.WriteString("data", _data);

            jsonWriter.WriteEndObject();
        }

        public override string ToString()
        {
            return $"{_timestamp} {_data}";
        }

        public TextEntry(string note, Timestamp timestamp)
        {
            _data = note;
            _timestamp = timestamp;
        }
    }
}
