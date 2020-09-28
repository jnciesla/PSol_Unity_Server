using System.Collections.Generic;
using System.Linq;
using DMod.Models;
using Data;
using Data.Services;

namespace Server
{
    public class ServerHandleData
    {
        private delegate void Packet(long connectionId, byte[] data);

        private static Dictionary<long, Packet> packets;
        private static long pLength;

        public static void InitializePackets()
        {
            Cnsl.Debug("Initializing Network Packets", true);
            packets = new Dictionary<long, Packet>
            {
                { (long)ClientPackets.CMovement, PACKET_CLIENTMOVEMENT },
                { (long)ClientPackets.CMessage, PACKET_CLIENTMESSAGE },
                { (long)ClientPackets.CLogin, PACKET_LOGIN },
                { (long)ClientPackets.CEquip, PACKET_EQUIP },
                { (long)ClientPackets.CAttack, PACKET_ATTACK },
                { (long)ClientPackets.CLoot, PACKET_LOOT },
                { (long)ClientPackets.CShopBuy, PACKET_PURCHASE },
                { (long)ClientPackets.CShopSell, PACKET_SELL },
                { (long)ClientPackets.CDischarge, PACKET_DISCHARGE },
                { (long)ClientPackets.CLog, PACKET_LOGERROR },
                { (long)ClientPackets.CManufacture, PACKET_MANUFACTURE },

            };
            Cnsl.Finalize("Initializing Network Packets");
        }

        public static void HandleData(long connectionId, byte[] data)
        {
            var buffer = (byte[])data.Clone();

            if (ServerTcp.Client[connectionId].playerBuffer == null)
            {
                ServerTcp.Client[connectionId].playerBuffer = new ByteBuffer();
            }
            ServerTcp.Client[connectionId].playerBuffer.WriteBytes(buffer);

            if (ServerTcp.Client[connectionId].playerBuffer.Count() == 0)
            {
                ServerTcp.Client[connectionId].playerBuffer.Clear();
                return;
            }

            if (ServerTcp.Client[connectionId].playerBuffer.Length() >= 8)
            {
                pLength = ServerTcp.Client[connectionId].playerBuffer.ReadLong(false);
                if (pLength <= 0)
                {
                    ServerTcp.Client[connectionId].playerBuffer.Clear();
                    return;
                }
            }

            while (pLength > 0 & pLength <= ServerTcp.Client[connectionId].playerBuffer.Length() - 8)
            {
                if (pLength <= ServerTcp.Client[connectionId].playerBuffer.Length() - 8)
                {
                    ServerTcp.Client[connectionId].playerBuffer.ReadLong();
                    data = ServerTcp.Client[connectionId].playerBuffer.ReadBytes((int)pLength);
                    HandleDataPackets(connectionId, data);
                }

                pLength = 0;

                if (ServerTcp.Client[connectionId].playerBuffer.Length() >= 8)
                {
                    pLength = ServerTcp.Client[connectionId].playerBuffer.ReadLong(false);

                    if (pLength < 0)
                    {
                        ServerTcp.Client[connectionId].playerBuffer.Clear();
                        return;
                    }
                }
            }
        }

        private static void HandleDataPackets(long connectionId, byte[] data)
        {
            var buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            var packetIdentifier = buffer.ReadLong();
            buffer.Dispose();

            if (packets.TryGetValue(packetIdentifier, out var packet))
            {
                packet.Invoke(connectionId, data);
            }
        }
        #region Handle packets

        private static void PACKET_CLIENTMOVEMENT(long connectionId, byte[] data)
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
            var player = Program._userService.ActiveUsers.FirstOrDefault(p => p.Id == Globals.PlayerIds[connectionId]);
            if (player == null) return;
            player.X = x;
            player.Z = z;
            player.Heading = Y;
            player.Roll = Z;
            player.M = M;
        }

        private static void PACKET_CLIENTMESSAGE(long connectionId, byte[] data)
        {
            var buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            buffer.ReadLong();
            var msg = buffer.ReadString();
            if (msg.ToLower().StartsWith("/c"))
            {
                var player = Program._userService.ActiveUsers.Find(p => p.Id == Globals.PlayerIds[connectionId]);
                var newString = player.Name + ": " + msg.Substring(3);
                ServerTcp.SendMessage(-1, newString, (int)ChatPackets.Chat);
            }
            // Admin commands
            if (msg.StartsWith("*"))
            {
                Admin.HandleCommand(connectionId, msg);
            }
            buffer.Dispose();
        }

