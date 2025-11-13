namespace EmployeeApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class editSalaryTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Salaries", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Salaries", "CreatedAt", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Salaries", "CreatedAt");
            DropColumn("dbo.Salaries", "IsDeleted");
        }
    }
}
