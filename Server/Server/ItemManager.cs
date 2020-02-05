using System;
using System.Linq;
using System.Security;
using Bindings;
using DMod.Models;

namespace Server
{
    public class ItemManager
    {
        public static void Equip(long connectionID, int from, int to)
        {
            var player = Program._userService.ActiveUsers.FirstOrDefault(p => p.Id == Types.PlayerIds[connectionID]);
            if (player == null) return;
            var inv = player.Inventory.First(i => i.Slot == from);
            if (to >= 100)
            {
                inv.Slot = to;
            }
            else
            {
                var item = Globals.Items.First(i => i.Id == inv.ItemId);
                if (item.Slot == to)
                {
                    inv.Slot = to;
                }
                // Check special cases
                // Check for weapon
                if (to >= 4 && to <= 8 && item.Slot == 22)
                {
                    inv.Slot = to;
                }
                // Check for muns
                if (to >= 9 && to <= 11 && item.Slot == 23)
                {
                    inv.Slot = to;
                }
                // Check for armor
                if (to >= 13 && to <= 20 && item.Slot == 21)
                {
                    inv.Slot = to;
                }
            }
            ServerTCP.SendInventory((int)connectionID);
        }

        public static void AddToInventory(User player, string itemID, int qty)
        {
            var item = Globals.Items.FirstOrDefault(it => it.Id == itemID);
            if (item == null) return;
            // Check if user already has it
            var slot = player.Inventory.FirstOrDefault(s => s.ItemId == itemID);
            if (slot != null)
            {
                if (item.Stack)
                {
                    AddExisting(slot, player, itemID, qty);
                }
                else
                {
                    AddNew(player, itemID, qty);
                }
            }
            else
            {
                AddNew(player, itemID, qty);
            }
            ServerTCP.SendInventory(Array.IndexOf(Types.PlayerIds, player.Id));
        }

        private static void AddExisting(Inventory slot, User player, string itemID, int qty)
        {
            if (slot.Quantity < 250 - qty)
            {
                slot.Quantity += qty;
            }
            else
            {
                var remainder = qty - (250 - slot.Quantity);
                slot.Quantity = 250;
                AddNew(player, itemID, remainder);
            }
        }

        private static void AddNew(User player, string itemID, int qty)
        {
            var openSlot = FindOpenSlot(player);
            if (openSlot != -1)
            {
                var newItem = new Inventory { ItemId = itemID, Dropped = DateTime.Now, Id = Guid.NewGuid().ToString(), Quantity = qty, Slot = openSlot, UserId = player.Id, X = 0f, Y = 0f };
                player.Inventory.Add(newItem);
            }
        }

        public static int FindOpenSlot(User player)
        {
            for (var i = 100; i < 160; i++)
            {
                var open = player.Inventory.FirstOrDefault(s => s.Slot == i);
                if (open == default(Inventory)) return i;
            }
            return -1;
        }
    }
}
