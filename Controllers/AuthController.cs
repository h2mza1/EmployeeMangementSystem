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
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;


namespace EmployeeApi.Controllers
{   
    [EnableCors(origins: "http://localhost:4200", headers: "*", methods: "*")]
    public class AuthController : SharedController
    {
        private TimeSpan startTime;
        private TimeSpan endTime;
        private readonly EmpDbContext context;
        public AuthController()
        {
            context = new EmpDbContext();
            startTime = new TimeSpan(8,0,0);//AM
            endTime = new TimeSpan(17,0,0);//PM
        }
            
        [HttpPost]
        [Route("api/Auth/Login")]
        [AllowAnonymous]
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
                var existing = await context.Attendances
                    .FirstOrDefaultAsync(a => a.EmployeeId == user.Id && a.Date == DateTime.Today && !a.IsDeleted);

                if (existing == null)
                {
                    var nowTime = DateTime.Now.TimeOfDay;

                    if (nowTime < startTime || nowTime > endTime)
                        return Ok(new { success = "true", data = token });
                    var attendance = new Attendance
                    {
                        EmployeeId = user.Id,
                        Date = DateTime.Today,
                        CheckInTime = DateTime.Now,
                        Status = true,
                    };
                    context.Attendances.Add(attendance);
                    await context.SaveChangesAsync();
                }
               
                return Ok(new {success = "true",data=token});

            }
            return BadRequest("invalid login");
        }   

        [HttpGet]
        [Route("api/Auth/IsAdmin")]
        [Authorize]
        public IHttpActionResult IsUserAdmin()
        {
            return Ok(IsAdmin());
        }

    }
}
