﻿using System.Collections.Generic;
using Bindings;
using Data.Models;

namespace Data.Services.Interfaces
{
    public interface IMobService
    {
        ICollection<Mob> GetMobs(int minX = 0, int maxX = Constants.PLAY_AREA_WIDTH, int minY = 0, int maxY = Constants.PLAY_AREA_HEIGHT, bool getDead = false);
        void RepopGalaxy(bool forceAll = false);
        void SaveMobs();
        void WanderMobs();
        void CheckAggro();
        void DoCombat();
    }
}