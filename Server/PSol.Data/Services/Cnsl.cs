using System;
using System.Collections.Generic;

namespace Data.Services
{
    public static class Cnsl
    {
        public static Dictionary<string, int> buffer = new Dictionary<string, int>();
        private static readonly object _MessageLock = new object();
        public static int crLine = 0;

        public static int Info(string text, bool incomplete = false)
        {
            lock (_MessageLock)
            {
                LineCount();
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(@" [INFO]  " + text);
                crLine++;
                Console.ResetColor();
            }
            if (incomplete) buffer.Add(text, Console.CursorTop - 1);
            return Console.CursorTop - 1;
        }

        public static int Debug(string text, bool incomplete = false)
        {
            lock (_MessageLock)
            {
                LineCount();
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(@" [DEBUG] " + text);
                crLine++;
                Console.ResetColor();
            }
            if (incomplete) buffer.Add(text, Console.CursorTop - 1);
            return Console.CursorTop - 1;
        }

        public static int Log(string text, bool incomplete = false)
        {
            lock (_MessageLock)
            {
                LineCount();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(@" [LOG]   " + text);
                crLine++;
                Console.ResetColor();
            }
            if (incomplete) buffer.Add(text, Console.CursorTop - 1);
            return Console.CursorTop - 1;
        }

        public static void Warn(string text)
        {
            lock (_MessageLock)
            {
                LineCount();
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(@" [WARN]  " + text);
                crLine++;
                Console.ResetColor();
            }
        }

        public static void Banner(string text)
        {
            lock (_MessageLock)
            {
                var len = text.Substring(0, text.IndexOf("\n", StringComparison.Ordinal)).Length;
                Console.WriteLine();
                crLine++;
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(@"  ");
                for (var i = 0; i < len; i++)
                {
                    Console.Write(@"*");
                }

                Console.WriteLine();
                crLine++;
                Console.WriteLine(text);
                crLine++;
                Console.Write(@"  ");
                for (var i = 0; i < len; i++)
                {
                    Console.Write(@"*");
                }

                Console.WriteLine();
                crLine++;
                Console.WriteLine();
                crLine++;
                Console.ResetColor();
            }
        }

        private static void LineCount()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(Console.CursorTop);
        }

        public static void Finalize(string value, bool pass = true)
        {
            lock (_MessageLock)
            {
                Console.SetCursorPosition(Console.BufferWidth - 8, buffer[value]);
                Console.ForegroundColor = pass ? ConsoleColor.DarkGreen : ConsoleColor.Red;
                Console.WriteLine(pass ? @"[PASS]" : @"[FAIL]");
                Console.CursorTop = crLine;
                Console.ResetColor();
            }
            
            buffer.Remove(value);
        }

    }
}
