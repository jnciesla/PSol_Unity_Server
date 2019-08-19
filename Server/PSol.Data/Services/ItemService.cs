using System.Collections.Generic;
using DMod.Models;
using Data.Repositories.Interfaces;
using Data.Services.Interfaces;

namespace Data.Services
{
    public class ItemService: IItemService
    {
        private readonly IItemRepository _itemRep;
        private ICollection<Item> _items;

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
    }
}
