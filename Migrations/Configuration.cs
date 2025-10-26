namespace EmployeeApi.Migrations
{
    using EmployeeApi.Models;
    using EmployeeApi.Services;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<EmployeeApi.Data.EmpDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(EmployeeApi.Data.EmpDbContext context)
        {
            //  This method will be called after migrating to the latest version.
            context.Departments.AddOrUpdate(
                d=>d.Id,
                 new Department { Id = 1, Name = "IT" },
                new Department { Id = 2, Name = "Eng" }
                );
        
            context.Employees.AddOrUpdate(
                e => e.Id,
                new Employee
                {
                    Id = 1,
                    Name = "Hamza Salem",
                    Email = "hamza@gmail.com"

                ,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now,
                    DeptId = 1,
                    Password = HashingService.HashPassword("hamza"),
                    RoleId = 10
                });
            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
        }
    }
}
