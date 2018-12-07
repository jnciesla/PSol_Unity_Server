using System;

namespace Server
{
    class Cnsl
    {
        public static void Info(string text)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(@"[INFO]  " + text);
            Console.ResetColor();
        }
        public static void Debug(string text)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(@"[DEBUG] " + text);
            Console.ResetColor();
        }
        public static void Log(string text)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(@"[LOG]   " + text);
            Console.ResetColor();
        }

        public static void Banner(string text)
        {
            var len = text.Substring(0, text.IndexOf("\n", StringComparison.Ordinal)).Length;
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Gray;
            for (var i = 0; i < len; i++)
            {
                Console.Write(@"*");
            }
            Console.WriteLine();
            Console.WriteLine(text);
            for (var i = 0; i < len; i++)
            {
                Console.Write(@"*");
            }
            Console.WriteLine();
            Console.WriteLine();
            Console.ResetColor();
        }
    }
}
