using System;

namespace Data.Models
{
    public class Inventory
    {
        // Classification information
        public string Id { get; set; }
        public string UserId { get; set; }
        public string ItemId { get; set; }
        public int Slot { get; set; }

        // General
        public int Quantity { get; set; }

        // Global dropped inventory data
        public DateTime Dropped { get; set; }
        public float X { get; set; }
        public float Y { get; set; }

        // Empty     0
        // Hull      1
        // Drive     2
        // Computer  3
        // Payload   4
        // Shield    5
        // Armor1-8  6
        // Weapon1   7
        // Weapon2   8
        // Weapon3   9
        // Weapon4   10
        // Weapon5   11
        // Ammo1     12
        // Ammo2     13
        // Ammo3     14
        // Fuel      15
        // Armor1    16
        // Armor2    17
        // Armor3    18
        // Armor4    19
        // Armor5    20
        // Armor6    21
        // Armor7    22
        // Armor8    23
        // General   101-160
    }
}
