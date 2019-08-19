using System.Collections.Generic;
using DMod.Models;

namespace Data.Services.Interfaces
{
    public interface IItemService
    {
        ICollection<Item> LoadItems();
    }
}