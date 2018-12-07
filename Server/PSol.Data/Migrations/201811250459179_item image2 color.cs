namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Itemimage2color : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Items", "OverlayColor", c => c.String(maxLength: 255));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Items", "OverlayColor");
        }
    }
}
