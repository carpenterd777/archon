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
        private int _currentSelection = 0;     // the user's currently selected entry
        private int _maxSelection;             // the last selection a user can make before it stops incrementing
        private int _pageSize;                 // the number of lines composing a page
        private int _currentPage;              // the current page the user is viewing

        // Public API

        /// <summary>
        /// Draws one frame of the viewer.
        /// </summary>
        public void Draw()
        {
            Console.Clear();
            _consoleOut.WriteLine($"{_sessionTitle}\t{_sessionNumber}\t{_date}");

            for (int i = 0; i < _entries.Count; i++)
            {
                if (i == _currentSelection)
                {
                    ConsoleColor lastBGColor = Console.BackgroundColor;
                    ConsoleColor lastFGColor = Console.ForegroundColor;
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                    _consoleOut.WriteLine(_entries[i]);
                    Console.BackgroundColor = lastBGColor;
                    Console.ForegroundColor = lastFGColor;
                }
                else
                {
                    _consoleOut.WriteLine(_entries[i]);
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
                case ConsoleKey.Escape:
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
                        string date = reader.GetString();
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
            Strings.Warn(_consoleOut, text);
        }

        // Constructors

        public ViewSessionManager(TextWriter consoleOut, TextReader consoleIn)
        {
            _consoleOut = consoleOut;
            _consoleIn = consoleIn;

            // minus one to reserve the top line for the title, number, and date
            _pageSize = Console.BufferHeight - 1;
        }
    }
}
