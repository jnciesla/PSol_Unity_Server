using System;
using System.Linq;
using Data;

namespace Server
{
    public class Admin
    {
        public static void HandleCommand(long connectionID, string msg)
        {
            if (msg.ToLower().StartsWith("*goto"))
            {
                var player = Program._userService.ActiveUsers.Find(p => p.Id == Globals.PlayerIds[connectionID]);
                var dest = msg.Split(new[] { ' ' }, 2);
                var coords = dest[1].Split(',');
                var testx = int.TryParse(coords[0].Trim(), out var x);
                var testy = int.TryParse(coords[1].Trim(), out var y);
                if (!testx || !testy || x > 100000 || x < 0 || y > 100000 || y < 0)
                {
                    ServerTcp.SendMessage((int)connectionID, "Invalid coordinates", (int)ChatPackets.Error);
                }
                else
                {
                    player.X = x;
                    player.Z = y;
                    ServerTcp.SendIngame((int)connectionID, 1);
                }
            }

            if (msg.ToLower().StartsWith("*savegame"))
            {
                Program.SaveGame();
            }

            if (msg.ToLower().StartsWith("*repop"))
            {
                Program._mobService.RepopGalaxy();
            }

            if (msg.ToLower().StartsWith("*give"))
            {
                if (msg.ToLower().Contains(" xp "))
                {
                    var part = msg.Split(new[] { ' ' }, 4);
                    var player = Program._userService.ActiveUsers.Find(p => string.Equals(p.Name, part[1], StringComparison.CurrentCultureIgnoreCase));
                    if (player == null)
                    {
                        ServerTcp.SendMessage((int)connectionID, "User does not exist", (int)ChatPackets.Error);
                        return;
                    }
                    player.Exp += int.Parse(part[3]);
                    ServerTcp.SendIngame((int)connectionID, 1);
                }

                if (msg.ToLower().Contains(" item "))
                {
                    var part = msg.Split(new[] { ' ' }, 4);
                    var player = Program._userService.ActiveUsers.Find(p =>
                        string.Equals(p.Name, part[1], StringComparison.CurrentCultureIgnoreCase));
                    if (player == null)
                    {
                        ServerTcp.SendMessage((int)connectionID, "User does not exist", (int)ChatPackets.Error);
                        return;
                    }
                    ItemManager.AddToInventory(player, part[3], int.Parse(part[2]));
                }

                if (msg.ToLower().Contains(" $"))
                {
                    var part = msg.Split(new[] { ' ' }, 3);
                    var player = Program._userService.ActiveUsers.Find(p =>
                        string.Equals(p.Name, part[1], StringComparison.CurrentCultureIgnoreCase));
                    if (player == null)
                    {
                        ServerTcp.SendMessage((int)connectionID, "User does not exist", (int)ChatPackets.Error);
                        return;
                    }
                    player.Credits += int.Parse(part[2].Substring(1));
                }

                if (msg.ToLower().Contains(" lvl "))
                {
                    var part = msg.Split(new[] { ' ' }, 4);
                    var player = Program._userService.ActiveUsers.Find(p =>
                        string.Equals(p.Name, part[1], StringComparison.CurrentCultureIgnoreCase));
                    if (player == null)
                    {
                        ServerTcp.SendMessage((int)connectionID, "User does not exist", (int)ChatPackets.Error);
                        return;
                    }

                    player.Level = int.Parse(part[3]);
                }
            }

            if (msg.ToLower().StartsWith("*heal"))
            {
                var part = msg.Split(new[] { ' ' }, 2);
                if (part[1] == "all")
                {
                    foreach (var player in Program._userService.ActiveUsers)
                    {
                        player.Health = player.MaxHealth;
                        player.Shield = player.MaxShield;
                        player.Weap1Charge = 100;
                        player.Weap2Charge = 100;
                        player.Weap3Charge = 100;
                        player.Weap4Charge = 100;
                        player.Weap5Charge = 100;
                    }
                }
                else
                {
                    var player = Program._userService.ActiveUsers.Find(p =>
                        string.Equals(p.Name, part[1], StringComparison.CurrentCultureIgnoreCase));
                    if (player == null)
                    {
                        ServerTcp.SendMessage((int)connectionID, "User does not exist", (int)ChatPackets.Error);
                        return;
                    }

                    player.Health = player.MaxHealth;
                    player.Shield = player.MaxShield;
                    player.Weap1Charge = 100;
                    player.Weap2Charge = 100;
                    player.Weap3Charge = 100;
                    player.Weap4Charge = 100;
                    player.Weap5Charge = 100;
                }
            }

            if (msg.ToLower().StartsWith("*destroy"))
            {
                var player = Program._userService.ActiveUsers.Find(p => p.Id == Globals.PlayerIds[connectionID]);
                var part = msg.Split(new[] { ' ' }, 2);
                var item = Globals.Items.First(i => i.Id == "6860e473-11f1-44c7-bcb3-9e2768b5bcbb");
                var mob = Program._mobService.GetMob(part[1]);
                Program._mobService.DoAttack(part[1], player.Id, item);
                ServerTcp.SendMessage(-1, player.Name + " has dealt " + mob.Name + " " + item.Damage + " damage with an " + item.Name, (int)ChatPackets.Notification);
            }
        }
    }
}
