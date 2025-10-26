using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeApi.Models
{
    public class Employee : Shared
    {
        public int Id { get; set; }
        [Required]
        [DataType(DataType.Text)]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [Index(IsUnique = true)]
        [MaxLength(100)]
        public string Email { get; set; }
        public int Phone { get; set; }
        [Required]
        public string Password { get; set; } = "hamza";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int RoleId { get; set; }
        public virtual Role Role { get; set; }
        [ForeignKey(nameof(Department))]
        public int DeptId { get; set; }
        public virtual Department Department { get; set; }
        public virtual ICollection<Attendance> Attendances { get; set; }

    }
}
