using System;
using System.Collections.Generic;
using System.Linq;
using Bindings;
using DMod.Models;
using Data.Repositories.Interfaces;
using Data.Services.Interfaces;

namespace Data.Services
{
    public class ItemService: IItemService
    {
        private readonly IItemRepository _itemRep;
        private ICollection<Item> _items;
        private ICollection<Recipe> _recipes;

        public ItemService(IItemRepository itemRep)
        {
            _itemRep = itemRep;
        }

        public ICollection<Item> LoadItems()
        {
            if (_items != null) return _items;
            _items = _itemRep.LoadItems();
            return _items;
        }

        public ICollection<Recipe> LoadRecipes()
        {
            if (_recipes != null) return _recipes;
            _recipes = _itemRep.LoadRecipes();
            return _recipes;
        }

        public void RemoveLoot()
        {
            Globals.Loot = Globals.Loot.Where(l => l.Dropped > DateTime.UtcNow.AddMinutes(-Constants.LOOT_DWELL)).ToList();
        }
    }
}
