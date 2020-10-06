using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Data;
using Data.Services;

namespace Server
{
    internal class ServerTcp
    {
        private static TcpListener serverSocket;
        private static TcpListener statusSocket;
        public static Dictionary<int, Clients> Client = new Dictionary<int, Clients>();

        public static void InitializeNetwork()
        {
            try
            {
                serverSocket = new TcpListener(IPAddress.Any, Constants.PORT);
                serverSocket.Start();
                serverSocket.BeginAcceptTcpClient(OnClientConnect, null);
                Cnsl.Finalize("Initializing Network");
            }
            catch
            {
                Cnsl.Finalize("Initializing Network", false);
            }
            Console.Title = @"Project Sol Server | " + Constants.PORT;
        }

        public static void InitializeStatusListener()
        {
            try
            {
                statusSocket = new TcpListener(IPAddress.Any, Constants.STATUS_PORT);
                statusSocket.Start();
                statusSocket.BeginAcceptTcpClient(OnStatusRequest, null);
                Cnsl.Finalize("Initializing Status Listener");
            }
            catch
            {
                Cnsl.Finalize("Initializing Status Listener", false);
            }
        }

        private static void OnStatusRequest(IAsyncResult result)
        {
            statusSocket.EndAcceptTcpClient(result);
            statusSocket.BeginAcceptTcpClient(OnStatusRequest, null);

        }

        private static void OnClientConnect(IAsyncResult result)
        {
            var client = serverSocket.EndAcceptTcpClient(result);
            serverSocket.BeginAcceptTcpClient(OnClientConnect, null);
            var port = ((IPEndPoint)client.Client.RemoteEndPoint).Port;
            if (!Client.ContainsKey(port))
            {
                var newClient = new Clients
                {
                    socket = client,
                    connectionID = port,
                    ip = client.Client.RemoteEndPoint.ToString()
                };
                Client.Add(port, newClient);
                Client[port].Start();
                Cnsl.Log("Connection received from " + Client[port].ip);
                return;
            }
        }

        public static void SendDataTo(int index, byte[] data)
        {
            var buffer = new ByteBuffer();
            var compressed = Compress(data);
            buffer.WriteBytes(compressed);
            try
            {
                Client[index].myStream.Write(buffer.ToArray(), 0, buffer.ToArray().Length);
            }
            catch
            {
                Cnsl.Debug(@"Unable to send packet- client disconnected");
            }

            buffer.Dispose();
        }

        public static void SendDataToAll(byte[] data)
        {
            foreach (var instance in Client)
            {
                if (instance.Value.socket == null) continue;
                var player = Program._userService.ActiveUsers.FirstOrDefault(p => p.Id == Globals.PlayerIDs[instance.Value.connectionID]);
                if (player == null) continue;
                if (!player.inGame) continue;
                SendDataTo(instance.Value.connectionID, data);
            }
        }

        #region SendPackages
        public static void SendIngame(int connectionId, int suppress = 0)
        {
            var buffer = new ByteBuffer();
            var player = Program._userService.ActiveUsers.Find(p => p.Id == Globals.PlayerIDs[connectionId]);
            buffer.WriteLong((long)ServerPackets.SIngame);
            buffer.WriteInteger(connectionId);
            buffer.WriteInteger(suppress);
            buffer.WriteString(player.Id);
            buffer.WriteString(player.Name);
            buffer.WriteFloat(player.X);
            buffer.WriteFloat(player.Z);
            buffer.WriteFloat(player.Heading);
            buffer.WriteFloat(player.Roll);
            buffer.WriteInteger(player.Health);
            buffer.WriteInteger(player.MaxHealth);
            buffer.WriteInteger(player.Shield);
            buffer.WriteInteger(player.MaxShield);
            buffer.WriteString(player.Rank);
            buffer.WriteInteger(player.Credits);
            buffer.WriteInteger(player.Exp);
            buffer.WriteInteger(player.Level);
            buffer.WriteInteger(player.Weap1Charge);
            buffer.WriteInteger(player.Weap2Charge);
            buffer.WriteInteger(player.Weap3Charge);
            buffer.WriteInteger(player.Weap4Charge);
            buffer.WriteInteger(player.Weap5Charge);
            buffer.WriteArray(player.Inventory.ToArray());
            buffer.WriteInteger(player.Admin ? 1 : 0);
            buffer.WriteString(player.Color);
            SendDataTo(connectionId, buffer.ToArray());
            buffer.Dispose();
        }

        public static void SendInventory(int connectionId)
        {
            var buffer = new ByteBuffer();
            var player = Program._userService.ActiveUsers.Find(p => p.Id == Globals.PlayerIDs[connectionId]);
            buffer.WriteLong((long)ServerPackets.SInventory);
            buffer.WriteArray(player.Inventory.ToArray());
            SendDataTo(connectionId, buffer.ToArray());
            buffer.Dispose();
        }

        public static byte[] PlayerData(int connectionId)
        {
            var buffer = new ByteBuffer();
            buffer.WriteLong((long)ServerPackets.SPlayerData);
            buffer.WriteInteger(connectionId);
            var data = buffer.ToArray();
            buffer.Dispose();
            return data;
        }

        public static void SendToGalaxy(int connectionId)
        {
            foreach (var instance in Client)
            {
                if (instance.Value.socket == null) continue;
                if (connectionId != instance.Value.connectionID) SendDataTo(connectionId, PlayerData(instance.Value.connectionID));
            }
            SendDataToAll(PlayerData(connectionId));
        }

