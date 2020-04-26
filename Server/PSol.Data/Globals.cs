using System.Collections.Generic;
using Bindings;
using DMod.Models;

namespace Data
{
    public class Globals
    {
        /// <summary>
        /// "Static" data has changed.  Set this bool to broadcast data not in normal broadcast
        /// </summary>
        public static bool FullData;
        public static bool Initialized = false;

        public static ICollection<Star> Galaxy;
        public static ICollection<Item> Items;
        public static ICollection<Inventory> Inventory = new List<Inventory>();
        public static List<Nebula> Nebulae = new List<Nebula>();
    }
}
