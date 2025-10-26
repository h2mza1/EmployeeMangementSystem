namespace EmployeeApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addSharedClass : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Attendances", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Attendances", "CreatedAt", c => c.DateTime(nullable: false));
            AddColumn("dbo.Roles", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Roles", "CreatedAt", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Roles", "CreatedAt");
            DropColumn("dbo.Roles", "IsDeleted");
            DropColumn("dbo.Attendances", "CreatedAt");
            DropColumn("dbo.Attendances", "IsDeleted");
        }
    }
}
