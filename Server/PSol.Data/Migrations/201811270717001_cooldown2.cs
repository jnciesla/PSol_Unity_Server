namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Cooldown2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "Cooldown", c => c.DateTime(nullable: false));
            AddColumn("dbo.Users", "CooldownSpan", c => c.Int(nullable: false));
            DropColumn("dbo.Users", "CooldownMaximum");
            DropColumn("dbo.Users", "CooldownRemaining");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "CooldownRemaining", c => c.Int(nullable: false));
            AddColumn("dbo.Users", "CooldownMaximum", c => c.Int(nullable: false));
            DropColumn("dbo.Users", "CooldownSpan");
            DropColumn("dbo.Users", "Cooldown");
        }
    }
}
