using System.Collections.Generic;
using Data.Models;

namespace Server
{
    internal class Globals
    {
        /// <summary>
        /// "Static" data has changed.  Set this bool to broadcast data not in normal broadcast
        /// </summary>
        public static bool FullData;

        public static ICollection<Star> Galaxy;
        public static ICollection<Item> Items;
        public static ICollection<Inventory> Inventory = new List<Inventory>();
        public static List<Loot> Loot = new List<Loot>();
        public static List<Nebula> Nebulae = new List<Nebula>();
    }
}
