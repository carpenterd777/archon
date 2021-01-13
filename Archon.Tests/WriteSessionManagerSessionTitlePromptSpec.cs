using System;
using System.IO;
using Xunit;

namespace Archon.Tests
{
    public class WriteSessionManagerSessionTitlePromptSpec
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
            setUp("The Assault on House Debauch");
            WriteSessionManager wsm = new(mockConsoleOut, mockConsoleIn);

            // Act
            wsm.PromptSessionTitle();

            // Assert
            Assert.Equal(MessageStrings.SESSION_TITLE_PROMPT + "\n", mockConsoleOut.ToString());
            tearDown();
        }

        [Fact]
        public void Session_title_prompt_accepts_a_string()
        {
            setUp("The Assault on House Debauch");
            WriteSessionManager wsm = new(mockConsoleOut, mockConsoleIn);

            // Act
            string providedTitle = wsm.PromptSessionTitle();

            // Assert
            Assert.Equal("The Assault on House Debauch", providedTitle);
            tearDown();
        }

        [Fact]
        public void Session_title_prompt_follows_up_if_single_int_input()
        {
            setUp("99\ny");
            WriteSessionManager wsm = new(mockConsoleOut, mockConsoleIn);

            // Act
            wsm.PromptSessionTitle();

            string allOutput = mockConsoleOut.ToString();
            string? followUp = allOutput.Split("\n")[1]; // first one is "Session title: "

            Assert.Equal(MessageStrings.SESSION_TITLE_INT_INPUT, followUp);
            tearDown();
        }

        [Fact]
        public void Session_title_no_follow_up_if_string_input()
        {
            setUp("The Assault on House Debauch");
            WriteSessionManager wsm = new(mockConsoleOut, mockConsoleIn);

            // Act
            wsm.PromptSessionTitle();

            string allOutput = mockConsoleOut.ToString();
            string followUp = allOutput.Split("\n")[1];

            Assert.Equal("", followUp);
            tearDown();
        }

        [Fact]
        public void Session_title_prompt_reprompts_if_int_and_user_discards_input()
        {
            setUp("99\nn\nGinnungagap time"); // User puts 99 as title, marks "n" and did not mean it
            WriteSessionManager wsm = new(mockConsoleOut, mockConsoleIn);

            // Act
            string userTitle = wsm.PromptSessionTitle();

            string allOutput = mockConsoleOut.ToString();
            string reprompt = allOutput.Split("\n")[2];

            Assert.Equal(MessageStrings.SESSION_TITLE_PROMPT, reprompt);
            Assert.Equal("Ginnungagap time", userTitle);
            tearDown();
        }
    }
}
