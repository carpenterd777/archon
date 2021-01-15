using System;

namespace Archon
{
    /// <summary>
    /// Contains all logic pertaining to the display and input of write sessions.
    /// </summary>
    public class WriteSessionManager
    {
   
        public string SessionTitle;
        public int SessionNumber;

        private System.IO.TextWriter consoleOut; 
        private System.IO.TextReader consoleIn;
        private System.Collections.Generic.List<IEntry> entries = new();
        private bool hasWarnedBeforeForceExit = false;
        private bool IsRecordingAudio = false;

        /// <summary>
        /// Prompts the user to input a session title. Returns the session title, and sets
        /// the session title to the input string.
        /// </summary>
        public string PromptSessionTitle()
        {
            bool userApprovesTitle = false;
            string userInput = null;
            
            while(!userApprovesTitle || userInput == null)
            {
                consoleOut.WriteLine(MessageStrings.SESSION_TITLE_PROMPT);
                userInput = consoleIn.ReadLine(); 
                if (canBeConvertedToInt(userInput))
                {
                    consoleOut.WriteLine(MessageStrings.SESSION_TITLE_INT_INPUT);
                    userApprovesTitle = consoleIn.ReadLine().ToLower() == "y";
                }
                else
                    // It is assumed the user approves the title if it was not an int
                    userApprovesTitle = true;
            }
            
            SessionTitle = userInput;
            return SessionTitle; 
        }

        /// <summary>
        /// Indicates whether or not the passed string could be successfully cast to an int.
        /// Null values are marked false.
        /// </summary>
        private bool canBeConvertedToInt(string s)
        {
            if (s == null)
            {
                return false;
            }

            try
            {
                int.Parse(s);
            }
            catch (FormatException)
            {
                return false;
            }
            catch (OverflowException)
            {
                return false;
            }
            return true;
        }
        
        /// <summary>
        /// Prompts the user to input a session number. Returns the session number.
        /// </summary>
        public int PromptSessionNumber()
        {
            // Display initial prompt
            consoleOut.WriteLine(MessageStrings.SESSION_NUMBER_PROMPT);
            // Get one line of user input
            string userInput = consoleIn.ReadLine();
            // While user input is not an int or begins with a dash (which may indicate a negative)
            while (!canBeConvertedToInt(userInput) || userInput[0] == '-')
            {
                // If user input is empty string
                if (userInput == "")
                {
                    SessionNumber = 0;
                    return 0;
                }
                // Reprompt user for session number
                consoleOut.WriteLine(MessageStrings.SESSION_NUMBER_INVALID_INPUT);
                // Get new line of user input
                userInput = consoleIn.ReadLine();
            }
            // Set session number field to new session number
            SessionNumber = int.Parse(userInput);
            // Return session number
            return SessionNumber;
        }

        /// <summary>
        /// Checks that the input is a valid session number.
        /// </summary>
        public bool IsValidSessionNumber(string potentialSessionNumber)
        {
            return default; // Not implemented  
        }

        /// <summary>
        /// A transaction center for all of the user text commands or the note input that could be received.
        /// Command values are "exit", "e", "e!", "tr", "quit", "q", and "q!".
        /// </summary>
        public void DispatchWriteSessionAction(string userTextCommand)
        {
            switch(userTextCommand)
            {
                case "exit":
                    exitUserTextCommand();
                    break;
                case "e":
                    exitUserTextCommand();
                    break;
                case "e!":
                    forceExitUserTextCommand();
                    break;
                case "tr":
                    toggleRecordUserTextCommand();
                    break;
                case "quit":
                    exitUserTextCommand();
                    break;
                case "q":
                    exitUserTextCommand();
                    break;
                case "q!":
                    forceExitUserTextCommand();
                    break;
                default:
                    noteUserText(userTextCommand);
                    break;
            }
        }

        /// <summary>
        /// Responds to a command by the user during a write session to exit the session.
        /// Saves the user's work and exits the program.
        /// </summary>
        private void exitUserTextCommand()
        {
            return; // Not implemented
        }
        
        /// <summary>
        /// Responds to a command by the user during a write session to exit the session without saving.
        /// Sends a warning to the user the first time they try to force an exit.
        /// </summary>
        private void forceExitUserTextCommand()
        {
            return; // Not implemented
        }
        
        /// <summary>
        /// Toggles the audio recording functionality. When its toggled off, adds the audio entry to the list of entries.
        /// Toggles off itself when 3 minutes have passed.
        /// </summary>
        private void toggleRecordUserTextCommand()
        {
            return; // Not implemented
        }
        
        /// <summary>
        /// Adds a note entry to the list of all entries. Changes display so that the timestamp is appended to the line the
        /// user pressed <Enter> on.
        /// </summary>
        private void noteUserText(string note)
        {
            return; // Not implemented
        }

        /// <summary>
        /// Creates a new WriteSessionManager.
        /// </summary>
        public WriteSessionManager(System.IO.TextWriter consoleOut, System.IO.TextReader consoleIn)
        {
            this.consoleOut = consoleOut;
            this.consoleIn = consoleIn;
        }
    }
}
