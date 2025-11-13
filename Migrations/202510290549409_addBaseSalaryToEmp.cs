namespace EmployeeApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addBaseSalaryToEmp : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Employees", "BaseSalary", c => c.Decimal(nullable: false, precision: 18, scale: 2,defaultValue:600));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Employees", "BaseSalary");
        }
    }
}
