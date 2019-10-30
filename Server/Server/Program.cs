using System;
using System.Linq;
using System.Threading;
using Data.Services.Interfaces;
using Ninject;

namespace Server
{
    class Program
    {
        private static Thread threadConsole;
        public static IMobService _mobService;
        public static IGameService _gameService;
        public static IUserService _userService;
        public static IStarService _starService;
        public static IItemService _itemService;
        public static ICombatService _combatService;

        private static void Main()
        {
            threadConsole = new Thread(ConsoleThread);
            threadConsole.Start();
            IKernel kernel = new StandardKernel(new ServerModule());
            _gameService = kernel.Get<IGameService>();
            _mobService = kernel.Get<IMobService>();
            _userService = kernel.Get<IUserService>();
            _starService = kernel.Get<IStarService>();
            _itemService = kernel.Get<IItemService>();
            _combatService = kernel.Get<ICombatService>();
            General.InitializeServer();
        }

        private static void ConsoleThread()
        {
            var command = "";

            var pulseTimer = new Timer(e =>
                {
                    if (Globals.Initialized) ServerTCP.PreparePulseBroadcast();
                },
                null,
                TimeSpan.FromSeconds(1),
                TimeSpan.FromMilliseconds(25));


            while (command != "end" && command != "e" && command != "exit" && command != "q" && command != "quit")
            {
                command = Console.ReadLine();
                Console.CursorTop--;
                switch (command)
                {
                    case "save":
                        Cnsl.Log("Saving game",true);
                        try
                        {
                            _gameService.SaveGame(_userService.ActiveUsers.ToList());
                            Cnsl.Finalize("Saving game");
                        }
                        catch
                        {
                            Cnsl.Finalize("Saving game", false);
                        }

                        break;
                    case "":
                        Cnsl.Debug("#");
                        break;
                    default:
                        if (command != "end" && command != "e" && command != "exit" && command != "q" && command != "quit")
                        {
                            Cnsl.Debug(@"Unknown Command");
                        }

                        break;
                }
            }

            pulseTimer.Dispose();

        }
    }
}
