namespace Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class Recipes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Recipes",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 36),
                        Item1 = c.String(maxLength: 255),
                        Item2 = c.String(maxLength: 255),
                        Item3 = c.String(maxLength: 255),
                        Item4 = c.String(maxLength: 255),
                        Item5 = c.String(maxLength: 255),
                        Item6 = c.String(maxLength: 255),
                        Item7 = c.String(maxLength: 255),
                        Item8 = c.String(maxLength: 255),
                        Output = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Recipes");
        }
    }
}
