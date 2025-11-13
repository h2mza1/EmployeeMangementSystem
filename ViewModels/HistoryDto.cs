using EmployeeApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeApi.ViewModels
{
    public class HistoryDto
    {
        public List<History> Employee {  get; set; } = new List<History>();
        public List<History> Department { get; set; } = new List<History>();
        public List<History> Attendance { get; set; } = new List<History>();


    }
}
