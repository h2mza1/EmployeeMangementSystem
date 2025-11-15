using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeApi.ViewModels
{
    public class VacationDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int Duration { get; set; }
    }
}
