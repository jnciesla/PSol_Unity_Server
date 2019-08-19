using System.Collections.Generic;
using DMod.Models;

namespace Bindings
{
    public class Types
    {
        // Switch to a collection?
        public static string[] PlayerIds = new string[Constants.MAX_PLAYERS];
        public static User Default = new User();
        public static ICollection<Star> Galaxy;
        public static ICollection<Item> Items;
    }
}
