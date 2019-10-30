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
            Cnsl.Info("Initializing Server", true);
            //Intializing all game data arrays
            Cnsl.Debug("Initializing Game Arrays", true);
            for (var i = 0; i < Constants.MAX_PLAYERS; i++)
            {
                ServerTCP.Client[i] = new Clients();
            }
            Cnsl.Finalize("Initializing Game Arrays");
            //Start the Networking
            Cnsl.Debug("Initializing Network", true);
            ServerHandleData.InitializePackets();
            ServerTCP.InitializeNetwork();
            //Load database items
            LoadData();
            var endTime = GetTickCount();
            Cnsl.Info(@"Initialization complete. Server loaded in " + (endTime - startTime) + " ms.");
            Cnsl.Banner("   Server has started successfully \n     @ " + DateTime.UtcNow + " UTC");
            Globals.Initialized = true;
            Cnsl.Finalize("Initializing Server");
        }
        public static void LoadData()
        {
            Cnsl.Info(@"Preparing to load data:");
            var items = 0;
            var startTime = GetTickCount();
            Cnsl.Debug("Loading items", true);
            try
            {
                Globals.Items = Program._itemService.LoadItems();
                Cnsl.Finalize("Loading items");
            }
            catch
            {
                Cnsl.Finalize("Loading items",false);
                return;
            }
            
            items += Globals.Items.Count;
            Cnsl.Debug("Loading galaxy", true);
            try
            {
                Globals.Galaxy = Program._starService.LoadStars();
                Cnsl.Finalize("Loading galaxy");
            }
            catch
            {
                Cnsl.Finalize("Loading galaxy", false);
                return;
            }
            items += Globals.Galaxy.Count;
            var endTime = GetTickCount();
            Cnsl.Info(@"Data loaded (" + items + " points) in " + (endTime - startTime) + " ms.");
        }

    }
}
