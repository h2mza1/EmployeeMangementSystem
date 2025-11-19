using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeApi.ViewModels
{
    public class SalaryDto
    {
        public int Id { get; set; }
        [Required]
        public int EmployeeId { get; set; }

        [Range(1, 12)]
        public int Month { get; set; } 
            
        public int Year { get; set; }

        public string EmployeeName{ get; set; }= string.Empty;
       
        [Column(TypeName = "decimal")]
        public decimal BaseSalary { get; set; }

        [Column(TypeName = "decimal")]
        public decimal Deduction { get; set; } = 0;

        [Column(TypeName = "decimal")]
        public decimal NetSalary { get; set; }
    }
}
