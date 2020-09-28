using System.Collections.Generic;

namespace DMod.Models
{
    public class Structure
    {
        public string ID { get; set; }

        // General
        public string Name { get; set; }
        public int Model { get; set; }
        public int Type { get; set; }

        // Inventory
        public virtual ICollection<Inventory> Inventory { get; set; }

        // Position
        public float X { get; set; }
        public float Y { get; set; }
    }
}
