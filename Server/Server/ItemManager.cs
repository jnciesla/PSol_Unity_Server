using System;
using System.Linq;
using Data;
using DMod.Models;

namespace Server
{
    public class ItemManager
    {
        public static void Equip(long connectionId, int from, int to)
        {
            var player = Program._userService.ActiveUsers.FirstOrDefault(p => p.Id == Globals.PlayerIds[connectionId]);
            if (player == null) return;
            var inv = player.Inventory.First(i => i.Slot == from);
            var newInv = player.Inventory.FirstOrDefault(i => i.Slot == to);
            if (to >= 100)
            {
                if (newInv == null)
                {
                    inv.Slot = to;
                }
                else
                {
                    if (inv.ItemId == newInv.ItemId)
                    {
                        AddExisting(newInv, player, inv.ItemId, inv.Quantity);
                    }
                }
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
                    switch (to)
                    {
                        case 4:
                            player.Weap1ChargeRate = item.Recharge;
                            break;
                        case 5:
                            player.Weap2ChargeRate = item.Recharge;
                            break;
                        case 6:
                            player.Weap3ChargeRate = item.Recharge;
                            break;
                        case 7:
                            player.Weap4ChargeRate = item.Recharge;
                            break;
                        case 8:
                            player.Weap5ChargeRate = item.Recharge;
                            break;
                    }
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
            ServerTcp.SendInventory((int)connectionId);
        }

        public static void AddToInventory(User player, string itemId, int qty)
        {
            var item = Globals.Items.FirstOrDefault(it => it.Id == itemId);
            if (item == null) return;
            // Check if user already has it
            var slot = player.Inventory.FirstOrDefault(s => s.ItemId == itemId);
            if (slot != null)
            {
                if (item.Stack)
                {
                    AddExisting(slot, player, itemId, qty);
                }
                else
                {
                    AddNew(player, itemId, qty);
                }
            }
            else
            {
                AddNew(player, itemId, qty);
            }
            ServerTcp.SendInventory(Array.IndexOf(Globals.PlayerIds, player.Id));
        }

        private static void AddExisting(Inventory slot, User player, string itemId, int qty)
        {
            if (slot.Quantity < Constants.MAX_STACK - qty)
            {
                slot.Quantity += qty;
            }
            else
            {
                var remainder = qty - (Constants.MAX_STACK - slot.Quantity);
                slot.Quantity = Constants.MAX_STACK;
                AddNew(player, itemId, remainder);
            }
        }

        private static void AddNew(User player, string itemId, int qty)
        {
            var openSlot = FindOpenSlot(player);
            if (openSlot != -1)
            {
                var newItem = new Inventory { ItemId = itemId, Dropped = DateTime.Now, Id = Guid.NewGuid().ToString(), Quantity = qty, Slot = openSlot, UserId = player.Id, X = 0f, Y = 0f };
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

        public static void CollectLoot(long connectionId, string lootId, int slot)
        {
            var player =
                Program._userService.ActiveUsers.FirstOrDefault(p => p.Id == Globals.PlayerIds[connectionId]);
            if (player == null) return;
            var loot = Globals.Loot.FirstOrDefault(l => l.Id == lootId);
            if (loot == null) return;
            if (slot == -23)
            {
                player.Credits += loot.Credits;
                loot.Credits = 0;
            }
            else
            {
                var item = loot.Items.FirstOrDefault(i => i.Slot == slot);
                if (item == null) return;
                var qty = item.Quantity;
                loot.Items.Remove(item);
                AddToInventory(player, item.ItemId, qty);
            }

            if (loot.Items.Count < 1 && loot.Credits < 1)
            {
                Globals.Loot.Remove(loot);
            }
        }

        public static void PurchaseItem(long connectionId, string shopId, int slot, int qty)
        {
            var player = Program._userService.ActiveUsers.FirstOrDefault(p => p.Id == Globals.PlayerIds[connectionId]);
        }

        public static void SellItem(long connectionId, int slot, int qty)
        {
            var player = Program._userService.ActiveUsers.FirstOrDefault(p => p.Id == Globals.PlayerIds[connectionId]);
            var inv = player?.Inventory.FirstOrDefault(i => i.Slot == slot);
            if (inv == null) return;
            var itm = Globals.Items.FirstOrDefault(i => i.Id == inv.ItemId);
            if (itm == null) return;
            if (inv.Quantity > qty)
            {
                player.Credits += itm.Cost * qty;
                inv.Quantity -= qty;
            }
            else if (inv.Quantity <= qty)
            {
                player.Credits += itm.Cost * inv.Quantity;
                player.Inventory.Remove(inv);
            }
            ServerTcp.SendInventory(Array.IndexOf(Globals.PlayerIds, player.Id));
        }

        public static void ManufactureItem(long connectionId, string[] invIDs, int qty)
        {
            var player = Program._userService.ActiveUsers.FirstOrDefault(p => p.Id == Globals.PlayerIds[connectionId]);
            var preHash = "";
            for (var i = 0; i < 8; i++)
            {
                var input = "";
                if (invIDs[i] == "empty")
                {
                    input = "empty";
                }
                else
                {
                    var item = player.Inventory.FirstOrDefault(v => v.Id == invIDs[i]);
                    if (item == null) return;
                    input = item.ItemId;
                }
                preHash += input;
            }
            var hash = General.CalculateMD5Hash(preHash);
            if (Globals.RecipeHash.ContainsKey(hash))
            {
                Globals.RecipeHash.TryGetValue(hash, out var output);
                var recipe = Globals.Recipes.First(r => r.ID == output);
                var item = Globals.Items.First(m => m.Id == recipe.Output);
                AddToInventory(player, recipe.Output, qty);
            }
        }
    }
}
