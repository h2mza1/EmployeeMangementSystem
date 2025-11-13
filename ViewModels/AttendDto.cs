using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeApi.ViewModels
{
    public class AttendDto
    {
        public int? Id { get; set; } = 0;
        public string EmployeeName { get; set; }
        public int EmployeeId { get; set; }
        public DateTime? CheckIn { get; set; } = null;
        public DateTime? CheckOut { get; set; }=null;
        public DateTime? Day { get; set; }   
        public TimeSpan? Duration { get; set; }
        public bool Status { get; set; } = false;

    }
}