        private static void PACKET_LOGIN(long connectionId, byte[] data)
        {
            var buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            buffer.ReadLong();
            var user = buffer.ReadString();
            var pass = buffer.ReadString();
            var validate = Program._userService.PasswordOK(user, pass);
            if (!validate)
            {
                ServerTcp.SendSystemByte((int)connectionId, SystemBytes.SysInvPass);
                return;
            }
            buffer.Dispose();

            var player = Program._userService.LoadPlayer(user);
            player.inGame = true;
            Globals.PlayerIds[connectionId] = player.Id;
            Program._userService.ActiveUsers.Add(player);
            ServerTcp.SendGlobals((int)connectionId);
            System.Threading.Thread.Sleep(250);
            ServerTcp.SendItems((int)connectionId);
            System.Threading.Thread.Sleep(250);
            ServerTcp.SendToGalaxy((int)connectionId);
            System.Threading.Thread.Sleep(250);
            ServerTcp.SendIngame((int)connectionId);
            System.Threading.Thread.Sleep(250);
            ServerTcp.SendInventory((int)connectionId);
            //ServerTCP.SendNebulae(connectionID);
            ServerTcp.SendMessage(-1, player.Name + " has connected.", (int)ChatPackets.Notification);
            System.Threading.Thread.Sleep(250);
            ServerTcp.SendGalaxy((int)connectionId);
            System.Threading.Thread.Sleep(250);
            ServerTcp.SendRecipes((int)connectionId);
            Globals.FullData = true;
            Cnsl.Log(user + @" logged in successfully.");
            player.receiving = true;
        }

        private static void PACKET_EQUIP(long connectionId, byte[] data)
        {
            var buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            buffer.ReadLong();
            var from = buffer.ReadInteger();
            var to = buffer.ReadInteger();
            buffer.Dispose();
            ItemManager.Equip(connectionId, from, to);
        }

        private static void PACKET_LOOT(long connectionId, byte[] data)
        {
            var buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            buffer.ReadLong();
            var lootId = buffer.ReadString();
            var slot = buffer.ReadInteger();
            buffer.Dispose();
            ItemManager.CollectLoot(connectionId, lootId, slot);
        }

        private static void PACKET_PURCHASE(long connectionId, byte[] data)
        {
            var buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            buffer.ReadLong();
            var shopId = buffer.ReadString();
            var slot = buffer.ReadInteger();
            var qty = buffer.ReadInteger();
            buffer.Dispose();
            ItemManager.PurchaseItem(connectionId, shopId, slot, qty);
        }

        private static void PACKET_SELL(long connectionId, byte[] data)
        {
            var buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            buffer.ReadLong();
            var slot = buffer.ReadInteger();
            var qty = buffer.ReadInteger();
            buffer.Dispose();
            ItemManager.SellItem(connectionId, slot, qty);
        }

        private static void PACKET_ATTACK(long connectionId, byte[] data)
        {
            var buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            buffer.ReadLong();
            var targetId = buffer.ReadString();
            var weaponSlot = buffer.ReadInteger();
            var ammo = buffer.ReadInteger();
            buffer.Dispose();
            var player = Program._userService.ActiveUsers.FirstOrDefault(p => p.Id == Globals.PlayerIds[connectionId]);
            if (player == null) return;
            var weaponId = player.Inventory.First(w => w.Slot == weaponSlot).ItemId;
            var weapon = Globals.Items.First(w => w.Id == weaponId);
            Item _ammo;
            if (ammo != -1)
            {
                var ammoId = player.Inventory.First(a => a.Slot == ammo).ItemId;
                _ammo = Globals.Items.First(a => a.Id == ammoId);
            } else
            {
                _ammo = new Item
                {
                    Id = new System.Guid().ToString(),
                    Slot = 23,
                    Damage = 0,
                    Type = "missile"
                };
            }
            Program._mobService.DoAttack(targetId, player.Id, weapon, _ammo);
            if (ammo == -1) return;
            player.Inventory.First(i => i.Slot == ammo).Quantity--;
        }

        private static void PACKET_DISCHARGE(long connectionId, byte[] data)
        {
            var buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            buffer.ReadLong();
            var slot = buffer.ReadInteger();
            buffer.Dispose();
            var player = Program._userService.ActiveUsers.FirstOrDefault(p => p.Id == Globals.PlayerIds[connectionId]);
            if (player == null) return;
            switch (slot)
            {
                case 4:
                    player.Weap1Charge = 0;
                    break;
                case 5:
                    player.Weap2Charge = 0;
                    break;
                case 6:
                    player.Weap3Charge = 0;
                    break;
                case 7:
                    player.Weap4Charge = 0;
                    break;
                case 8:
                    player.Weap5Charge = 0;
                    break;
            }
        }

        private static void PACKET_LOGERROR(long connectionId, byte[] data)
        {
            var buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            buffer.ReadLong();
            var message = buffer.ReadString();
            buffer.Dispose();
            Cnsl.Warn(message);
        }

        private static void PACKET_MANUFACTURE(long connectionId, byte[] data)
        {
            var buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            buffer.ReadLong();
            string[] invIDs = buffer.ReadList<string>().ToArray();
            var qty = buffer.ReadInteger();
            buffer.Dispose();
            ItemManager.ManufactureItem(connectionId, invIDs, qty);
        }
        #endregion
    }
}