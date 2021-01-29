using System;
using System.IO;

namespace Archon
{
    internal class Warner
    {
        public static void Warn(TextWriter consoleOut, string text)
        {
            ConsoleColor previousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            consoleOut.WriteLine(text);
            Console.ForegroundColor = previousColor;
        }
    }
}
