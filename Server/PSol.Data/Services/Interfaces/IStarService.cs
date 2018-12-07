using System.Collections.Generic;
using Data.Models;

namespace Data.Services.Interfaces
{
    public interface IStarService
    {
        ICollection<Star> LoadStars();
    }
}
