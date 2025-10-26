namespace EmployeeApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSeed : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Employees",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        Phone = c.Int(nullable: false),
                        DepId = c.Int(nullable: false, defaultValue:1),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Departments", t => t.DepId, cascadeDelete: true)
                .Index(t => t.DepId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Employees", "DepId", "dbo.Departments");
            DropIndex("dbo.Employees", new[] { "DepId" });
            DropTable("dbo.Employees");
            DropTable("dbo.Departments");
        }
    }
}
