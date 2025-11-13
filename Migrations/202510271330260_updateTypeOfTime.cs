namespace EmployeeApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateTypeOfTime : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Attendances", "CheckInTime", c => c.DateTime(nullable:true));
            AlterColumn("dbo.Attendances", "CheckOutTime", c => c.DateTime(nullable: true));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Attendances", "CheckOutTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Attendances", "CheckInTime", c => c.DateTime(nullable: false));
        }
    }
}
