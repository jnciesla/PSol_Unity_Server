namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Weaponsourcesfuel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "Admin", c => c.Boolean(nullable: false));
            AddColumn("dbo.Users", "Weap1MunSrc", c => c.Int(nullable: false));
            AddColumn("dbo.Users", "Weap2MunSrc", c => c.Int(nullable: false));
            AddColumn("dbo.Users", "Weap3MunSrc", c => c.Int(nullable: false));
            AddColumn("dbo.Users", "Weap4MunSrc", c => c.Int(nullable: false));
            AddColumn("dbo.Users", "Weap5MunSrc", c => c.Int(nullable: false));
            AddColumn("dbo.Users", "FuelEnergy", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "FuelEnergy");
            DropColumn("dbo.Users", "Weap5MunSrc");
            DropColumn("dbo.Users", "Weap4MunSrc");
            DropColumn("dbo.Users", "Weap3MunSrc");
            DropColumn("dbo.Users", "Weap2MunSrc");
            DropColumn("dbo.Users", "Weap1MunSrc");
            DropColumn("dbo.Users", "Admin");
        }
    }
}
