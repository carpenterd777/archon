using System;
using System.IO;

namespace Archon.Utils
{
    public class Strings
    {
        public static void Warn(TextWriter consoleOut, string text)
        {
            ConsoleColor previousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            consoleOut.WriteLine(text);
            Console.ForegroundColor = previousColor;
        }

        public static string GetInvalidStatusExceptionMessage(string status) =>
            $"Failed to check for status {status}";

        public static string GetUnsupportedPlatformForRecordingWarning(PlatformID platform) =>
            $"Unsupported platform {platform}. Recording cannot be performed.";
    }
}
