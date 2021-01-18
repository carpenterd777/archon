using System;
using Xunit;

namespace Archon.Tests
{
    public class TimestampSpec
    {
        // This is really just my preference, and for a more robust program in the future
        // this should be customizable
        // this may fail for cultures outside of en-US.
        [Fact]
        public void To_string_uses_AM_and_PM()
        {
            // Act
            Timestamp ts1 = new(new DateTime(1989, 12, 13, 16, 20, 0, 0));
            Timestamp ts2 = new(new DateTime(1989, 12, 13, 4, 20, 0, 0));

            // Assert
            Assert.Equal("[4:20 PM]", ts1.ToString());
            Assert.Equal("[4:20 AM]", ts2.ToString());
        }
    }
}
