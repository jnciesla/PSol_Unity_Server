namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addedplayercolor : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "Color", c => c.String(maxLength: 255));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "Color");
        }
    }
}
