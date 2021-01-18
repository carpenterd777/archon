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
                @"{""type"":""note"", ""timestamp"":""[5:33 PM]"", ""data"":""Lonqu looked for trouble""}"; 
            string actual = entry.SerializeToArchonJSON();

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
