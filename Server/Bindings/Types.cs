using System.Collections.Generic;
using Data.Models;

namespace Bindings
{
    public class Types
    {
        // Switch to a collection?
        public static User[] Player = new User[Constants.MAX_PLAYERS];
        public static string[] PlayerIds = new string[Constants.MAX_PLAYERS];
        public static User Default = new User();
        public static ICollection<Star> Galaxy;
        public static ICollection<Item> Items;
    }
}
