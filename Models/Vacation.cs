using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeApi.Models
{
    public class Vacation
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int Duration { get; set; }
    }
}
