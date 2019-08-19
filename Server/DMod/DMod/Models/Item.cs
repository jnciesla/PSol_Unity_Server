namespace DMod.Models
{
    public class Item
    {
        public string Id { get; set; }

        // General
        public string Name { get; set; }                // Name
        public string Type { get; set; }                // Type of item
        public string Description { get; set; }         // Brief description of the item
        public int Image { get; set; }                  // Image number
        public int Overlay { get; set; } = 0;           // Image number 2
        public string Color { get; set; } = "0";        // RRRGGGBBB.  Anything < 9 characters will result in no coloration
        public string OverlayColor { get; set; } = "0"; // RRRGGGBBB.  Anything < 9 characters will result in no coloration
        public int Mass { get; set; }                   // Weight
        public int Cost { get; set; }                   // Cost in credits
        public bool Stack { get; set; } = false;        // Stackable
        public int Level { get; set; } = 0;             // Minimum level to use/drop item
        public int Slot { get; set; } = 0;              // Slot item is equippable to.  Variable-slot items use first slot available (7, for weapons)
        public int Chance { get; set; } = 0;            // Chance to drop.  0 = not droppable.

        // Attributes
        public int Hull { get; set; } = 0;              // Health
        public int Shield { get; set; } = 0;            // Shield
        public int Armor { get; set; } = 0;             // Armor
        public int Thrust { get; set; } = 0;            // Thrust
        public int Power { get; set; } = 0;             // Power
        public int Damage { get; set; } = 0;            // Damage
        public int Recharge { get; set; } = 0;          // Amount of time to charge
        public int Repair { get; set; } = 0;            // Repair amount
        public int Defense { get; set; } = 0;           // Defense
        public int Offense { get; set; } = 0;           // Offense
        public int Capacity { get; set; } = 0;          // Weight capacity for hulls
        public int Weapons { get; set; } = 0;           // Weapons slots for hulls
        public int Special { get; set; } = 0;           // Special slot
    }
}
