using System.Collections.Generic;
using DMod.Models;

namespace Data.Services.Interfaces
{
    public interface IGameService
    {
        void SaveGame(List<User> users);
    }
}
