using System.Collections.Generic;
using Data.Models;

namespace Data.Services.Interfaces
{
    public interface IItemService
    {
        ICollection<Item> LoadItems();
    }
}