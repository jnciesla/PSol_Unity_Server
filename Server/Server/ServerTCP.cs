using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Bindings;

namespace Server
{
    internal class ServerTCP
    {
        private static TcpListener serverSocket;
        public static Clients[] Client = new Clients[Constants.MAX_PLAYERS];

        public static void InitializeNetwork()
        {
            serverSocket = new TcpListener(IPAddress.Any, Constants.PORT);
            serverSocket.Start();
            serverSocket.BeginAcceptTcpClient(OnClientConnect, null);
            Console.Title = @"Project Sol Server | " + Constants.PORT;
        }
        static void OnClientConnect(IAsyncResult result)
        {
            var client = serverSocket.EndAcceptTcpClient(result);
            serverSocket.BeginAcceptTcpClient(OnClientConnect, null);
            for (var i = 0; i < Constants.MAX_PLAYERS; i++)
            {
                if (Client[i].socket == null)
                {
                    Client[i].socket = client;
                    Client[i].connectionID = i;
                    Client[i].ip = client.Client.RemoteEndPoint.ToString();
                    Client[i].Start();
                    Cnsl.Log("Connection received from " + Client[i].ip + " | ConnectionID: " + Client[i].connectionID);
                    return;
                }
            }
        }

        private static void SendDataTo(int connectionID, byte[] data)
        {
            var buffer = new ByteBuffer();
            buffer.WriteLong((data.GetUpperBound(0) - data.GetLowerBound(0)) + 1);
            buffer.WriteBytes(data);
            Client[connectionID].myStream.BeginWrite(buffer.ToArray(), 0, buffer.ToArray().Length, null, null);
            buffer.Dispose();
        }

        private static void SendDataToAll(byte[] data, int connectionID = -1)
        {
            for (var i = 0; i < Constants.MAX_PLAYERS; i++)
            {
                if (i == connectionID) continue;
                if (Client[i].socket != null)
                {
                    SendDataTo(i, data);
                }
            }
        }

        #region SendPackages
        public static void SendIngame(int connectionID)
        {
            var buffer = new ByteBuffer();
            buffer.WriteLong((long)ServerPackets.SIngame);
            buffer.WriteInteger(connectionID);
            SendDataTo(connectionID, buffer.ToArray());
            buffer.Dispose();
        }
        public static byte[] PlayerData(int connectionID)
        {
            var buffer = new ByteBuffer();
            buffer.WriteLong((long)ServerPackets.SPlayerData);
            buffer.WriteInteger(connectionID);
            return buffer.ToArray();
        }
        public static void SendToGalaxy(int connectionID)
        {
            for (var i = 0; i < Constants.MAX_PLAYERS; i++)
            {
                if (Client[i].socket != null)
                {
                    if (i != connectionID)
                    {
                        SendDataTo(connectionID, PlayerData(i));
                    }
                }
            }
            SendDataToAll(PlayerData(connectionID));
        }

        public static void PlayerMove(int connectionID, float x, float z, float Y, float Z, int M)
        {
            var buffer = new ByteBuffer();
            buffer.WriteLong((long)ServerPackets.SPlayerMove);
            buffer.WriteInteger(connectionID);
            buffer.WriteFloat(x);
            buffer.WriteFloat(z);
            buffer.WriteFloat(Y);
            buffer.WriteFloat(Z);
            buffer.WriteInteger(M);
            SendDataToAll(buffer.ToArray(), connectionID);
            buffer.Dispose();
        }

        public static void SendMessage(int connectionID, string message, int type)
        {
            var buffer = new ByteBuffer();
            buffer.WriteLong((long)ServerPackets.SMessage);
            buffer.WriteString(message);
            buffer.WriteInteger(type);
            if (connectionID < 0)
            {
                SendDataToAll(buffer.ToArray());
            }
            else
            {
                SendDataTo(connectionID, buffer.ToArray());
            }
            buffer.Dispose();
        }

        public static void PreparePulseBroadcast()
        {
            var look = 2000;
            //Program._combatService.CycleArrays();
            for (var i = 0; i < Constants.MAX_PLAYERS; i++)
            {
                if (Client[i].socket != null)
                {
                    var player = Program._userService.ActiveUsers.FirstOrDefault(p => p.Id == Types.PlayerIds[i]);
                    if (player == null) continue;
                    if (player.inGame && player.receiving)
                    {
                        var buffer = new ByteBuffer();
                        buffer.WriteLong((long)ServerPackets.SPulse);
                        buffer.WriteInteger(Program._userService.ActiveUsers.Count);
                        buffer.WriteBytes(BitConverter.GetBytes(DateTime.UtcNow.ToBinary()));
                        Program._userService.ActiveUsers.ForEach(p =>
                        {
                            var index = Array.IndexOf(Types.PlayerIds, p.Id);
                            buffer.WriteInteger(index);
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
                            buffer.WriteBytes(BitConverter.GetBytes(Program._userService.ActiveUsers[index].inGame));
                        });
                        var minX = (int)player.X - look;
                        var minY = (int)player.Z - look;
                        var maxX = (int)player.X + look;
                        var maxY = (int)player.Z + look;
                        //buffer.WriteArray(Program._mobService.GetMobs(minX, maxX, minY, maxY).ToArray());
                        //buffer.WriteArray(Program._combatService.GetCombats((int)player.X, (int)player.Z).ToArray());
                        //buffer.WriteArray(Globals.Inventory.Where(m => m.X >= minX && m.X <= maxX && m.Y >= minY && m.Y <= maxY).ToArray());
                        //buffer.WriteArray(Globals.Loot.Where(L => L.X >= minX && L.X <= maxX && L.Y >= minY && L.Y <= maxY && L.Owner == player.Id).ToArray());
                        SendDataTo(i, buffer.ToArray());
                        buffer.Dispose();
                    }
                }
            }
        }

        public static void SendSystemByte(int connectionID, SystemBytes sysByte)
        {
            var buffer = new ByteBuffer();
            buffer.WriteLong((long)ServerPackets.SSystemByte);
            buffer.WriteLong((long)sysByte);
            SendDataTo(connectionID, buffer.ToArray());
            buffer.Dispose();
        }
        #endregion
    }
}

