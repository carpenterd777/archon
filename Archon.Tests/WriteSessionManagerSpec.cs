using System;
using System.IO;
using System.Globalization;
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

            // Assert
            Assert.Equal(MessageStrings.SESSION_TITLE_PROMPT + "\n", mockConsoleOut.ToString());
            tearDown();
        }

        [Fact]
        public void Session_title_prompt_accepts_a_string()
        {
            setUp("The Assault on House Debauch\n");
            WriteSessionManager wsm = new(mockConsoleOut, mockConsoleIn);

            // Act
            string providedTitle = wsm.PromptSessionTitle();

            // Assert
            Assert.Equal("The Assault on House Debauch", providedTitle);
            Assert.Equal("The Assault on House Debauch", wsm.SessionTitle);
            tearDown();
        }

        [Fact]
        public void Session_title_prompt_follows_up_if_single_int_input()
        {
            setUp("99\ny\n");
            WriteSessionManager wsm = new(mockConsoleOut, mockConsoleIn);

            // Act
            wsm.PromptSessionTitle();

            string allOutput = mockConsoleOut.ToString();
            var followUp = allOutput.Split("\n")[1]; // first one is "Session title: "

            // Assert
            Assert.Equal(MessageStrings.SESSION_TITLE_INT_INPUT, followUp);
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
            string followUp = allOutput.Split("\n")[1];

            // Assert
            Assert.Equal("", followUp);
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
            string reprompt = allOutput.Split("\n")[2];
            
            // Assert
            Assert.Equal(MessageStrings.SESSION_TITLE_PROMPT, reprompt);
            Assert.Equal("Ginnungagap time", providedTitle);
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
            Assert.Equal("", userInput);
        }


        // Session number tests
        
        [Fact]
        public void Session_number_prompt_displays_a_message()
        {
           setUp("1\n");
            
           WriteSessionManager wsm = new(mockConsoleOut, mockConsoleIn);

           // Act
           wsm.PromptSessionNumber();

           // Assert
           Assert.Equal(MessageStrings.SESSION_NUMBER_PROMPT + "\n", mockConsoleOut.ToString());
           tearDown();
        }

        [Fact]
        public void Session_number_prompt_reprompts_if_non_int_given()
        {
            setUp("Not a number\n99");

            WriteSessionManager wsm = new(mockConsoleOut, mockConsoleIn);

            // Act
            wsm.PromptSessionNumber();
            string reprompt = mockConsoleOut.ToString().Split("\n")[1];
            
            // Assert
            Assert.Equal(MessageStrings.SESSION_NUMBER_INVALID_INPUT, reprompt);
        }

        [Fact]
        public void Session_number_prompt_returns_passed_session_number()
        {
            setUp("23\n");

            WriteSessionManager wsm = new(mockConsoleOut, mockConsoleIn);

            // Act
            var sessionNumber = wsm.PromptSessionNumber();

            // Assert
            Assert.IsType<int>(sessionNumber);
            Assert.Equal(23, sessionNumber);
            Assert.Equal(23, wsm.SessionNumber);
            tearDown();
        }

        [Fact]
        public void Session_number_prompt_reprompts_if_negative()
        {
            setUp("-1\n1\n"); // User enters invalid int, then valid int

            WriteSessionManager wsm = new(mockConsoleOut, mockConsoleIn);

            // Act
            wsm.PromptSessionNumber();

            string reprompt = mockConsoleOut.ToString().Split("\n")[1];

            Assert.Equal(MessageStrings.SESSION_NUMBER_INVALID_INPUT, reprompt);
            tearDown();
        }

        // Create Json tests
        
        [Fact]
        public void Create_Json_creates_json()
        {
            setUp("");

            WriteSessionManager wsm = new(mockConsoleOut, mockConsoleIn, new DateTime(2015, 4, 12, 17, 33, 0, 0));

            //Act
            string json = wsm.CreateJson();
            string expected = "{\n  \"title\": \"\",\n  \"session\": \"\",\n  \"date\": \"4/12/2015\",\n  \"entries\": []\n}";

            Assert.Equal(expected, json);
            tearDown();
                
        }
         
    }
}
