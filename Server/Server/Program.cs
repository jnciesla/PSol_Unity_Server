using System;
using System.Threading;

namespace Server
{
    class Program
    {
        private static Thread threadConsole;

        static void Main(string[] args)
        {
            threadConsole = new Thread(new ThreadStart(ConsoleThread));
            threadConsole.Start();
            General.InitializeServer();
        }

        static void ConsoleThread()
        {
            string line; string[] parts;

            while (true)
            {
                line = Console.ReadLine();
                parts = line.Split(' ');
            }
        }
    }
}
