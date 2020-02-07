﻿using Data.Repositories;
using Data.Repositories.Interfaces;
using Data.Services;
using Data.Services.Interfaces;
using Ninject.Modules;

namespace Server
{
    public class ServerModule : NinjectModule
    {
        public override void Load()
        {
            Bind(typeof(IUserRepository)).To(typeof(UserRepository)).InSingletonScope();
            Bind(typeof(IUserService)).To(typeof(UserService)).InSingletonScope();
            Bind(typeof(IGameService)).To(typeof(GameService)).InSingletonScope();
            Bind(typeof(IStarRepository)).To(typeof(StarRepository)).InSingletonScope();
            Bind(typeof(IStarService)).To(typeof(StarService)).InSingletonScope();
            Bind(typeof(IMobRepository)).To(typeof(MobRepository)).InSingletonScope();
            Bind(typeof(IMobService)).To(typeof(MobService)).InSingletonScope();
            Bind(typeof(IItemRepository)).To(typeof(ItemRepository)).InSingletonScope();
            Bind(typeof(IItemService)).To(typeof(ItemService)).InSingletonScope();
        }
    }
}
