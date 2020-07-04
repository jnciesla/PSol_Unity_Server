using System;
using System.Collections.Generic;
using System.Linq;
using DMod.Models;
using Data.Repositories.Interfaces;

namespace Data.Repositories
{
    public class ItemRepository : IDisposable, IItemRepository
    {
        private readonly PSolDataContext _context;

        public ItemRepository(PSolDataContext context)
        {
            _context = context;
        }

        public ICollection<Item> LoadItems()
        {
            return _context.Items.ToList();
        }

        public ICollection<Recipe> LoadRecipes()
        {
            return _context.Recipes.ToList();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
