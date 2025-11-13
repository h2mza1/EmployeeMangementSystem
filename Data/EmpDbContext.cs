using EmployeeApi.Models;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.IdentityModel.Protocols.WSTrust;
using System.Linq;
using System.Text;
using System.Text.Json;
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

            modelBuilder.Entity<LeaveRequest>()
                .HasRequired(l => l.Employee)
                .WithMany() 
                .HasForeignKey(l => l.EmployeeId)
                .WillCascadeOnDelete(false);  

            modelBuilder.Entity<LeaveRequest>()
                .HasOptional(l => l.ApprovedBy)
                .WithMany()
                .HasForeignKey(l => l.ApprovedById)
                .WillCascadeOnDelete(false); 




        }
       
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Role > Roles { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Salary> Salaries { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<Vacation> Vacations { get; set; }

        //public int CurrentUserId { get; set; } = 0;

        public override async Task<int> SaveChangesAsync()
        {
            var deletedRecords = ChangeTracker.Entries()
                .Where(x => x.State == EntityState.Modified
                && x.CurrentValues.PropertyNames.Contains("IsDeleted")
                && x.OriginalValues["IsDeleted"] is bool old
                && x.CurrentValues["IsDeleted"] is bool cur
                && old == false && cur == true)
                .ToList();



            foreach (var entry in deletedRecords)
            {
                if (entry.CurrentValues.PropertyNames.Contains("DeletedAt"))
                {
                    entry.CurrentValues["DeletedAt"] = DateTime.Now;
                }
            }

            return await base.SaveChangesAsync();

        }


    }
}
