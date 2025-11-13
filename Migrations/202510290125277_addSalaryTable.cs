namespace EmployeeApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addSalaryTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Salaries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        Month = c.Int(nullable: false),
                        Year = c.Int(nullable: false),
                        BaseSalary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Deduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NetSalary = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Salaries", "EmployeeId", "dbo.Employees");
            DropIndex("dbo.Salaries", new[] { "EmployeeId" });
            DropTable("dbo.Salaries");
        }
    }
}
