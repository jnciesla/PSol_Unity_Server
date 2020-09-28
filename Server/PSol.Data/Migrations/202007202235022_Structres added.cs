namespace Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Structresadded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Structures",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 36),
                        Name = c.String(maxLength: 255),
                        Model = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        X = c.Single(nullable: false),
                        Y = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.Inventories", "Structure_ID", c => c.String(maxLength: 36));
            AddColumn("dbo.Planets", "Structure", c => c.String(maxLength: 255));
            CreateIndex("dbo.Inventories", "Structure_ID");
            AddForeignKey("dbo.Inventories", "Structure_ID", "dbo.Structures", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Inventories", "Structure_ID", "dbo.Structures");
            DropIndex("dbo.Inventories", new[] { "Structure_ID" });
            DropColumn("dbo.Planets", "Structure");
            DropColumn("dbo.Inventories", "Structure_ID");
            DropTable("dbo.Structures");
        }
    }
}
