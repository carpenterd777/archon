using System;
using System.IO;
using System.Text.Json;
using Archon;
using Xunit;

namespace Archon.Tests
{
    public class TextEntrySpec
    {
        [Fact]
        public void Serialization_has_type_timestamp_data()
        {
            string note = "Lonqu looked for trouble";
            Timestamp ts = new(new DateTime(2015, 4, 12, 17, 33, 0, 0));
            TextEntry entry = new(note, ts);

            // Act
            string expected = 
                "{\n  \"type\": \"note\",\n  \"timestamp\": \"[5:33 PM]\",\n  \"data\": \"Lonqu looked for trouble\"\n}"; 

            MemoryStream stream = new();
            using (Utf8JsonWriter writer = ArchonJsonWriterFactory.CreateArchonJsonWriter(stream))
            {
                entry.AddToJsonWriter(writer);
            }

            string actual;
            stream.Position = 0;
            using (StreamReader reader = new(stream))
            {
                actual = reader.ReadToEnd();
            }

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
