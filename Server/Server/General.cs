using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using Bindings;
using Data;
using Data.Services;

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
                ServerTcp.Client[i] = new Clients();
            }
            Cnsl.Finalize("Initializing Game Arrays");
            //Start the Networking
            Cnsl.Debug("Initializing Network", true);
            Cnsl.Debug("Initializing Status Listener", true);
            ServerHandleData.InitializePackets();
            ServerTcp.InitializeNetwork();
            ServerTcp.InitializeStatusListener();
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
            Cnsl.Debug("Loading recipes", true);
            try
            {
                Globals.Recipes = Program._itemService.LoadRecipes();
                Cnsl.Finalize("Loading recipes");
            }
            catch
            {
                Cnsl.Finalize("Loading recipes", false);
                return;
            }

            Cnsl.Debug("Building hash table", true);
            try
            {
                PopulateHashTable();
                Cnsl.Finalize("Building hash table");
            }
            catch
            {
                Cnsl.Finalize("Building hash table", false);
                return;
            }

            items += Globals.Recipes.Count;
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

        public static string CalculateMD5Hash(string input)
        {
            var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(input);
            var hash = md5.ComputeHash(inputBytes);
            var sb = new StringBuilder();
            foreach (var t in hash)
            {
                sb.Append(t.ToString("X2"));
            }
            return sb.ToString();
        }

        public static void PopulateHashTable()
        {
            foreach (var r in Globals.Recipes)
            {
                var preString = r.Item1 + r.Item2 + r.Item3 + r.Item4 + r.Item5 + r.Item6 + r.Item7 + r.Item8;
                var hash = CalculateMD5Hash(preString);
                Globals.RecipeHash.Add(hash, r.ID);
            }
        }
    }
}
