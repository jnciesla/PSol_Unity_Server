namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Itemimage2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Items", "Overlay", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Items", "Overlay");
        }
    }
}
