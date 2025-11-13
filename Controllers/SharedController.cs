using EmployeeApi.Data;
using EmployeeApi.Models;
using EmployeeApi.Services;
using EmployeeApi.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web.Http;

namespace EmployeeApi.Controllers
{
    public class SharedController : ApiController
    {

        public bool IsAdmin()
        {
            var identity = User.Identity as ClaimsIdentity;
            var role = identity.FindFirst(ClaimTypes.Role).Value;
            if (role.ToLower() == "admin")
                return true;
            return false;
        }
        public bool IsManager()
        {
            var identity = User.Identity as ClaimsIdentity;
            var role = identity.FindFirst(ClaimTypes.Role).Value;
            if (role.ToLower() == "manager")
                return true;
            return false;
        }
        public int GetCurrentUserId()
        {
            var identity = User.Identity as ClaimsIdentity;

            return int.Parse(identity.FindFirst(ClaimTypes.NameIdentifier).Value ?? "");
        }
    }
}
