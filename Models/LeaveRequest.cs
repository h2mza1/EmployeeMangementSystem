using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeApi.Models
{
    public enum RequestType
    {
        Annual,
        Sick,
      
    }

    public enum RequestStatus
    {
        Pending = 1,     
        Approved = 2,    
        Rejected = 3    
    }
    public class LeaveRequest  :Shared
    {
        public int Id { get; set; }
        public RequestType Type { get; set; } 
        public RequestStatus Status { get; set; } = RequestStatus.Pending;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Reason { get; set; }
        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
        public DateTime? FromTime { get; set; }
        public DateTime? ToTime { get; set; }
        [ForeignKey("ApprovedBy")]
        public int? ApprovedById { get; set; }
        public virtual Employee ApprovedBy {  set; get; }

    }
}
