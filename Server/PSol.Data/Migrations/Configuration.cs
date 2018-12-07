using System.Collections.Generic;
using Data.Models;

namespace Data.Migrations
{
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<PSolDataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(PSolDataContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.

            context.Stars.AddOrUpdate(new Star
            {
                Id = "7179FC09-EBAA-49D8-8ED4-A53F7B95F260",
                Name = "Vyt",
                Class = "MK - G",
                Belligerence = "Moderate",
                X = 1000,
                Y = 1000,
                Planets = new List<Planet>()
            });
            context.Stars.AddOrUpdate(new Star
            {
                Id = "d2f03d3c-c465-4ae2-a887-d783037253ef",
                Name = "Scathe",
                Class = "MK - G",
                Belligerence = "Moderate",
                X = 4000,
                Y = 5000,
                Planets = new List<Planet>()
            });

            context.Planets.AddOrUpdate(new Planet
            {
                Id = "c804ef2e-793f-43f1-9d8f-d90a2df71d3a",
                StarId = "7179FC09-EBAA-49D8-8ED4-A53F7B95F260",
                Name = "Vyt I",
                Class = "Terrestrial",
                Belligerence = "Moderate",
                Orbit = 600,
                Sprite = 2,
                X = 1600,
                Y = 1600,
                Color = "1"
            });

            context.SaveChanges();

            context.MobTypes.AddOrUpdate(new MobType
            {
                Id = "7A93241E-D251-4E7E-A565-46B0E274B5C6",
                BonusExp = 0,
                Credits = 0,
                Level = 1,
                MaxHealth = 100,
                MaxShield = 0,
                MaxSpawned = 4,
                Name = "Vyt Scout",
                SpawnRadius = 100,
                SpawnTimeMax = 45,
                SpawnTimeMin = 20,
                Sprite = 2,
                Star = context.Stars.FirstOrDefault(s => s.Name == "Vyt")
            });

            context.Items.AddOrUpdate(new Item
            {
                Id = "50521cfe-7d63-4495-a1cf-b900fb8d225f",
                Name = "Weird horse thing",
                Description = "The very first item added to the game.",
                Type = "Biospecimen",
                Image = 1,
                Color = "1",
                Mass = 1,
                Cost = 500,
                Stack = true,
                Level = 0
            });

            context.Items.AddOrUpdate(new Item
            {
                Id = "a53edcf0-1105-4edf-9c30-217279c3d286",
                Name = "Laser MK1",
                Description = "Basic laser",
                Type = "Weapon",
                Image = 1,
                Color = "1",
                Mass = 1,
                Cost = 500,
                Stack = true,
                Level = 0,
                Power = 0,
                Damage = 20,
                Recharge = 20,
                Slot = 7
            });
            context.Items.AddOrUpdate(new Item
            {
                Id = "b0eaf963-7ed2-4472-9131-0470eefc3f04",
                Name = "Gold Ingot",
                Description = "Ingot of refined gold",
                Type = "Metal",
                Image = 2,
                Color = "227167015",
                Mass = 1,
                Cost = 20,
                Stack = true
            });
            context.Items.AddOrUpdate(new Item
            {
                Id = "fed12f95-711d-4fb3-ac97-535fb85587e3",
                Name = "Silver Ingot",
                Description = "Ingot of refined silver",
                Type = "Metal",
                Image = 2,
                Color = "142163175",
                Mass = 1,
                Cost = 16,
                Stack = true
            });
            context.Items.AddOrUpdate(new Item
            {
                Id = "0b7889d3-a5ea-4ce6-9dfc-b8a6cde752ee",
                Name = "Iron Ingot",
                Description = "Ingot of refined iron",
                Type = "Metal",
                Image = 2,
                Color = "073062056",
                Mass = 1,
                Cost = 8,
                Stack = true
            });
            context.Items.AddOrUpdate(new Item
            {
                Id = "8512d971-ebb4-4826-bf1a-f8b60414c84f",
                Name = "Gold Ore",
                Description = "Ore containing traces of gold",
                Type = "Ore",
                Image = 3,
                Overlay = 5,
                OverlayColor = "196138000",
                Mass = 1,
                Cost = 8,
                Stack = true
            });
            context.Items.AddOrUpdate(new Item
            {
                Id = "9a2a8c63-7b1f-4fbb-8189-c41a45379fbb",
                Name = "Salvaged Materials",
                Description = "Repair materials salvaged from enemy craft.  This can be used to repair damage to your own ship.",
                Type = "Consumable",
                Image = 4,
                Color = "0",
                Mass = 1,
                Cost = 2,
                Stack = true
            });

            context.SaveChanges();
        }
    }
}
