using Data.Models;
using System.Collections.Generic;

namespace Data.Repositories.Interfaces
{
    public interface IStarRepository
    {
        ICollection<Star> LoadStars();
    }
}
