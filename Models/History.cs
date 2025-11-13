using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeApi.Models
{
    public class History
    {
        [Required]
        public int Id { get; set; }                 
        public string Name { get; set; } = "";   
        public string Description { get; set; } = "";
        [Required]
        public string EntityType { get; set; } = ""; 
        public DateTime? DeletedAt { get; set; }    
        public string ExtraInfo { get; set; } = "";


    }
    }
