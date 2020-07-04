using System.Collections.Generic;
using Bindings;
using DMod.Models;

namespace Data.Services.Interfaces
{
    public interface IMobService
    {
        ICollection<Mob> GetMobs(int minX = 0, int maxX = Constants.PLAY_AREA_WIDTH, int minY = 0, int maxY = Constants.PLAY_AREA_HEIGHT, bool getDead = false);
        void RepopGalaxy(bool forceAll = false);
        void SaveMobs();
        void WanderMobs();
        void GenerateLoot(User[] owners, Mob mob);
        void CheckAggro();
        void DoCombat();
        Combat DoAttack(string targetId, string attackerId, Item weapon);
        ICollection<Combat> GetCombats(int x, int y);
        Mob GetMob(string ID);
        void CycleArrays();
    }
}
