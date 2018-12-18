using System;
using System.Collections.Generic;
using System.Linq;
using Bindings;

namespace Server
{
    public class ServerHandleData
    {
        private delegate void Packet_(long connectionID, byte[] data);
        static Dictionary<long, Packet_> packets;
        static long pLength;

        public static void InitializePackets()
        {
            Cnsl.Debug("Initializing Network Packets...");
            packets = new Dictionary<long, Packet_>
            {
                { (long)ClientPackets.CThankYouMessage, PACKET_THANKYOUMESSAGE },
                { (long)ClientPackets.CMovement, PACKET_CLIENTMOVEMENT },
                { (long)ClientPackets.CMessage, PACKET_CLIENTMESSAGE },
                { (long)ClientPackets.CLogin, PACKET_LOGIN },
            };
        }

        public static void HandleData(long connectionID, byte[] data)
        {
            byte[] Buffer;
            Buffer = (byte[])data.Clone();

            if (ServerTCP.Client[connectionID].playerBuffer == null)
            {
                ServerTCP.Client[connectionID].playerBuffer = new ByteBuffer();
            }
            ServerTCP.Client[connectionID].playerBuffer.WriteBytes(Buffer);

            if (ServerTCP.Client[connectionID].playerBuffer.Count() == 0)
            {
                ServerTCP.Client[connectionID].playerBuffer.Clear();
                return;
            }

            if (ServerTCP.Client[connectionID].playerBuffer.Length() >= 8)
            {
                pLength = ServerTCP.Client[connectionID].playerBuffer.ReadLong(false);
                if (pLength <= 0)
                {
                    ServerTCP.Client[connectionID].playerBuffer.Clear();
                    return;
                }
            }

            while (pLength > 0 & pLength <= ServerTCP.Client[connectionID].playerBuffer.Length() - 8)
            {
                if (pLength <= ServerTCP.Client[connectionID].playerBuffer.Length() - 8)
                {
                    ServerTCP.Client[connectionID].playerBuffer.ReadLong();
                    data = ServerTCP.Client[connectionID].playerBuffer.ReadBytes((int)pLength);
                    HandleDataPackets(connectionID, data);
                }

                pLength = 0;

                if (ServerTCP.Client[connectionID].playerBuffer.Length() >= 8)
                {
                    pLength = ServerTCP.Client[connectionID].playerBuffer.ReadLong(false);

                    if (pLength < 0)
                    {
                        ServerTCP.Client[connectionID].playerBuffer.Clear();
                        return;
                    }
                }
            }
        }
        static void HandleDataPackets(long connectionID, byte[] data)
        {
            var buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            var packetIdentifier = buffer.ReadLong();
            buffer.Dispose();

            if (packets.TryGetValue(packetIdentifier, out var packet))
            {
                packet.Invoke(connectionID, data);
            }
        }
        #region Handle packets
        static void PACKET_THANKYOUMESSAGE(long connectionID, byte[] data)
        {
            var buffer = new ByteBuffer();
            buffer.WriteBytes(data);

            buffer.ReadLong();
            var msg = buffer.ReadString();

            Console.WriteLine(msg);

            buffer.Dispose();
        }

        static void PACKET_CLIENTMOVEMENT(long connectionID, byte[] data)
        {
            var buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            buffer.ReadLong();
            var x = buffer.ReadFloat();     // Latitude
            var z = buffer.ReadFloat();     // Longitude
            var Y = buffer.ReadFloat();     // Heading
            var Z = buffer.ReadFloat();     // Roll
            var M = buffer.ReadInteger();   // Moving
            buffer.Dispose();
            var player = Program._userService.ActiveUsers.FirstOrDefault(p => p.Id == Types.PlayerIds[connectionID]);
            if (player == null) return;
            player.X = x;
            player.Z = z;
            player.Roll = Y;
            player.Heading = Z;
            player.M = M;
        }

        static void PACKET_CLIENTMESSAGE(long connectionID, byte[] data)
        {
            var buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            buffer.ReadLong();
            var msg = buffer.ReadString();
            ServerTCP.SendMessage(-1, msg, (int)ChatPackets.Chat);
            buffer.Dispose();
        }

        static void PACKET_LOGIN(long connectionID, byte[] data)
        {
            var buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            buffer.ReadLong();
            var user = buffer.ReadString();
            var pass = buffer.ReadString();
            var validate = Program._userService.PasswordOK(user, pass);
            if (!validate)
            {
                ServerTCP.SendSystemByte((int)connectionID, SystemBytes.SysInvPass);
                return;
            }
            buffer.Dispose();

            var player = Program._userService.LoadPlayer(user);
            player.inGame = true;
            player.receiving = true;
            Types.PlayerIds[connectionID] = player.Id;
            Program._userService.ActiveUsers.Add(player);
            General.JoinGame((int)connectionID);
            //ServerTCP.SendLoad(connectionID);
            //ServerTCP.SendGalaxy(connectionID);
            //ServerTCP.SendItems(connectionID);
            //ServerTCP.SendNebulae(connectionID);
            ServerTCP.SendMessage(-1, player.Name + " has connected.", (int)ChatPackets.Notification);
            Globals.FullData = true;
            Cnsl.Log(user + @" logged in successfully.");
        }
        #endregion
    }
}
