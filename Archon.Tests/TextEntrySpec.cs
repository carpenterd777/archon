using System;
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
                "{\n\t\"type\": \"note\",\n\t\"timestamp\": \"[5:33 PM]\",\n\t\"data\": \"Lonqu looked for trouble\"\n}"; 
            string actual = entry.ToArchonJson();

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
