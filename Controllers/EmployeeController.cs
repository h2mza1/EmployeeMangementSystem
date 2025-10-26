using EmployeeApi.Data;
using EmployeeApi.Models;
using EmployeeApi.Services;

//using EmployeeApi.OkModels;
using EmployeeApi.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.UI.WebControls;
using WebApplication1.Filters;

namespace EmployeeApi.Controllers
{
    [AllowCrossSiteJson]
    //[Authorize(Roles ="Admin")]
    public class EmployeeController : ApiController
    {
        private readonly EmpDbContext empDbContext;
        public EmployeeController() {
            empDbContext = new EmpDbContext();
        }
        [HttpGet]
        [Route("api/Employee/GetAll")]
        [Authorize(Roles="Admin,Manager")]
        public async Task<IHttpActionResult> GetAll()
        {
            var emps = await empDbContext.Employees
                .Where(e=>!e.IsDeleted)
                .AsNoTracking()
                .Include(e=> e.Department)
                .ToListAsync();
            if (!emps.Any())
                return NotFound();
            var employees = emps.Select(e=> new EmpDto
            {
                Id = e.Id,
                DeptId = e.DeptId,
                DeptName = e.Department.Name ?? "",
                Email   = e.Email ?? "",
                Name = e.Name ?? "",
                Phone = e.Phone ,
                RoleId = e.RoleId,
                RoleName = e.Role.Name ?? "",
                EndDate = e.EndDate,
                StartDate = e.StartDate,

            });
            return Ok(employees);
        }

        [HttpGet]
        [Route("api/Employee/GetById/{id}")]
        [Authorize]
        public async Task<IHttpActionResult> GetById(int id)
        {
            if(IsAdmin() || IsManager())
            {
                var employee = await empDbContext.Employees.FindAsync(id);
                if (employee == null) return NotFound();
                var res = new EmpDto
                {
                    Id = employee.Id,
                    Name = employee.Name,
                    Phone = employee.Phone,
                    RoleId = employee.RoleId,
                    DeptId = employee.DeptId,
                    Email = employee.Email,
                    DeptName = employee.Department.Name,
                    RoleName = employee.Role.Name,
                    EndDate = employee.EndDate,
                    StartDate = employee.StartDate,
                };
                return Ok(res);
            }
            else if(GetCurrentUserId()==id)
            {
                var employee = await empDbContext.Employees.FindAsync(id);
                if (employee == null) return NotFound();
                var res = new EmpDto
                {
                    Id = employee.Id,
                    Name = employee.Name,
                    Phone = employee.Phone,
                    RoleId = employee.RoleId,
                    DeptId = employee.DeptId,
                    Email = employee.Email,
                    DeptName = employee.Department.Name,
                    RoleName = employee.Role.Name,
                    EndDate = employee.EndDate,
                    StartDate = employee.StartDate,
                };
                return Ok(res);
            }
            else
            {
                return Unauthorized();
            }
          

        }
        [HttpPost]
        [Route("api/Employee/Create")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IHttpActionResult> Create( [FromBody] EmpDto model)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            var checkEmail = await empDbContext.Employees.AnyAsync(e => e.Email.ToLower() == model.Email.ToLower());
            if (checkEmail)
                return BadRequest($"email {model.Email} already founded");
            var emp = new Employee
            {
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                DeptId = model.DeptId,
                RoleId=model.RoleId,
                EndDate = model.EndDate,
                StartDate = model.StartDate,

            };
             var hash = HashingService.HashPassword(model.Password);
            emp.Password = hash;
            try
            {
                empDbContext.Employees.Add(emp);
                await empDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");

            }
            var response = new EmpDto
            {
                Id = emp.Id,
                Name = emp.Name ?? "",
                Email = emp.Email ?? "",
                Phone = emp.Phone,
                RoleId = emp.RoleId,
                DeptId = emp.DeptId,
                DeptName = emp.Department.Name ?? "",
                RoleName = emp.Role.Name ?? "",
                  EndDate = emp.EndDate,
                StartDate = emp.StartDate,

            };
            return Ok(response);
        }
       
        [HttpPut]
        [Route("api/Employee/update/{id}")]
        [Authorize(Roles = "Admin,Manager,Employee")]
        public async Task<IHttpActionResult> Edit([FromBody] EmpDto model,int id)
        {
            if (!IsAdmin() && !IsManager() && GetCurrentUserId() != id)
                return Unauthorized();
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var emp = await empDbContext.Employees.FirstOrDefaultAsync(e => e.Id == id);
            if (emp == null)
                return NotFound();
            var checkEmail = await empDbContext.Employees
                
                .AnyAsync(e => e.Email.ToLower() == model.Email.ToLower() && e.Id !=id)
                ;
            if (checkEmail)
                return BadRequest($"email {model.Email} already founded");
            emp.Email = model.Email;
            emp.Phone = model.Phone;
            emp.Name = model.Name;
           
            if(IsAdmin() || IsManager())
            {
                emp.DeptId = model.DeptId;
                emp.RoleId = model.RoleId;  

            }
            await empDbContext.SaveChangesAsync();
            var response = new EmpDto
            {
                Id = emp.Id,
                Name = emp.Name ?? "",
                Email = emp.Email ?? "",
                Phone = emp.Phone,
                RoleId = emp.RoleId,
                DeptId = emp.DeptId,
                DeptName = emp.Department.Name ?? "",
                RoleName = emp.Role.Name ?? ""


            };
            return Ok(response);
        }

        [HttpDelete]
        [Route("api/Employee/Delete")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var emp = await empDbContext.Employees.FirstOrDefaultAsync(e => e.Id == id);
            if (emp == null)
                return NotFound();

            emp.IsDeleted = true;
            await empDbContext.SaveChangesAsync();
            return Ok();
        }
        private bool IsAdmin()
        {
            var identity = User.Identity as ClaimsIdentity;
            var role= identity.FindFirst(ClaimTypes.Role).Value;
            if(role.ToLower() == "admin")
                return true;
            return false;
        }
        private bool IsManager()
        {
            var identity = User.Identity as ClaimsIdentity;
            var role= identity.FindFirst(ClaimTypes.Role).Value;
            if(role.ToLower() == "manager")
                return true;
            return false;
        }
        private int GetCurrentUserId()
        {
            var identity = User.Identity as ClaimsIdentity;

            return int.Parse(identity.FindFirst(ClaimTypes.NameIdentifier).Value);
        }
       
    }
}
