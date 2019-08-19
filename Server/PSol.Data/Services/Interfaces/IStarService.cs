using System.Collections.Generic;
using DMod.Models;

namespace Data.Services.Interfaces
{
    public interface IStarService
    {
        ICollection<Star> LoadStars();
    }
}
