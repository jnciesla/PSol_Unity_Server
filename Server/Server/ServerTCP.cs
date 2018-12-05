using System;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    internal class ServerTCP
    {
        private static TcpListener serverSocket;
        public static Clients[] Client = new Clients[Constants.MAX_PLAYERS];

        public static void InitializeNetwork()
        {
            serverSocket = new TcpListener(IPAddress.Any, 5555);
            serverSocket.Start();
            serverSocket.BeginAcceptTcpClient(OnClientConnect, null);
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
                    General.JoinGame(i);
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
                    //Console.WriteLine("Sending to " + Client[i].ip + " | ConnectionID: " + Client[i].connectionID);
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

        public static void SendMessage(int connectionID, string message)
        {
            var buffer = new ByteBuffer();
            buffer.WriteLong((long)ServerPackets.SMessage);
            buffer.WriteString(message);
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
        #endregion
    }
}

