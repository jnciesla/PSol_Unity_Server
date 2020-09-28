using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DMod.Models;
using Data.Repositories.Interfaces;

namespace Data.Repositories
{
    public class StarRepository: IDisposable, IStarRepository
    {
        private readonly PSolDataContext _context;

        public StarRepository(PSolDataContext context)
        {
            _context = context;
        }

        public ICollection<Star> LoadStars()
        {
            return _context.Stars.Include(s => s.Planets).ToList();
        }

        public ICollection<Structure> LoadStructures()
        {
            return _context.Structures.ToList();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
