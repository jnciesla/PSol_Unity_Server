﻿using System.Data.Entity;
using DMod.Models;

namespace Data
{
    public class PSolDataContext : DbContext
    {
        private const int MaxIdLength = 36;
        public DbSet<User> Users { get; set; }
        public DbSet<Star> Stars { get; set; }
        public DbSet<Planet> Planets { get; set; }
        public DbSet<MobType> MobTypes { get; set; }
        public DbSet<Mob> Mobs { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Inventory> Inventory { get; set; }

        public PSolDataContext() : base("PSolDataConnection")
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<PSolDataContext>());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Properties<string>().Configure(c => c.HasMaxLength(255));
            

            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .HasMaxLength(MaxIdLength);

            modelBuilder.Entity<Star>()
                .Property(u => u.Id)
                .HasMaxLength(MaxIdLength);

            modelBuilder.Entity<Planet>()
                .Property(u => u.Id)
                .HasMaxLength(MaxIdLength);

            modelBuilder.Entity<Planet>()
                .Property(u => u.StarId)
                .HasMaxLength(MaxIdLength);

            modelBuilder.Entity<MobType>()
                .Property(m => m.Id)
                .HasMaxLength(MaxIdLength);

            modelBuilder.Entity<Mob>()
                .Property(m => m.Id)
                .HasMaxLength(MaxIdLength);

            modelBuilder.Entity<Mob>()
                .Property(m => m.KilledDate)
                .HasColumnType("datetime2");

            modelBuilder.Entity<Mob>()
                .Property(m => m.SpawnDate)
                .HasColumnType("datetime2");

            modelBuilder.Entity<Item>()
                .Property(i => i.Id)
                .HasMaxLength(MaxIdLength);

            modelBuilder.Entity<Inventory>()
                .Property(i => i.Id)
                .HasMaxLength(MaxIdLength);

            modelBuilder.Entity<Inventory>()
                .Property(i => i.UserId)
                .HasMaxLength(MaxIdLength);

            modelBuilder.Entity<Inventory>()
                .Property(i => i.ItemId)
                .HasMaxLength(MaxIdLength);
        }
    }
}
