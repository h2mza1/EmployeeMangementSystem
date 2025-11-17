using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeApi.ViewModels
{
    public class SalaryDetail
    {
        public int Id { get; set; }
        public decimal BaseSalary { get; set; }
        public decimal Deduction { get; set; }
        public decimal NetSalary { get; set; }
        public int TotalWorkHours { get; set; }
        public int TotalAbsentHours { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string EmployeeName { get; set; }
        public string DeptName { get; set; }
        public int PresenttDays { get; set; } =0;

    }
}
    