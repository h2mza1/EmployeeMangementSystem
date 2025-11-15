namespace EmployeeApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addVactionITtableLeave : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Vacations");
            AddColumn("dbo.LeaveRequests", "VacationId", c => c.Int(nullable: false));
            AlterColumn("dbo.Vacations", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Vacations", "Id");
            CreateIndex("dbo.LeaveRequests", "VacationId");
            AddForeignKey("dbo.LeaveRequests", "VacationId", "dbo.Vacations", "Id", cascadeDelete: true);
            DropColumn("dbo.LeaveRequests", "Type");
        }
        
        public override void Down()
        {
            AddColumn("dbo.LeaveRequests", "Type", c => c.Int(nullable: false));
            DropForeignKey("dbo.LeaveRequests", "VacationId", "dbo.Vacations");
            DropIndex("dbo.LeaveRequests", new[] { "VacationId" });
            DropPrimaryKey("dbo.Vacations");
            AlterColumn("dbo.Vacations", "Id", c => c.Long(nullable: false, identity: true));
            DropColumn("dbo.LeaveRequests", "VacationId");
            AddPrimaryKey("dbo.Vacations", "Id");
        }
    }
}