        public static void SendMessage(int connectionId, string message, int type)
        {
            var buffer = new ByteBuffer();
            buffer.WriteLong((long)ServerPackets.SMessage);
            buffer.WriteString(message);
            buffer.WriteInteger(type);
            if (connectionId < 0)
            {
                SendDataToAll(buffer.ToArray());
            }
            else
            {
                SendDataTo(connectionId, buffer.ToArray());
            }
            buffer.Dispose();
        }

        public static void PreparePulseBroadcast()
        {
            const int look = 500;
            Program._mobService.CycleArrays();
            Program._mobService.CheckAggro();
            Program._mobService.DoCombat();
            foreach (var instance in Client)
            {
                if (instance.Value.socket != null)
                {
                    if (!Globals.PlayerIDs.ContainsKey(instance.Value.connectionID)) continue;
                    var player = Program._userService.ActiveUsers.FirstOrDefault(p => p.Id == Globals.PlayerIDs[instance.Value.connectionID]);
                    if (player == null) continue;
                    if (player.inGame && player.receiving)
                    {
                        var buffer = new ByteBuffer();
                        buffer.WriteLong((long)ServerPackets.SPulse);
                        buffer.WriteInteger(Program._userService.ActiveUsers.Count);
                        buffer.WriteBytes(BitConverter.GetBytes(DateTime.UtcNow.ToBinary()));
                        Program._userService.ActiveUsers.ForEach(p =>
                        {
                            buffer.WriteInteger(p.connectionID);
                            buffer.WriteString(p.Id);
                            buffer.WriteFloat(p.X);
                            buffer.WriteFloat(p.Z);
                            buffer.WriteFloat(p.Heading);
                            buffer.WriteFloat(p.Roll);
                            buffer.WriteInteger(p.M);
                            buffer.WriteInteger(p.Health);
                            buffer.WriteInteger(p.MaxHealth);
                            buffer.WriteInteger(p.Shield);
                            buffer.WriteInteger(p.MaxShield);
                            buffer.WriteInteger(p.Exp);
                            buffer.WriteInteger(p.Level);
                            buffer.WriteInteger(p.Credits);
                            buffer.WriteInteger(player.Weap1Charge);
                            buffer.WriteInteger(player.Weap2Charge);
                            buffer.WriteInteger(player.Weap3Charge);
                            buffer.WriteInteger(player.Weap4Charge);
                            buffer.WriteInteger(player.Weap5Charge);
                            buffer.WriteBytes(BitConverter.GetBytes(p.inGame));
                        });
                        var minX = (int)player.X - look;
                        var minY = (int)player.Z - look;
                        var maxX = (int)player.X + look;
                        var maxY = (int)player.Z + look;
                        buffer.WriteArray(Program._mobService.GetMobs(minX, maxX, minY, maxY).ToArray());
                        buffer.WriteArray(Program._mobService.GetCombats((int)player.X, (int)player.Z).ToArray());
                        buffer.WriteArray(Globals.Loot.Where(m => m.X >= minX && m.X <= maxX && m.Y >= minY && m.Y <= maxY).ToArray());
                        SendDataTo(instance.Value.connectionID, buffer.ToArray());
                        buffer.Dispose();
                    }
                }
            }
        }

        public static void SendGalaxy(int index)
        {
            var buffer = new ByteBuffer();
            buffer.WriteLong((long)ServerPackets.SGalaxy);
            buffer.WriteArray(Globals.Galaxy.ToArray());
            buffer.WriteArray(Globals.Structures.ToArray());
            SendDataTo(index, buffer.ToArray());
            buffer.Dispose();
        }

        public static void SendRecipes(int index)
        {
            var buffer = new ByteBuffer();
            buffer.WriteLong((long)ServerPackets.SRecipes);
            buffer.WriteArray(Globals.Recipes.ToArray());
            SendDataTo(index, buffer.ToArray());
            buffer.Dispose();
        }

        public static void SendGlobals(int index)
        {
            var buffer = new ByteBuffer();
            buffer.WriteLong((long)ServerPackets.SGlobals);
            buffer.WriteInteger(index);
            buffer.WriteInteger(Constants.LVL_BASE);
            buffer.WriteInteger(Constants.MAX_LEVEL);
            buffer.WriteFloat(Constants.LVL_EXPONENT);
            buffer.WriteInteger(Constants.MAX_STACK);
            SendDataTo(index, buffer.ToArray());
            buffer.Dispose();
        }

        public static void SendItems(int index)
        {
            var buffer = new ByteBuffer();
            buffer.WriteLong((long)ServerPackets.SItems);
            buffer.WriteArray(Globals.Items.ToArray());
            SendDataTo(index, buffer.ToArray());
            buffer.Dispose();
        }

        public static void SendSystemByte(int connectionId, SystemBytes sysByte)
        {
            var buffer = new ByteBuffer();
            buffer.WriteLong((long)ServerPackets.SSystemByte);
            buffer.WriteLong((long)sysByte);
            SendDataTo(connectionId, buffer.ToArray());
            buffer.Dispose();
        }
        #endregion

        public static byte[] Compress(byte[] bytes)
        {
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionLevel.Optimal))
                {
                    msi.CopyTo(gs);
                }
                return mso.ToArray();
            }
        }
    }
}

