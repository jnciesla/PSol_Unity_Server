using System;
using System.Collections.Generic;
using System.Linq;
using Bindings;
using Data.Services;

namespace Server
{
    public class ServerHandleData
    {
        private delegate void Packet_(long connectionID, byte[] data);

        private static Dictionary<long, Packet_> packets;
        private static long pLength;

        public static void InitializePackets()
        {
            Cnsl.Debug("Initializing Network Packets", true);
            packets = new Dictionary<long, Packet_>
            {
                { (long)ClientPackets.CMovement, PACKET_CLIENTMOVEMENT },
                { (long)ClientPackets.CMessage, PACKET_CLIENTMESSAGE },
                { (long)ClientPackets.CLogin, PACKET_LOGIN },
                { (long)ClientPackets.CEquip, PACKET_EQUIP },
                { (long)ClientPackets.CAttack, PACKET_ATTACK }
            };
            Cnsl.Finalize("Initializing Network Packets");
        }

        public static void HandleData(long connectionID, byte[] data)
        {
            var Buffer = (byte[])data.Clone();

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

        private static void HandleDataPackets(long connectionID, byte[] data)
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

        private static void PACKET_CLIENTMOVEMENT(long connectionID, byte[] data)
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
            player.Heading = Y;
            player.Roll = Z;
            player.M = M;
        }

        private static void PACKET_CLIENTMESSAGE(long connectionID, byte[] data)
        {
            var buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            buffer.ReadLong();
            var msg = buffer.ReadString();
            if (msg.ToLower().StartsWith("/c"))
            {
                var player = Program._userService.ActiveUsers.Find(p => p.Id == Types.PlayerIds[connectionID]);
                var newString = player.Name + ": " + msg.Substring(3);
                ServerTCP.SendMessage(-1, newString, (int)ChatPackets.Chat);
            }
            // Admin commands
            if (msg.StartsWith("*"))
            {
                Admin.HandleCommand(connectionID, msg);
            }
            buffer.Dispose();
        }

        private static void PACKET_LOGIN(long connectionID, byte[] data)
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
            Types.PlayerIds[connectionID] = player.Id;
            Program._userService.ActiveUsers.Add(player);
            ServerTCP.SendToGalaxy((int)connectionID);
            System.Threading.Thread.Sleep(250);
            ServerTCP.SendIngame((int)connectionID);
            System.Threading.Thread.Sleep(250);
            ServerTCP.SendItems((int)connectionID);
            //ServerTCP.SendNebulae(connectionID);
            ServerTCP.SendMessage(-1, player.Name + " has connected.", (int)ChatPackets.Notification);
            System.Threading.Thread.Sleep(250);
            ServerTCP.SendGalaxy((int)connectionID);
            Globals.FullData = true;
            Cnsl.Log(user + @" logged in successfully.");
            player.receiving = true;
        }

        private static void PACKET_EQUIP(long connectionID, byte[] data)
        {
            var buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            buffer.ReadLong();
            var from = buffer.ReadInteger();
            var to = buffer.ReadInteger();
            buffer.Dispose();
            ItemManager.Equip(connectionID, from, to);
        }

        private static void PACKET_ATTACK(long connectionID, byte[] data)
        {
            var buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            buffer.ReadLong();
            var targetID = buffer.ReadString();
            var weaponSlot = buffer.ReadInteger();
            buffer.Dispose();
            var player = Program._userService.ActiveUsers.FirstOrDefault(p => p.Id == Types.PlayerIds[connectionID]);
            if (player == null) return;
            var weaponID = player.Inventory.First(w => w.Slot == weaponSlot).ItemId;
            var weapon = Globals.Items.First(w => w.Id == weaponID);
            Program._mobService.DoAttack(targetID, player.Id, weapon);
        }
        #endregion
    }
}