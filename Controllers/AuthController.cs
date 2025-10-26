using EmployeeApi.Data;
using EmployeeApi.Services;
using EmployeeApi.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace EmployeeApi.Controllers
{
    public class AuthController : ApiController
    {   
        private readonly EmpDbContext context;
        public AuthController()
        {
            context = new EmpDbContext();
        }

        [HttpPost]
        [Route("api/Auth/Login")]
        public async Task<IHttpActionResult> Login(Login model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var user = await context.Employees.FirstOrDefaultAsync(x=> x.Email == model.Email);
            if (user == null)
                return BadRequest($"employee with email : {model.Email} not found");
            var result = false;
            try
            {
                result = HashingService.VerifyPassword(model.Password, user.Password);

            }
            catch (Exception ex)
            {
                result = false;
                return BadRequest(ex.Message);
            }
            if (result)
            {
                var token = JwtService.GenerateToken(user);
                return Ok(new {success = "true",data=token});

            }
            return Ok("invild login");
        }
    }
}
