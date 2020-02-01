using System;
using Bindings;

namespace Server
{
    public class Admin
    {
        public static void HandleCommand(long connectionID, string msg)
        {
            if (msg.ToLower().StartsWith("*goto"))
            {
                var player = Program._userService.ActiveUsers.Find(p => p.Id == Types.PlayerIds[connectionID]);
                var dest = msg.Split(new[] { ' ' }, 2);
                var coords = dest[1].Split(',');
                var testx = int.TryParse(coords[0].Trim(), out var x);
                var testy = int.TryParse(coords[1].Trim(), out var y);
                if (!testx || !testy || x > 100000 || x < 0 || y > 100000 || y < 0)
                {
                    ServerTCP.SendMessage((int)connectionID, "Invalid coordinates", (int)ChatPackets.Error);
                }
                else
                {
                    player.X = x;
                    player.Z = y;
                    ServerTCP.SendIngame((int)connectionID, 1);
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
                        ServerTCP.SendMessage((int)connectionID, "User does not exist", (int)ChatPackets.Error);
                        return;
                    }
                    player.Exp += int.Parse(part[3]);
                    ServerTCP.SendIngame((int)connectionID, 1);
                }

                if (msg.ToLower().Contains(" item "))
                {
                    var part = msg.Split(new[] { ' ' }, 4);
                    var player = Program._userService.ActiveUsers.Find(p =>
                        string.Equals(p.Name, part[1], StringComparison.CurrentCultureIgnoreCase));
                    if (player == null)
                    {
                        ServerTCP.SendMessage((int)connectionID, "User does not exist", (int)ChatPackets.Error);
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
                        ServerTCP.SendMessage((int)connectionID, "User does not exist", (int)ChatPackets.Error);
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
                        ServerTCP.SendMessage((int)connectionID, "User does not exist", (int)ChatPackets.Error);
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
                    }
                }
                else
                {
                    var player = Program._userService.ActiveUsers.Find(p =>
                        string.Equals(p.Name, part[1], StringComparison.CurrentCultureIgnoreCase));
                    if (player == null)
                    {
                        ServerTCP.SendMessage((int) connectionID, "User does not exist", (int) ChatPackets.Error);
                        return;
                    }

                    player.Health = player.MaxHealth;
                    player.Shield = player.MaxShield;
                }
            }
        }
    }
}
