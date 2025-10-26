using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeApi.Models
{
    public class Role : Shared
    {
        public int Id { get; set; }
        [Index(IsUnique = true)]
        [MaxLength(100)]
        public string Name { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
    }
}
