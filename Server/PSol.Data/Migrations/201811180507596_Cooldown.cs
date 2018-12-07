namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Cooldown : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "CooldownMaximum", c => c.Int(nullable: false));
            AddColumn("dbo.Users", "CooldownRemaining", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "CooldownRemaining");
            DropColumn("dbo.Users", "CooldownMaximum");
        }
    }
}
