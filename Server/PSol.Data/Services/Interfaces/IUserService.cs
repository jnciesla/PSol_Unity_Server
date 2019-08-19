﻿using System.Collections.Generic;
using DMod.Models;

namespace Data.Services.Interfaces
{
    public interface IUserService
    {
        List<User> ActiveUsers { get; set; }
        User RegisterUser(string username, string password);
        User LoadPlayer(string username);
        void SavePlayer(User user);
        bool AccountExists(string username);
        bool PasswordOK(string username, string password);
    }
}
