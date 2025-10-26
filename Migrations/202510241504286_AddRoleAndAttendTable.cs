namespace EmployeeApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRoleAndAttendTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Attendances",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CheckInTime = c.DateTime(nullable: false),
                        CheckOutTime = c.DateTime(nullable: false),
                        Date = c.DateTime(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Employees", "StartDate", c => c.DateTime(nullable: true));
            AddColumn("dbo.Employees", "EndDate", c => c.DateTime(nullable: true));
            AddColumn("dbo.Employees", "RoleId", c => c.Int(nullable: false,defaultValue:3));
            CreateIndex("dbo.Employees", "RoleId");
            AddForeignKey("dbo.Employees", "RoleId", "dbo.Roles", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Attendances", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.Employees", "RoleId", "dbo.Roles");
            DropIndex("dbo.Employees", new[] { "RoleId" });
            DropIndex("dbo.Attendances", new[] { "EmployeeId" });
            DropColumn("dbo.Employees", "RoleId");
            DropColumn("dbo.Employees", "EndDate");
            DropColumn("dbo.Employees", "StartDate");
            DropTable("dbo.Roles");
            DropTable("dbo.Attendances");
        }
    }
}
