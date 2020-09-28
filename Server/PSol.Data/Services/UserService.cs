using System;
using System.Collections.Generic;
using DMod.Models;
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

        public void AddExperience(User user, int killLevel, int bonus, bool special)
        {
            var nextLevel = Math.Floor(Constants.LVL_BASE * Math.Pow(user.Level, Constants.LVL_EXPONENT));
            float offset = 1;
            if (killLevel > user.Level)
            {
                offset = 1 + 0.05f * (killLevel - user.Level);
            }
            else if (killLevel < user.Level)
            {
                offset = 1.0f - (float)(user.Level - killLevel) / 10;
                if (user.Level - killLevel >= 10) offset = 0; //Don't give xp if the level difference is greater than 10.
            }

            var amount = (killLevel * Constants.EXP_MULT + bonus) * offset;

            if (special) amount = amount * 1.5f;
            user.Exp += (int)Math.Ceiling(amount);

            if (user.Exp >= nextLevel)
            {
                if (user.Level < Constants.MAX_LEVEL)
                {
                    user.Level++;
                }
                else
                {

                }
            }
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

        public void RechargeSystems()
        {
            ActiveUsers.ForEach(user =>
            {
                if (user.Weap1Charge <= 100)
                {
                    user.Weap1Charge += user.Weap1ChargeRate;
                }
                else
                {
                    user.Weap1Charge = 100;
                }
                if (user.Weap2Charge <= 100)
                {
                    user.Weap2Charge += user.Weap2ChargeRate;
                }
                else
                {
                    user.Weap2Charge = 100;
                }
                if (user.Weap3Charge <= 100)
                {
                    user.Weap3Charge += user.Weap3ChargeRate;
                }
                else
                {
                    user.Weap3Charge = 100;
                }
                if (user.Weap4Charge <= 100)
                {
                    user.Weap4Charge += user.Weap4ChargeRate;
                }
                else
                {
                    user.Weap4Charge = 100;
                }
                if (user.Weap5Charge <= 100)
                {
                    user.Weap5Charge += user.Weap5ChargeRate;
                }
                else
                {
                    user.Weap5Charge = 100;
                }
            });
        }
    }
}
