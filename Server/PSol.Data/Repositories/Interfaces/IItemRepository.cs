using System.Collections.Generic;
using DMod.Models;

namespace Data.Repositories.Interfaces
{
    public interface IItemRepository
    {
        ICollection<Item> LoadItems();
        ICollection<Recipe> LoadRecipes();
    }
}
