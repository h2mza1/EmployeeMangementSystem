using EmployeeApi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeApi.Services
{
    public class SalaryService
    {
        private readonly EmpDbContext context;
        private const int day = 22;
        public SalaryService() { context = new EmpDbContext(); }
        
        public decimal CalculateNetSalary(decimal baseSalary , int empId)
        {
            decimal netSalary = 0;
            return netSalary;
        }
    }
}
