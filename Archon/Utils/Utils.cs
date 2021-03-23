using System.IO;
using System.Text.Json;
using System;
using System.Diagnostics;

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
                MessageStrings.Warn(consoleOut, $"Cannot read file of type {suffix}");
                System.Environment.Exit(1);
            }
        }

        public static Process CreateLinuxProcess(string command)
        {
            Process proc = new()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{command}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            return proc;
        }
    }
}
