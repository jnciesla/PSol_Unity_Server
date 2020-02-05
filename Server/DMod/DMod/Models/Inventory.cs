using System;

namespace DMod.Models
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

        // PDS (Propulsion drive system)    00
        // FCA (Flight computer assembly)   01
        // APM (Auxiliary payload module)   02
        // SGU (Shield generator unit)      03
        // Weapon 1                         04
        // Weapon 2                         05
        // Weapon 3                         06
        // Weapon 4                         07
        // Weapon 5                         08
        // Munitions 1                      09
        // Munitions 2                      10
        // Munitions 3                      11
        // Fuel                             12
        // Armor 1                          13
        // Armor 2                          14
        // Armor 3                          15
        // Armor 4                          16
        // Armor 5                          17
        // Armor 6                          18
        // Armor 7                          19
        // Armor 8                          20
        // Armor general                    21
        // Weapon general                   22
        // Munitions general                23
        // Hull                             24
        // General   101-160
    }
}
