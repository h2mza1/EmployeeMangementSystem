namespace EmployeeApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addDeletedAtProp : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Attendances", "DeletedAt", c => c.DateTime());
            AddColumn("dbo.Employees", "DeletedAt", c => c.DateTime());
            AddColumn("dbo.Departments", "DeletedAt", c => c.DateTime());
            AddColumn("dbo.Roles", "DeletedAt", c => c.DateTime());
            AddColumn("dbo.Salaries", "DeletedAt", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Salaries", "DeletedAt");
            DropColumn("dbo.Roles", "DeletedAt");
            DropColumn("dbo.Departments", "DeletedAt");
            DropColumn("dbo.Employees", "DeletedAt");
            DropColumn("dbo.Attendances", "DeletedAt");
        }
    }
}
