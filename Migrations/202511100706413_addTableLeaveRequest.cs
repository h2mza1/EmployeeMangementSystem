namespace EmployeeApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTableLeaveRequest : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LeaveRequests",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(),
                        Reason = c.String(),
                        EmployeeId = c.Int(nullable: false),
                        FromTime = c.DateTime(),
                        ToTime = c.DateTime(),
                        ApprovedById = c.Int(),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        DeletedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.ApprovedById)
                .ForeignKey("dbo.Employees", t => t.EmployeeId)
                .Index(t => t.EmployeeId)
                .Index(t => t.ApprovedById);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LeaveRequests", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.LeaveRequests", "ApprovedById", "dbo.Employees");
            DropIndex("dbo.LeaveRequests", new[] { "ApprovedById" });
            DropIndex("dbo.LeaveRequests", new[] { "EmployeeId" });
            DropTable("dbo.LeaveRequests");
        }
    }
}
