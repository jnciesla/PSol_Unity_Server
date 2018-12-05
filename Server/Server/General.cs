﻿using System;

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
            var startTime = 0; var endTime = 0;
            startTime = GetTickCount();
            Cnsl.Debug("Initializing Server...");
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

            endTime = GetTickCount();
            Cnsl.Info("Initialization complete. Server loaded in " + (endTime - startTime) + " ms.");
        }

        public static void JoinGame(int connectionID)
        {
            ServerTCP.SendIngame(connectionID);
            ServerTCP.SendToGalaxy(connectionID);
        }
    }
}
