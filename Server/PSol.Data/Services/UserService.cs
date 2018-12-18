using System;
using System.Collections.Generic;
using Data.Models;
using Data.Repositories.Interfaces;
using Data.Services.Interfaces;

namespace Data.Services
{
    public class UserService : IUserService
    {
        public List<User> ActiveUsers { get; set; } = new List<User>();
        private readonly IUserRepository _userRepo;

        public UserService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public User LoadPlayer(string username)
        {
            return _userRepo.LoadPlayer(username);
        }

        public void SavePlayer(User user)
        {
            _userRepo.SavePlayer(user);
        }


        public User RegisterUser(string username, string password)
        {
            var newUser = new User
            {
                Password = password,
                Name = username,
                X = 0,
                Z = 0,
                Heading = 0,
                Roll = 0,
                M = 0,
                Health = 100,
                MaxHealth = 100,
                Shield = 100,
                MaxShield = 100,
                Rank = "2LT",
                Credits = 1000,
                Cooldown = DateTime.UtcNow
            };
            _userRepo.Add(newUser);
            return newUser;
        }

        public bool AccountExists(string username)
        {
            return _userRepo.AccountExists(username);
        }

        public bool PasswordOK(string username, string password)
        {
            return _userRepo.PasswordOK(username, password);
        }
    }
}
