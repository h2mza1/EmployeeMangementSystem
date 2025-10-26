namespace EmployeeApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPassword : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Employees", "Password", c => c.String(nullable: false,defaultValue:"hamza"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Employees", "Password");
        }
    }
}
