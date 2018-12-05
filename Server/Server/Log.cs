using System;

namespace Server
{
    class Log
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
            Console.WriteLine("[Debug]" + text);
            Console.ResetColor();
        }
        public static void Message(string text)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("[Log]" + text);
            Console.ResetColor();
        }
    }
}
