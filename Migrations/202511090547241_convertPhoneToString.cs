namespace EmployeeApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class convertPhoneToString : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Employees", "Phone", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Employees", "Phone", c => c.Int(nullable: false));
        }
    }
}
