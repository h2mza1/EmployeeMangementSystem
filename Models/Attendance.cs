using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeApi.Models
{
    public class Attendance : Shared
    {
        public int Id { get; set; }
        public DateTime? CheckInTime { get; set; } = null;
        public DateTime? CheckOutTime { get; set; }= null;
        public DateTime Date {  get; set; }= DateTime.Today;
        public int EmployeeId { get; set; }
        public bool Status { get; set; }= false;
        public virtual Employee Employee { get; set; }
       
       

    }
}
