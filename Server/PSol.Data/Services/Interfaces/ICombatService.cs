using System.Collections.Generic;
using DMod.Models;

namespace Data.Services.Interfaces
{
    public interface ICombatService
    {
        Combat DoAttack(string targetId, string attackerId, Item weapon);
        ICollection<Combat> GetCombats(int x, int y);
        void CycleArrays();
    }
}
