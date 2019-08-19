using System.Collections.Generic;
using DMod.Models;
using Data.Repositories.Interfaces;
using Data.Services.Interfaces;

namespace Data.Services
{
    public class StarService: IStarService
    {
        private readonly IStarRepository _starRep;
        private ICollection<Star> _stars;

        public StarService(IStarRepository starRep)
        {
            _starRep = starRep;
        }

        public ICollection<Star> LoadStars()
        {
            if (_stars != null) return _stars;
            _stars = _starRep.LoadStars();
            return _stars;
        }
    }
}
