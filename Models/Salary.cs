using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeApi.Models
{

    public class Salary:Shared
    {
        [Key]
        public int Id { get; set; } 

        [Required]
        [ForeignKey("Employee")]
        public int EmployeeId { get; set; } 

        [Required]
        [Range(1, 12)]
        public int Month { get; set; } 

        [Required]
        public int Year { get; set; } 

        [Required]
        [Column(TypeName = "decimal")]
        public decimal BaseSalary { get; set; } 

        [Column(TypeName = "decimal")]
        public decimal Deduction { get; set; } = 0; 
     
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Column(TypeName = "decimal")]
        [DefaultValue(0)]
        public decimal NetSalary { get; set; }         
        public virtual Employee Employee { get; set; }
    }
}
