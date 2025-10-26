namespace EmployeeApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class setUniqToEmail : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Employees", "Email", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.Roles", "Name", c => c.String(maxLength: 100));
            CreateIndex("dbo.Employees", "Email", unique: true);
            CreateIndex("dbo.Roles", "Name", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.Roles", new[] { "Name" });
            DropIndex("dbo.Employees", new[] { "Email" });
            AlterColumn("dbo.Roles", "Name", c => c.String());
            AlterColumn("dbo.Employees", "Email", c => c.String(nullable: false));
        }
    }
}
