using System.Collections.Generic;
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
        public static ICollection<Recipe> Recipes;
        public static ICollection<Loot> Loot = new List<Loot>();
        public static ICollection<Structure> Structures = new List<Structure>();
        public static List<Nebula> Nebulae = new List<Nebula>();
        public static Dictionary<int, string> PlayerIDs = new Dictionary<int, string>();
        public static Dictionary<string, string> RecipeHash = new Dictionary<string, string>();
    }
}
