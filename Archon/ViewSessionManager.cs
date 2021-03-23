using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

using Archon.Utils;

namespace Archon
{
    public class ViewSessionManager
    {
        private TextWriter _consoleOut;        // text stream to write to
        private TextReader _consoleIn;         // text stream to read from
        private string _sessionTitle;          // the session title read from file
        private string _sessionNumber;         // the session number read from file
        private string _date;                  // the date read from file
        private List<IEntry> _entries = new(); // each entry read from file 
        private int _currentSelection = 1;     // the user's currently selected entry
        private int _pageSize;                 // the number of lines composing a page
        private int _currentPage = 1;          // the current page the user is viewing
        private AudioPlaybackManager _audiopm
            = AudioPlaybackManager.GetPlatformSpecificAudioManager();

        // Public API

        /// <summary>
        /// Draws one frame of the viewer.
        /// </summary>
        public void Draw()
        {
            Console.Clear();
            writeLine($"Session title: {_sessionTitle}\tSession number: {_sessionNumber}\tDate: {_date}", ConsoleColor.Magenta);

            int pageStart = _pageSize * (_currentPage - 1) + 1;
            int pageEnd = _pageSize * _currentPage;

            for (int i = pageStart; i <= pageEnd; i++)
            {
                if (i >= _entries.Count + 1)
                {
                    break;
                }

                if (i == _currentSelection)
                {
                    writeLine(_entries[i - 1], ConsoleColor.Black, ConsoleColor.White);
                }
                else
                {
                    _consoleOut.WriteLine(_entries[i - 1]);
                }
            }
        }

        /// <summary>
        /// Loop to draw each frame of the viewing.
        /// </summary>
        public void RenderLoop()
        {
            Draw();
            while (true)
            {
                WaitForInput();
                Draw();
            }
        }

        /// <summary>
        /// Reads the user's next input key on a keyboard and updates the manager state
        /// based on the key.
        /// </summary>
        public void WaitForInput()
        {
            switch (Console.ReadKey(false).Key)
            {
                case ConsoleKey.UpArrow:
                case ConsoleKey.K:
                    moveUp();
                    break;
                case ConsoleKey.DownArrow:
                case ConsoleKey.J:
                    moveDown();
                    break;
                case ConsoleKey.Enter:
                    activateCurrentEntry();
                    break;
                case ConsoleKey.Escape:
                case ConsoleKey.Q:
                    Console.Clear();
                    System.Environment.Exit(0);
                    break;
            }
        }

        /// <summary>
        /// Loads all the data from the file passed at the command line.
        /// </summary>
        public void Load(string filename)
        {
            Utilities.ExitIfWrongFile(filename, _consoleOut);
            System.Text.Json.Utf8JsonReader reader = JsonReaderFactory.CreateJsonReader(
                 new System.Buffers.ReadOnlySequence<Byte>(
                     File.ReadAllBytes(
                         filename)));

            int tokenNumber = 0;
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.String)
                {
                    // session title
                    if (tokenNumber == 0)
                    {
                        _sessionTitle = reader.GetString();
                        tokenNumber++;
                        continue;
                    }

                    // session number
                    if (tokenNumber == 1)
                    {
                        _sessionNumber = reader.GetString();
                        tokenNumber++;
                        continue;
                    }

                    // date
                    if (tokenNumber == 2)
                    {
                        _date = reader.GetString();
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

        private void warn(string text)
        {
            MessageStrings.Warn(_consoleOut, text);
        }

        private void moveUp()
        {
            if (_currentSelection > 1)
            {
                _currentSelection--;
            }

            handlePage();
        }

        private void moveDown()
        {
            if (_currentSelection < _entries.Count)
            {
                _currentSelection++;
            }

            handlePage();
        }

        private void handlePage()
        {
            _currentPage = ((_currentSelection - 1) / _pageSize) + 1;
        }

        private void writeLine(object text, ConsoleColor fgColor)
        {
            ConsoleColor lastFGColor = Console.BackgroundColor;
            Console.ForegroundColor = fgColor;
            _consoleOut.WriteLine(text);
            Console.ForegroundColor = lastFGColor;
        }

        private void writeLine(object text, ConsoleColor fgColor, ConsoleColor bgColor)
        {
            ConsoleColor lastBGColor = Console.BackgroundColor;
            ConsoleColor lastFGColor = Console.ForegroundColor;
            Console.BackgroundColor = bgColor;
            Console.ForegroundColor = fgColor;
            _consoleOut.WriteLine(text);
            Console.BackgroundColor = lastBGColor;
            Console.ForegroundColor = lastFGColor;

        }

        private void activateCurrentEntry()
        {
            IEntry currentEntry = _entries[_currentSelection - 1];

            if (currentEntry.GetType().Equals(typeof(AudioEntry)))
            {
                _audiopm.Filename = currentEntry.GetData();
                _audiopm.Play();
            }
        }

        // Constructors

        public ViewSessionManager(TextWriter consoleOut, TextReader consoleIn)
        {
            _consoleOut = consoleOut;
            _consoleIn = consoleIn;

            // minus one to reserve the top line for the title, number, and date
            _pageSize = Console.WindowHeight - 2;
        }
    }
}
