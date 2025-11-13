using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeApi.ViewModels
{
    public class LeaveRequestDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="The Employee is required")]
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        [Required(ErrorMessage = "The Start Date is required")]
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? FromTime { get; set; }
        public DateTime? ToTime { get; set; }
        [Required(ErrorMessage = "The Reason is required")]
        public string Reason { get; set; }
        public string ApprovedByName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
