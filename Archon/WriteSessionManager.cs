using System;
using System.IO;

namespace Archon
{
    /// <summary>
    /// Contains all logic pertaining to the display and input of write sessions.
    /// </summary>
    public class WriteSessionManager
    {
   
        public string SessionTitle;
        public int SessionNumber;

        private TextWriter _consoleOut; 
        private TextReader _consoleIn;
        private System.Collections.Generic.List<IEntry> _entries = new();
        private bool _hasWarnedBeforeForceExit = false;
        private bool _isRecordingAudio = false;
        private System.DateTime _dateCreated;

        private const string _prompt = "> ";

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
                _consoleOut.WriteLine(MessageStrings.SESSION_TITLE_PROMPT);
                userInput = _consoleIn.ReadLine(); 
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
            _consoleOut.WriteLine(MessageStrings.SESSION_TITLE_INT_INPUT);
            return _consoleIn.ReadLine().ToLower() == "y";
        }

        /// <summary>
        /// Prompts the user to input a session number. Returns the session number.
        /// </summary>
        public int PromptSessionNumber()
        {
            // Display initial prompt
            _consoleOut.WriteLine(MessageStrings.SESSION_NUMBER_PROMPT);
            // Get one line of user input
            string userInput = _consoleIn.ReadLine();
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
                _consoleOut.WriteLine(MessageStrings.SESSION_NUMBER_INVALID_INPUT);
                // Get new line of user input
                userInput = _consoleIn.ReadLine();
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
               _consoleOut.Write(_prompt);
               nextInput = _consoleIn.ReadLine();
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

        /// <summary>
        /// Creates a JSON string based on all of the entries that the user has created so far during this session.
        /// </summary>
        public string CreateJson()
        {
            string sessionTitleinJson = SessionTitle == null ? "" : SessionTitle;
            string sessionNumberinJson = SessionNumber == 0 ? "" : SessionNumber.ToString();
            
            MemoryStream stream = new();

            using (System.Text.Json.Utf8JsonWriter jsonWriter = createArchonJsonWriter(stream))
            {
                jsonWriter.WriteStartObject();

                jsonWriter.WriteString("title", sessionTitleinJson);
                jsonWriter.WriteString("session", sessionNumberinJson);
                jsonWriter.WriteString("date", _dateCreated.ToShortDateString());

                jsonWriter.WritePropertyName("entries");
                jsonWriter.WriteStartArray();
                foreach (IEntry entry in _entries)
                {
                    entry.AddToJsonWriter(jsonWriter);
                }
                jsonWriter.WriteEndArray();

                jsonWriter.WriteEndObject();
            }

            stream.Position = 0;
            string json; 
            using (StreamReader sr = new(stream))
            {
                json = sr.ReadToEnd();
            }
            return json;
        }
    
        /// <summary>
        /// Creates an .archon.json file in the directory that the write command was called from.
        /// </summary>
        public void SaveEntries()
        {
            string filename;
            // If the session title is set
            if (SessionTitle != "")
            {
                filename = SessionTitle;
                filename = removeIllegalFilenameCharacters(filename); 
                filename = createUniqueFileNameFromString(filename);
            }
            else
            {
                filename = _dateCreated.ToString("yyyy_MM_dd");
                filename = removeIllegalFilenameCharacters(filename);
                filename = createUniqueFileNameFromString(filename);
            }

            //  Write the Json to the file
            using (StreamWriter sw = File.CreateText(Path.Combine(Directory.GetCurrentDirectory(), filename)))
            {
                sw.Write(CreateJson());
            }

            return;
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
            if (_hasWarnedBeforeForceExit)
                System.Environment.Exit(0);
            // Else the user has not yet warned
            else
            {
                // Warn the user
                warn(MessageStrings.GetForceExitWarning(exitCommand));
                
                // Note that the user has been warned
                _hasWarnedBeforeForceExit = true;
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
            _entries.Add(entry);
            // Change display of last line so that timestamp in string form is prepended to it
            return; // Not implemented
        }

        private void warn(string text)
        {
            System.ConsoleColor previousColor = System.Console.ForegroundColor;
            System.Console.ForegroundColor = ConsoleColor.Red;
            _consoleOut.WriteLine(text);
            System.Console.ForegroundColor = previousColor;
        }

        private System.Text.Json.Utf8JsonWriter createArchonJsonWriter(Stream stream) => 
            ArchonJsonWriterFactory.CreateArchonJsonWriter(stream);

        private string createUniqueFileNameFromString(string filename)
        {
            // Create list of strings with names of all files from directory
            System.Collections.Generic.List<string> allFileNames = 
                new(Directory.EnumerateFileSystemEntries(Directory.GetCurrentDirectory()));
            // Keep track of number of iterations attempted
            string newFilename = filename + ".archon.json";
            int iterations = 1;
            // while filename matches name of any file from directory
            while (allFileNames.Contains(Path.Combine(Directory.GetCurrentDirectory(), newFilename)))
            {
            //  Append number of iterations appended to filename
                newFilename = filename + "(" + iterations.ToString() + ")" + ".archon.json";
                iterations++;
            }
            // Return filename
            return newFilename;
        }

        private string removeIllegalFilenameCharacters(string filename)
        {
            System.Collections.Generic.List<char> illegalCharacters = 
                new (Path.GetInvalidFileNameChars());

            System.Text.StringBuilder buffer = new();

            foreach(char c in filename)
            {
                if (illegalCharacters.Contains(c))
                    buffer.Append("_");
                else
                    buffer.Append(c);
            }

            return buffer.ToString();
        }

        // Constructors

        /// <summary>
        /// Creates a new WriteSessionManager.
        /// </summary>
        public WriteSessionManager(TextWriter consoleOut, TextReader consoleIn) 
            : this(consoleOut, consoleIn, System.DateTime.Now)
        {
        }

        /// <summary>
        /// Creates a new WriteSessionManager set to have been created at the passed in date.
        /// </summary>
        public WriteSessionManager(TextWriter consoleOut, TextReader consoleIn, System.DateTime dateCreated)
        {
            _consoleOut = consoleOut;
            _consoleIn = consoleIn;
            _dateCreated = dateCreated; 
        }
    }
}
