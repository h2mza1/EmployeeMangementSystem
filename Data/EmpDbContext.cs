using EmployeeApi.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeApi.Data
{
    public class EmpDbContext : DbContext
    {
        public EmpDbContext() : base("name=DefaultConnection") { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Attendance>()
                .HasRequired(e => e.Employee)
                .WithMany(e => e.Attendances)
                .HasForeignKey(e => e.EmployeeId)
                .WillCascadeOnDelete();
            modelBuilder.Entity<Employee>()
                .HasRequired(e => e.Role)
                .WithMany(r => r.Employees)
                .HasForeignKey(e => e.RoleId);
            
          

                
           

        }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Role > Roles { get; set; }
        public DbSet<Attendance> Attendances { get; set; }


        
    }
}
