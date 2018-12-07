namespace Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class Loottables : DbMigration
    {
        public override void Up()
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
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.MobTypes", t => t.MobType_Id)
                .ForeignKey("dbo.MobTypes", t => t.MobType_Id1)
                .ForeignKey("dbo.Planets", t => t.Planet_Id)
                .ForeignKey("dbo.Planets", t => t.Planet_Id1)
                .Index(t => t.MobType_Id)
                .Index(t => t.MobType_Id1)
                .Index(t => t.Planet_Id)
                .Index(t => t.Planet_Id1);
            
            AddColumn("dbo.Items", "Droppable", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LootItems", "Planet_Id1", "dbo.Planets");
            DropForeignKey("dbo.LootItems", "Planet_Id", "dbo.Planets");
            DropForeignKey("dbo.LootItems", "MobType_Id1", "dbo.MobTypes");
            DropForeignKey("dbo.LootItems", "MobType_Id", "dbo.MobTypes");
            DropIndex("dbo.LootItems", new[] { "Planet_Id1" });
            DropIndex("dbo.LootItems", new[] { "Planet_Id" });
            DropIndex("dbo.LootItems", new[] { "MobType_Id1" });
            DropIndex("dbo.LootItems", new[] { "MobType_Id" });
            DropColumn("dbo.Items", "Droppable");
            DropTable("dbo.LootItems");
        }
    }
}
