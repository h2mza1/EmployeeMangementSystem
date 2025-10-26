using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeApi.ViewModels
{
    public class RoleDto
    {
        public int? Id { get; set; }
        [Required]
        [DataType(DataType.Text)]
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
    }
}
