namespace EmployeeApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edittable : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Employees", name: "DepId", newName: "DeptId");
            RenameIndex(table: "dbo.Employees", name: "IX_DepId", newName: "IX_DeptId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Employees", name: "IX_DeptId", newName: "IX_DepId");
            RenameColumn(table: "dbo.Employees", name: "DeptId", newName: "DepId");
        }
    }
}
