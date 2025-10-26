using System;
using System.Collections.Generic;
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
        public int Phone { get; set; }
        public string Password { get; set; }
        public int DeptId { get; set; }
        public string DeptName { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; }= DateTime.Now;

    }
}
