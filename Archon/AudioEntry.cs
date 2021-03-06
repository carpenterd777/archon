using System;
using System.IO;
using System.Text.Json;

namespace Archon
{
    public class AudioEntry : IEntry
    {
        private string _data;         // the filepath to the audio file
        private Timestamp _timestamp; // the timestamp when recording started

        private const string _jsonType = "recording"; // the value the "type" field will have in the Json serialization
        
        public void AddToJsonWriter(Utf8JsonWriter jsonWriter)
        {
            jsonWriter.WriteStartObject();

            jsonWriter.WriteString("type", _jsonType);
            jsonWriter.WriteString("timestamp", _timestamp.ToString());
            jsonWriter.WriteString("data", _data);

            jsonWriter.WriteEndObject();
        } 

        public override string ToString(){
            return $"{_timestamp} {_data}";
        }

        public AudioEntry(string filepath, Timestamp timestamp)
        {
            _data = filepath;
            _timestamp = timestamp;
        }
    }
}
