namespace EmployeeApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Attendances", "Status", c => c.Boolean(nullable: false,defaultValue:false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Attendances", "Status");
        }
    }
}
