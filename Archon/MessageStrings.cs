using System;
using System.IO;

namespace Archon
{
    public class MessageStrings
    {
        public static void Warn(TextWriter consoleOut, string text)
        {
            ConsoleColor previousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            consoleOut.WriteLine(text);
            Console.ForegroundColor = previousColor;
        }

        public static readonly string SESSION_TITLE_PROMPT = "Session title: ";
        public static readonly string SESSION_TITLE_INT_INPUT =
            "You have input a number for the session title. Did you mean this? [y/n]";
        public static readonly string SESSION_NUMBER_PROMPT = "Session number: ";
        public static readonly string SESSION_NUMBER_INVALID_INPUT =
            "That is not a valid session number. Please input a number, or leave it blank:";
        public static readonly string RECORDING_STOPPED = "Recording has stopped.";

        public static readonly string NO_ALSA =
            "Could not find ALSA driver on this system. Cannot record audio without ALSA support";

        public static string GetForceExitWarning(string exitCommand) =>
            $"You are about to exit without saving. Enter {exitCommand} again to confirm.";

        public static string GetUnsupportedPlatformForRecordingWarning(PlatformID platform) =>
            $"Unsupported platform {platform}. Recording cannot be performed.";
    }
}
