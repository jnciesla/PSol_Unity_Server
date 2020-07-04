using System;
using System.Linq;
using System.Threading;
using Data;
using Data.Services;
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
            General.InitializeServer();
        }

        private static void ConsoleThread()
        {
            var command = "";

            var pulseTimer = new Timer(e =>
            {
                if (Globals.Initialized) ServerTcp.PreparePulseBroadcast();
            },
                null,
                TimeSpan.FromSeconds(1),
                TimeSpan.FromMilliseconds(25));

            // Recharge systems
            var rechargeTimer = new Timer(e =>
            {
                if (Globals.Initialized)
                {
                    _userService.RechargeSystems();
                }
            },
            null,
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(1));

            // Try a repop every 30 seconds
            // Remove old loots
            var repopTimer = new Timer(e =>
            {
                if (Globals.Initialized) _mobService.RepopGalaxy();
                if (Globals.Initialized) _itemService.RemoveLoot();
            },
                null,
                TimeSpan.FromSeconds(1),
                TimeSpan.FromMilliseconds(30000));

            // Wander mobs everys 15 seconds
            var wanderTimer = new Timer(e =>
            {
                if (Globals.Initialized) _mobService.WanderMobs();
            },
                null,
                TimeSpan.FromSeconds(1),
                TimeSpan.FromMilliseconds(15000));


            // Save the game every 5 minutes
            var saveTimer = new Timer(e =>
            {
                if (Globals.Initialized) SaveGame();
            },
                null,
                TimeSpan.FromSeconds(1),
                TimeSpan.FromMilliseconds(300000));


            while (command != "end" && command != "e" && command != "exit" && command != "q" && command != "quit")
            {
                command = Console.ReadLine();
                Console.CursorTop--;
                switch (command)
                {
                    case "save":
                        SaveGame();
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

            // Try one last save on exit
            SaveGame();

            pulseTimer.Dispose();
            repopTimer.Dispose();
            wanderTimer.Dispose();
            saveTimer.Dispose();

        }

        public static void SaveGame()
        {
            Cnsl.Log("Saving game", true);
            try
            {
                _gameService.SaveGame(_userService.ActiveUsers.ToList());
                Cnsl.Finalize("Saving game");
            }
            catch
            {
                Cnsl.Finalize("Saving game", false);
            }
        }
    }
}
