using System;
using System.IO;
using Xunit;

namespace Archon.Tests
{
    public class WriteSessionManagerSpec
    {
        string mockInput;
        string mockOutput;

        TextWriter mockConsoleOut;
        TextReader mockConsoleIn;

        private void setUp(string input)
        {
            mockInput = input;
            mockOutput = "";

            mockConsoleOut = new StringWriter();
            mockConsoleIn = new StringReader(mockInput);
        }

        private void setUp()
        {
            setUp("");
        }

        private void tearDown()
        {
            mockConsoleOut.Close();
            mockConsoleIn.Close();
        }

        [Fact]
        public void Session_title_prompt_displays_a_message()
        {
            setUp();
            WriteSessionManager wsm = new(mockConsoleOut, mockConsoleIn);

            // Act
            wsm.PromptSessionTitle();

            // Assert
            Assert.Equal("Session title: \n", mockConsoleOut.ToString());
            tearDown();
        }
    }
}
