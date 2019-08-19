using System;
using Bindings;

namespace Server
{
    class General
    {
        public static int GetTickCount()
        {
            return Environment.TickCount;
        }

        public static void InitializeServer()
        {
            var startTime = GetTickCount();
            Cnsl.Info("Initializing Server...");
            //Intializing all game data arrays
            Cnsl.Debug("Initializing Game Arrays...");
            for (var i = 0; i < Constants.MAX_PLAYERS; i++)
            {
                ServerTCP.Client[i] = new Clients();
            }
            //Start the Networking
            Cnsl.Debug("Initializing Network...");
            ServerHandleData.InitializePackets();
            ServerTCP.InitializeNetwork();
            //Load database items
            LoadData();
            var endTime = GetTickCount();
            Cnsl.Info("Initialization complete. Server loaded in " + (endTime - startTime) + " ms.");
            Cnsl.Banner(" Server has started successfully \n   @ "+ DateTime.UtcNow + " UTC");
            Globals.Initialized = true;
        }
        public static void LoadData()
        {
            Cnsl.Info("Loading data:");
            var items = 0;
            var startTime = GetTickCount();
            Cnsl.Debug("Loading items...");
            Globals.Items = Program._itemService.LoadItems();
            items += Globals.Items.Count;
            Cnsl.Debug("Loading galaxy...");
            Globals.Galaxy = Program._starService.LoadStars();
            items += Globals.Galaxy.Count;
            var endTime = GetTickCount();
            Cnsl.Info("Data loaded (" + items + " points) in " + (endTime - startTime) + " ms.");
        }

    }
}
