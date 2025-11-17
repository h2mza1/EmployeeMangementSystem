using Microsoft.DotNet.Scaffolding.Shared.T4Templating;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeApi.ViewModels
{
    public class EmpDto
    {   
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public int DeptId { get; set; }
        public string DeptName { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; }= DateTime.Now;
        public bool IsDeleted { get; set; } = false;
        [Column(TypeName = "decimal")]

        public decimal Salary { get; set; }
        public int workingHours { get; set; } = 0;
        public int expectedWorkHours { get; set; } = 0;



    }
}
