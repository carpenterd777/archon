using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;

using Archon.Utils;

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
        private List<IEntry> _entries = new();
        private bool _hasWarnedBeforeForceExit = false;
        private bool _isRecordingAudio = false;
        private System.DateTime _dateCreated;
        private AudioRecManager _audiorm;

        private const string _prompt = "> ";

        /// <summary>
        /// Prompts the user to input a session title. Returns the session title, and sets
        /// the session title to the input string.
        /// </summary>
        public string PromptSessionTitle()
        {
            bool userApprovesTitle = false;
            string userInput = null;

            while (!userApprovesTitle || userInput == null)
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
            printAllEntries();

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
            DispatchWriteSessionAction(userTextCommand, DateTime.Now);
        }

        /// <summary>
        /// A transaction center for all of the user text commands or the note input that could be received.
        /// Accepts a DateTime for testing.
        /// Command values are "exit", "e", "e!", "tr", "quit", "q", and "q!".
        /// </summary>
        public void DispatchWriteSessionAction(string userTextCommand, DateTime dt)
        {
            switch (userTextCommand)
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
                    noteUserText(userTextCommand, dt);
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

            using (System.Text.Json.Utf8JsonWriter jsonWriter = createJsonWriter(stream))
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
            string filename = SessionTitle != "" ? SessionTitle : _dateCreated.ToString("yyyy_MM_dd");

            filename = removeIllegalFilenameCharacters(filename);
            filename = createUniqueFileNameFromString(filename);

            //  Write the Json to the file
            using (StreamWriter sw = File.CreateText(Path.Combine(Directory.GetCurrentDirectory(), filename)))
            {
                sw.Write(CreateJson());
            }

            return;
        }

        /// <summary>
        /// Loads an .archon.json file to continue appending to.
        /// </summary>
        public void Load(string filename)
        {
            Utilities.ExitIfWrongFile(filename, _consoleOut);

            System.Text.Json.Utf8JsonReader reader = JsonReaderFactory.CreateJsonReader(
                new System.Buffers.ReadOnlySequence<Byte>(
                    File.ReadAllBytes(
                        filename)));

            int tokenNumber = 0;

            // this process assumes the json is properly saved
            while (reader.Read())
            {

                if (reader.TokenType == JsonTokenType.String)
                {
                    // session title
                    if (tokenNumber == 0)
                    {
                        SessionTitle = reader.GetString();
                        tokenNumber++;
                        continue;
                    }

                    // session number
                    if (tokenNumber == 1)
                    {
                        string sessionNumString = reader.GetString();
                        if (!canBeConvertedToInt(sessionNumString))
                        {
                            SessionNumber = 0;
                        }
                        else
                        {
                            SessionNumber = int.Parse(sessionNumString);
                        }
                        tokenNumber++;
                        continue;
                    }

                    // date
                    if (tokenNumber == 2)
                    {
                        string date = reader.GetString();
                        _dateCreated = convertDateStringToDateTime(date);
                        tokenNumber++;
                        continue;
                    }

                    // read whole entry at once

                    // entry starts with a string
                    string type = reader.GetString();
                    reader.Read(); // skip ???
                    reader.Read(); // skip "timestamp:"
                    string timestamp = reader.GetString();
                    reader.Read(); // skip ???
                    reader.Read(); // skip "data:"
                    string data = reader.GetString();

                    if (type == "note")
                    {
                        Timestamp tsFromText = Timestamp.CreateFromString(timestamp);
                        TextEntry newEntry = new(data, tsFromText);
                        _entries.Add(newEntry);
                    }
                    else if (type == "recording")
                    {
                        Timestamp tsFromText = Timestamp.CreateFromString(timestamp);
                        AudioEntry newEntry = new(data, tsFromText);
                        _entries.Add(newEntry);
                    }
                    tokenNumber++;
                }
            }
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
            resetForceExitWarning();
            if (!_isRecordingAudio)
            {
                Timestamp tsNow = new();

                string filename =
                    $"{AudioRecManager.ArchonRecordingsDir}/{DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss")}.mp3";

                AudioEntry entry = new(filename, tsNow);

                if (_audiorm.CanRecord())
                {
                    _entries.Add(entry);
                    _audiorm.Filename = filename;
                    _audiorm.StartRecording();
                    _isRecordingAudio = true;
                    rewriteLineAbove($"{tsNow.ToString()} Recording to {filename}...");
                }
                else
                {
                    rewriteLineAbove("Was unable to start recording.");
                }
            }
            else // already recording
            {
                _audiorm.StopRecording();

                _isRecordingAudio = false;
                rewriteLineAbove(MessageStrings.RECORDING_STOPPED);
            }
        }

        /// <summary>
        /// Adds a note entry to the list of all entries. Changes display so that the current timestamp is prepended to the 
        /// line the user pressed <Enter> on.
        /// </summary>
        private void noteUserText(string note)
        {
            noteUserText(note, new DateTime());
        }

        /// <summary>
        /// Adds a note entry to the list of all entries. Changes display so that the timestamp is prepended to the line the
        /// user pressed <Enter> on.
        /// </summary>
        private void noteUserText(string note, DateTime dt)
        {
            resetForceExitWarning();
            // Create timestamp for this instant
            Timestamp tsNow = new(dt);
            // Create a text entry object using note and timestamp
            TextEntry entry = new(note, tsNow);
            // Add text entry object to list of all entries
            _entries.Add(entry);
            // Change display of last line(s) so that timestamp in string form is prepended to it
            rewriteLinesAbove($"{tsNow.ToString()} {note}");
        }

        private void warn(string text)
        {
            MessageStrings.Warn(_consoleOut, text);
        }

        private System.Text.Json.Utf8JsonWriter createJsonWriter(Stream stream) =>
            JsonWriterFactory.CreateJsonWriter(stream);

        private string createUniqueFileNameFromString(string filename)
        {
            // Create list of strings with names of all files from directory
            System.Collections.Generic.List<string> allFileNames =
                new(Directory.EnumerateFileSystemEntries(Directory.GetCurrentDirectory()));

            // Clean up the passed filename
            string fileSuffix = ".archon.json";
            string newFilename = "";
            foreach (char c in filename.ToLower())
            {
                if (c == '.')
                {
                    continue;
                }
                else if (c == ' ')
                {
                    newFilename += '_';
                }
                else
                {
                    newFilename += c;
                }
            }
            newFilename += fileSuffix;

            int iterations = 1;
            while (allFileNames.Contains(Path.Combine(Directory.GetCurrentDirectory(), newFilename)))
            {
                // Append number of iterations to filename
                newFilename = newFilename + "_" + iterations.ToString() + fileSuffix;
                iterations++;
            }

            return newFilename;
        }

        private string removeIllegalFilenameCharacters(string filename)
        {
            System.Collections.Generic.List<char> illegalCharacters =
                new(Path.GetInvalidFileNameChars());

            System.Text.StringBuilder buffer = new();

            foreach (char c in filename)
            {
                if (illegalCharacters.Contains(c))
                    buffer.Append("_");
                else
                    buffer.Append(c);
            }

            return buffer.ToString();
        }

        private void resetForceExitWarning() => _hasWarnedBeforeForceExit = false;

        private void rewriteLineAbove(string newLineAbove)
        {
            rewriteLinesAbove(newLineAbove, 1);
        }

        private void rewriteLinesAbove(string newLineAbove, int numberOfLines)
        {
            Console.SetCursorPosition(0, Console.CursorTop - numberOfLines);
            for (int i = 0; i < numberOfLines; i++)
            {
                _consoleOut.WriteLine(""); // clear the line
            }
            Console.SetCursorPosition(0, Console.CursorTop - numberOfLines);
            _consoleOut.WriteLine(newLineAbove);
        }

        private void rewriteLinesAbove(string newLineAbove)
        {
            double fractionalLines = Convert.ToDouble(newLineAbove.Length) / Convert.ToDouble(Console.BufferWidth);
            int numberOfLines = (int)Math.Ceiling(fractionalLines);
            rewriteLinesAbove(newLineAbove, numberOfLines);
        }

        private DateTime convertDateStringToDateTime(string dateString)
        {
            string[] splits = dateString.Split('/');
            int day;
            int month;
            int year;
            try
            {
                day = int.Parse(splits[1]);
                month = int.Parse(splits[0]);
                year = int.Parse(splits[2]);
            }
            catch (FormatException)
            {
                warn("There was a problem reading the date. Setting the date to today");
                return DateTime.Today;
            }
            return new DateTime(year, month, day);
        }

        private void printAllEntries()
        {
            foreach (IEntry entry in _entries)
            {
                _consoleOut.WriteLine(entry);
            }
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
            _audiorm = AudioRecManager.GetPlatformSpecificAudioManager(_consoleOut);
        }
    }
}
