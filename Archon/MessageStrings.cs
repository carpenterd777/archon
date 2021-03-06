using System;

namespace Archon
{
    public class MessageStrings
    {
        public static readonly string SESSION_TITLE_PROMPT = "Session title: ";
        public static readonly string SESSION_TITLE_INT_INPUT =
            "You have input a number for the session title. Did you mean this? [y/n]";
        public static readonly string SESSION_NUMBER_PROMPT = "Session number: ";
        public static readonly string SESSION_NUMBER_INVALID_INPUT =
            "That is not a valid session number. Please input a number, or leave it blank:";
        public static readonly string RECORDING_STOPPED = "Recording has stopped.";

        public static string GetForceExitWarning(string exitCommand) =>
            $"You are about to exit without saving. Enter {exitCommand} again to confirm.";
    }
}
