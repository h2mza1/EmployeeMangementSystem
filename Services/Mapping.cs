using EmployeeApi.Models;
using EmployeeApi.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeApi.Services
{
    public static class Mapping
    {
    public  static Employee MapTo (EmpDto model)
        {
            return new Employee
            {
                Id = model.Id,
                Name = model.Name,
                Email = model.Email,
                BaseSalary = model.Salary,
                EndDate = model.EndDate,
                StartDate = model.StartDate,
                IsDeleted = model.IsDeleted,
                DeptId = model.DeptId,
                Phone = model.Phone,
                RoleId = model.RoleId,

            };
        }
     public   static EmpDto MapTo(Employee model)
        {
            return new EmpDto
            {
                Id = model.Id,
                Name = model.Name,
                Email = model.Email,
                Salary = model.BaseSalary,
                EndDate = model.EndDate,
                StartDate = model.StartDate,
                IsDeleted = model.IsDeleted,
                RoleId = model.RoleId,
                DeptId = model.DeptId,
                Phone = model.Phone,
                

            };
        }
        public static DeptDto MapTo(Department model)
        {
            return new DeptDto
            {
                Id = model.Id,
                Name = model.Name,
               
                
            };
        }
        public static Department MapTo(DeptDto model)
        {
            return new Department
            {
                Id = model.Id ?? 0,
                Name = model.Name
            };
        }
    }
}
