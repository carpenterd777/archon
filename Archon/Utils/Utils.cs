using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System;

namespace Archon.Utils
{
    public class Utilities
    {
        public static bool HasCorrectFileSuffix(string filename)
        {
            string suffix = filename.Substring(filename.IndexOf('.'));
            return suffix == ".archon.json";
        }

        public static void ExitIfWrongFile(string filename, TextWriter consoleOut)
        {
            if (!HasCorrectFileSuffix(filename))
            {
                string suffix = filename.Substring(filename.IndexOf('.'));
                Strings.Warn(consoleOut, $"Cannot read file of type {suffix}");
                System.Environment.Exit(1);
            }
        }

        public static IEntry ReadWholeEntry(Utf8JsonReader reader)
        {
            // entry starts with a string
            string type = reader.GetString();
            reader.Read(); // skip ???
            reader.Read(); // skip "timestamp:"
            string timestamp = reader.GetString();
            reader.Read(); // skip ???
            reader.Read(); // skip "data:"
            string data = reader.GetString();

            IEntry newEntry;
            if (type == "note")
            {
                Timestamp tsFromText = Timestamp.CreateFromString(timestamp);
                newEntry = new TextEntry(data, tsFromText);
                return newEntry;
            }
            else if (type == "recording")
            {
                Timestamp tsFromText = Timestamp.CreateFromString(timestamp);
                newEntry = new AudioEntry(data, tsFromText);
                return newEntry;
            }
            else
            {
                // this should not be reached
                throw new Exception($"reached unknown entry type");
            }
        }
    }
}
