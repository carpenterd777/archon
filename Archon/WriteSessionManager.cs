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
            consoleOut.WriteLine(MessageStrings.SESSION_TITLE_PROMPT);
            string userInput = new InputHelper(consoleIn).GetNextLine();
            SessionTitle = userInput;
            return userInput; 
        }
        
        /// <summary>
        /// Prompts the user to input a session number. Returns the session number.
        /// </summary>
        public int PromptSessionNumber()
        {
            return default; // Not implemented  
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
        /// </summary
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

    class OutputHelper
    {
        private System.IO.TextWriter consoleOut;
        public void DisplayLine(string text) => consoleOut.WriteLine(text); 
        public OutputHelper(System.IO.TextWriter consoleOut)
        {
            this.consoleOut = consoleOut;
        }
    }
    class InputHelper
    {
        private System.IO.TextReader consoleIn;
        public string GetNextLine() => consoleIn.ReadLine(); 
        public InputHelper(System.IO.TextReader consoleIn)
        {
            this.consoleIn = consoleIn;
        }
    }
}
