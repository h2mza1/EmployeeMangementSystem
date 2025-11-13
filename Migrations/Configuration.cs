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
            if (!context.Departments.Any())
            
                {
                    context.Departments.AddOrUpdate(
                d => d.Id,
                 new Department { Id = 1, Name = "IT" },
                new Department { Id = 2, Name = "Eng" }
                );

                    context.SaveChanges();
                }
                if (!context.Roles.Any())
                {
                    context.Roles.AddOrUpdate(
                r => r.Id,
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "Manager" },
                new Role { Id = 3, Name = "Employee" }


                );
                    context.SaveChanges();
                }
                if (!context.Employees.Any())
                {
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
                    DeptId = 3,
                    Password = HashingService.HashPassword("hamza"),
                    RoleId = 1,

                });
                    context.SaveChanges();
                }
            
            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
        }
    }
}
