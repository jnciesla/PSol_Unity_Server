using System.Collections.Generic;
using Data.Models;

namespace Data.Services.Interfaces
{
    public interface IGameService
    {
        void SaveGame(List<User> users);
    }
}
