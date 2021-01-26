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
        
        public string ToArchonJson()
        {
            MemoryStream stream = new();

            using (Utf8JsonWriter jsonWriter = createArchonJsonWriter(stream))
            {
                jsonWriter.WriteStartObject();

                jsonWriter.WriteString("type", _jsonType);
                jsonWriter.WriteString("timestamp", _timestamp.ToString());
                jsonWriter.WriteString("data", _data);

                jsonWriter.WriteEndObject();
            }

            // Read from the beginning of what's written to the stream
            stream.Position = 0;
            string json;
            using (StreamReader sr = new(stream))
            {
                json = sr.ReadToEnd();
            }
            
            return json;
        }

        private Utf8JsonWriter createArchonJsonWriter(Stream stream) => 
            ArchonJsonWriterFactory.CreateArchonJsonWriter(stream);

        public TextEntry(string note, Timestamp timestamp)
        {
            _data = note;
            _timestamp = timestamp;
        }
    }
}
