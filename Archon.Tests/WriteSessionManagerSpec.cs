using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using FluentAssertions;
using Xunit;

namespace Archon.Tests
{
    public class WriteSessionManagerSpec
    {
        string mockInput;

        TextWriter mockConsoleOut;
        TextReader mockConsoleIn;

        private void setUp(string input)
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
            mockInput = input;

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

        // Session title prompt tests

        [Fact]
        public void Session_title_prompt_displays_a_message()
        {
            setUp("The Assault on House Debauch\n");
            WriteSessionManager wsm = new(mockConsoleOut, mockConsoleIn);

            // Act
            wsm.PromptSessionTitle();
            List<string> outLines = new List<string>(mockConsoleOut.ToString().Split("\n"));

            // Assert
            outLines.Contains(MessageStrings.SESSION_TITLE_PROMPT).Should().BeTrue("because that is the prompt message");
            tearDown();
        }

        [Fact]
        public void Session_title_prompt_accepts_a_string()
        {
            string input = "The Assault on House Debauch";
            setUp(input + "\n");
            WriteSessionManager wsm = new(mockConsoleOut, mockConsoleIn);

            // Act
            string providedTitle = wsm.PromptSessionTitle();

            // Assert
            providedTitle.Should().Be(input, "because that was the provided title");
            wsm.SessionTitle.Should().Be(input, "because that was the provided title");
            tearDown();
        }

        [Fact]
        public void Session_title_prompt_follows_up_if_single_int_input()
        {
            setUp("99\ny\n");
            WriteSessionManager wsm = new(mockConsoleOut, mockConsoleIn);

            // Act
            wsm.PromptSessionTitle();
            List<string> outLines = new List<string>(mockConsoleOut.ToString().Split("\n"));

            // Assert
            outLines.Contains(MessageStrings.SESSION_TITLE_INT_INPUT).Should().BeTrue("because the user may have confused the session title prompt for the session number prompt");
            tearDown();
        }

        [Fact]
        public void Session_title_no_follow_up_if_string_input()
        {
            setUp("The Assault on House Debauch\n");
            WriteSessionManager wsm = new(mockConsoleOut, mockConsoleIn);

            // Act
            wsm.PromptSessionTitle();

            string allOutput = mockConsoleOut.ToString();
            List<string> outLines = new List<string>(allOutput.Split("\n"));

            // Assert
            outLines.Contains("").Should().BeTrue("because the user likely did not confuse the session title prompt with the session number prompt");
            tearDown();
        }

        [Fact]
        public void Session_title_prompt_reprompts_if_int_and_user_discards_input()
        {
            setUp("99\nn\nGinnungagap time\n"); // User puts 99 as title, marks "n" and did not mean it
            WriteSessionManager wsm = new(mockConsoleOut, mockConsoleIn);

            // Act
            string providedTitle = wsm.PromptSessionTitle();

            string allOutput = mockConsoleOut.ToString();
            List<string> outLines = new List<string>(allOutput.Split("\n"));

            // Assert
            outLines.Contains(MessageStrings.SESSION_TITLE_PROMPT).Should().BeTrue("the user indicated that they put in the wrong session title");
            tearDown();
        }

        [Fact]
        public void Session_title_prompt_accepts_lack_of_input()
        {
            setUp("\n");

            WriteSessionManager wsm = new(mockConsoleOut, mockConsoleIn);

            // Act
            string userInput = wsm.PromptSessionTitle();

            //Assert
            userInput.Should().Be("", "because the user did not enter anything for the session title");
        }


        // Session number tests

        [Fact]
        public void Session_number_prompt_displays_a_message()
        {
            setUp("1\n");

            WriteSessionManager wsm = new(mockConsoleOut, mockConsoleIn);

            // Act
            wsm.PromptSessionNumber();
            List<string> outLines = new List<string>(mockConsoleOut.ToString().Split("\n"));

            // Assert
            outLines.Contains(MessageStrings.SESSION_NUMBER_PROMPT).Should().BeTrue("because that is the session number prompt");
            tearDown();
        }

