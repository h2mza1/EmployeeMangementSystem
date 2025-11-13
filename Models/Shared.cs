using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeApi.Models
{
    public class Shared
    {
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Shared()
        {
            IsDeleted = false;
            CreatedAt = DateTime.Now;
            DeletedAt = null;
            
        }
    }
}
