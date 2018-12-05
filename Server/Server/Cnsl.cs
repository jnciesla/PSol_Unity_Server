using System;

namespace Server
{
    class Cnsl
    {
        public static void Info(string text)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("[INFO]" + text);
            Console.ResetColor();
        }
        public static void Debug(string text)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("[DEBUG]" + text);
            Console.ResetColor();
        }
        public static void Log(string text)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("[LOG]" + text);
            Console.ResetColor();
        }
    }
}
