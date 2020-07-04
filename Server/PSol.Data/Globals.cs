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
        public static ICollection<Recipe> Recipes;
        public static ICollection<Loot> Loot = new List<Loot>();
        public static List<Nebula> Nebulae = new List<Nebula>();
        public static string[] PlayerIds = new string[Constants.MAX_PLAYERS];
        public static Dictionary<string, string> RecipeHash = new Dictionary<string, string>();
    }
}
