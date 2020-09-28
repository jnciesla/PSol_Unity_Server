namespace Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Lootchanges : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.LootItems", "MobType_Id", "dbo.MobTypes");
            DropForeignKey("dbo.LootItems", "MobType_Id1", "dbo.MobTypes");
            DropForeignKey("dbo.LootItems", "Planet_Id", "dbo.Planets");
            DropForeignKey("dbo.LootItems", "Planet_Id1", "dbo.Planets");
            DropIndex("dbo.LootItems", new[] { "MobType_Id" });
            DropIndex("dbo.LootItems", new[] { "MobType_Id1" });
            DropIndex("dbo.LootItems", new[] { "Planet_Id" });
            DropIndex("dbo.LootItems", new[] { "Planet_Id1" });
            AddColumn("dbo.Items", "Acquisition", c => c.Int(nullable: false));
            AddColumn("dbo.MobTypes", "LootTable", c => c.String(maxLength: 255));
            DropColumn("dbo.Items", "Chance");
            DropTable("dbo.LootItems");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.LootItems",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 255),
                        ItemID = c.String(maxLength: 255),
                        MinQTY = c.Int(nullable: false),
                        MaxQTY = c.Int(nullable: false),
                        Chance = c.Int(nullable: false),
                        MobType_Id = c.String(maxLength: 36),
                        MobType_Id1 = c.String(maxLength: 36),
                        Planet_Id = c.String(maxLength: 36),
                        Planet_Id1 = c.String(maxLength: 36),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.Items", "Chance", c => c.Int(nullable: false));
            DropColumn("dbo.MobTypes", "LootTable");
            DropColumn("dbo.Items", "Acquisition");
            CreateIndex("dbo.LootItems", "Planet_Id1");
            CreateIndex("dbo.LootItems", "Planet_Id");
            CreateIndex("dbo.LootItems", "MobType_Id1");
            CreateIndex("dbo.LootItems", "MobType_Id");
            AddForeignKey("dbo.LootItems", "Planet_Id1", "dbo.Planets", "Id");
            AddForeignKey("dbo.LootItems", "Planet_Id", "dbo.Planets", "Id");
            AddForeignKey("dbo.LootItems", "MobType_Id1", "dbo.MobTypes", "Id");
            AddForeignKey("dbo.LootItems", "MobType_Id", "dbo.MobTypes", "Id");
        }
    }
}