        [Fact]
        public void Session_number_prompt_reprompts_if_non_int_given()
        {
            setUp("Not a number\n99");

            WriteSessionManager wsm = new(mockConsoleOut, mockConsoleIn);

            // Act
            wsm.PromptSessionNumber();
            List<string> outLines = new List<string>(mockConsoleOut.ToString().Split("\n"));

            // Assert
            outLines.Contains(MessageStrings.SESSION_NUMBER_INVALID_INPUT).Should().BeTrue("because the user did not pass in a number");
            tearDown();
        }

        [Fact]
        public void Session_number_prompt_returns_passed_session_number()
        {
            setUp("23\n");

            WriteSessionManager wsm = new(mockConsoleOut, mockConsoleIn);

            // Act
            int sessionNumber = wsm.PromptSessionNumber();

            // Assert
            sessionNumber.Should().Be(23, "because that was the passed in session number");
            wsm.SessionNumber.Should().Be(23, "because that was the passed in session number");
            tearDown();
        }

        [Fact]
        public void Session_number_prompt_reprompts_if_negative()
        {
            setUp("-1\n1\n"); // User enters invalid int, then valid int

            WriteSessionManager wsm = new(mockConsoleOut, mockConsoleIn);

            // Act
            wsm.PromptSessionNumber();
            List<string> outLines = new List<string>(mockConsoleOut.ToString().Split("\n"));

            outLines.Contains(MessageStrings.SESSION_NUMBER_INVALID_INPUT).Should().BeTrue("because the user entered an invalid int");
            tearDown();
        }

        // Create Json tests

        [Fact]
        public void Create_Json_creates_json()
        {
            setUp();

            WriteSessionManager wsm = new(mockConsoleOut, mockConsoleIn, new DateTime(2015, 4, 12, 17, 33, 0, 0));

            //Act
            string json = wsm.CreateJson();
            string expected = "{\n  \"title\": \"\",\n  \"session\": \"\",\n  \"date\": \"4/12/2015\",\n  \"entries\": []\n}";

            json.Should().Be(expected, "because that is the form of a JSON entry");
            tearDown();

        }

        // File creation tests

        [Fact]
        public void Creates_snake_case_filename_from_session_title()
        {
            setUp();

            WriteSessionManager wsm = new(mockConsoleOut, mockConsoleIn);
            wsm.SessionTitle = "Wings for Ginnungagap";
            wsm.SaveEntries();

            File.Exists("./wings_for_ginnungagap.archon.json").Should().BeTrue();

            File.Delete("./wings_for_ginnungagap.archon.json");
            tearDown();
        }

        [Fact]
        public void Removes_period_from_filename()
        {
            setUp();

            WriteSessionManager wsm = new(mockConsoleOut, mockConsoleIn);
            wsm.SessionTitle = "Wings. For Ginnungagap";
            wsm.SaveEntries();

            File.Exists("./wings_for_ginnungagap.archon.json").Should().BeTrue();

            File.Delete("./wings_for_ginnungagap.archon.json");
            tearDown();
        }

        // Note rendering tests

        [Fact]
        public void Renders_note()
        {
            setUp();

            WriteSessionManager wsm = new(mockConsoleOut, mockConsoleIn);

            // this cannot be tested if not being run in a console
            if (Console.BufferWidth > 1)
            {
                wsm.DispatchWriteSessionAction("a note", new DateTime(2015, 4, 12, 17, 33, 0, 0));

                List<string> outLines = new List<string>(mockConsoleOut.ToString().Split("\n"));

                outLines.Contains("[5:33 PM] a note").Should().BeTrue("because the user did not type in a specific command");
                tearDown();
            }
        }

        [Fact]
        public void Renders_multiline_note()
        {
            setUp();

            WriteSessionManager wsm = new(mockConsoleOut, mockConsoleIn);

            // this cannot be tested if not being run in a console
            if (Console.BufferWidth > 1)
            {
                string multilineNote = "";
                for (int i = 0; i < Console.BufferWidth + 1; i++)
                {
                    multilineNote += "a";
                }

                wsm.DispatchWriteSessionAction(multilineNote, new DateTime(2015, 4, 12, 17, 33, 0, 0));

                List<string> outLines = new List<string>(mockConsoleOut.ToString().Split("\n"));

                outLines.Contains($"[5:33 PM] {multilineNote}").Should().BeTrue("because the general command was more than one line long");
                tearDown();
            }
        }
    }
}
