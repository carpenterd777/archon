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
                    userApprovesTitle = VerifyIntSessionTitle();
                else
                    // It is assumed the user approves the title if it was not an int
                    userApprovesTitle = true;
            }
            
            SessionTitle = userInput;
            return SessionTitle; 
        }

        /// <summary>
        /// Verifies with the user that the int that they passed as a session title was intended.
        /// </summary>
        public bool VerifyIntSessionTitle()
        {
            consoleOut.WriteLine(MessageStrings.SESSION_TITLE_INT_INPUT);
            return consoleIn.ReadLine().ToLower() == "y";
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
            while (!IsValidSessionNumber(userInput))
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
        public bool IsValidSessionNumber(string potentialSessionNumber) => 
            canBeConvertedToInt(potentialSessionNumber) && int.Parse(potentialSessionNumber) >= 0;
        
        public void CommandLoop()
        {
           System.Console.Clear(); 

           string nextInput;
           while (true)
           {
               consoleOut.Write("> ");
               nextInput = consoleIn.ReadLine();
               DispatchWriteSessionAction(nextInput);
           }
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
                case "exit!":
                    forceExitUserTextCommand(userTextCommand);
                    break;
                case "e!":
                    forceExitUserTextCommand(userTextCommand);
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
                case "quit!":
                    forceExitUserTextCommand(userTextCommand);
                    break;
                case "q!":
                    forceExitUserTextCommand(userTextCommand);
                    break;
                default:
                    noteUserText(userTextCommand);
                    break;
            }
        }
    
        public void SaveEntries()
        {
            return; // Not implemented
        }

        /// <summary>
        /// Indicates whether or not the passed string could be successfully cast to an int.
        /// Null values are marked false.
        /// </summary>
        private bool canBeConvertedToInt(string s)
        {
            if (s == null)
                return false;

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
        /// Responds to a command by the user during a write session to exit the session.
        /// Saves the user's work and exits the program.
        /// </summary>
        private void exitUserTextCommand()
        {
            SaveEntries();
            System.Environment.Exit(0);
            return; 
        }
        
        /// <summary>
        /// Responds to a command by the user during a write session to exit the session without saving.
        /// Sends a warning to the user the first time they try to force an exit.
        /// </summary>
        private void forceExitUserTextCommand(string exitCommand)
        {
            // If has already warned user about exiting without saving
            if (hasWarnedBeforeForceExit)
                System.Environment.Exit(0);
            // Else the user has not yet warned
            else
            {
                // Warn the user
                warn(MessageStrings.GetForceExitWarning(exitCommand));
                
                // Note that the user has been warned
                hasWarnedBeforeForceExit = true;
            }
        }
        
        /// <summary>
        /// Toggles the audio recording functionality. When its toggled off, adds the audio entry to the list of entries.
        /// Toggles off itself when 3 minutes have passed. Indicating the new file's name and recording success when toggled off.
        /// </summary>
        private void toggleRecordUserTextCommand()
        {
            return; // Not implemented
        }
        
        /// <summary>
        /// Adds a note entry to the list of all entries. Changes display so that the timestamp is prepended to the line the
        /// user pressed <Enter> on.
        /// </summary>
        private void noteUserText(string note)
        {
            // Create timestamp for this instant
            Timestamp tsNow = new();
            // Create a text entry object using note and timestamp
            TextEntry entry = new(note, tsNow);
            // Add text entry object to list of all entries
            entries.Add(entry);
            // Change display of last line so that timestamp in string form is prepended to it
            return; // Not implemented
        }

        private void warn(string text)
        {
            System.Console.ForegroundColor = ConsoleColor.Red;
            consoleOut.WriteLine(text);
            System.Console.ForegroundColor = ConsoleColor.White;
        }

        // Constructors

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
