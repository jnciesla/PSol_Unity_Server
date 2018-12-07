namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Itemdropchanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Items", "Chance", c => c.Int(nullable: false));
            DropColumn("dbo.Items", "Droppable");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Items", "Droppable", c => c.Boolean(nullable: false));
            DropColumn("dbo.Items", "Chance");
        }
    }
}
