using System;
using System.IO;
using Xunit;
using Archon;

namespace Archon.Tests
{
    public class WriteSessionManagerSpec
    {
        [Fact]
        public void Session_title_prompt_displays_a_message()
        {
            TextWriter stdoutWriter = new StreamWriter(Console.OpenStandardOutput());
            TextReader stdoutReader = new StreamReader(Console.OpenStandardOutput());
            WriteSessionManager wsm = new(stdoutWriter);

            // Act
            wsm.PromptSessionTitle();

            // Assert
            Assert.Equal("Session title: ",  stdoutReader.ReadLine());
        }
    }
}
