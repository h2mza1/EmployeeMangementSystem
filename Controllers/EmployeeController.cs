using EmployeeApi.Data;
using EmployeeApi.Models;
using EmployeeApi.Services;

//using EmployeeApi.OkModels;
using EmployeeApi.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Drawing;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace EmployeeApi.Controllers
{


    [EnableCors(origins: "http://localhost:4200", headers: "*", methods: "*")]

    public class EmployeeController : ApiController
    {
        private readonly EmpDbContext empDbContext;
        private int user = 0;
        public EmployeeController() {
            empDbContext = new EmpDbContext();
            user = GetCurrentUserId();
        }
        [HttpGet]
       
         [Authorize(Roles="Manager,Admin")]
        [Route("api/Employee/GetAll")]
        public async Task<IHttpActionResult> GetAll()
        {
            var emps = new List<EmpDto>();
            var result = IsAdmin();
            if(result)
            {
             
                var res = await (from emp in empDbContext.Employees
                             join dept in empDbContext.Departments 
                             on emp.DeptId equals dept.Id 
                             join role in empDbContext.Roles
                             on emp.RoleId equals role.Id
                             where (emp.Id != user && !emp.IsDeleted) 
                             select new EmpDto
                             {
                                Id= emp.Id,
                                Name= emp.Name,
                                Email=emp.Email,
                                Phone=emp.Phone,
                                DeptName= dept.Name , 
                                Salary= emp.BaseSalary,
                                DeptId = emp.DeptId,
                                EndDate = emp.EndDate,
                                StartDate = emp.StartDate,
                                RoleId = emp.RoleId,
                                RoleName=role.Name,
                             }
                          ).ToListAsync();

                emps = res;
            }
            else
            {
                emps = await empDbContext.Employees
             .Where(e => !e.IsDeleted && e.Id != user && e.Role.Name=="Employee")
             .AsNoTracking()
             .Select(emp => new EmpDto
             {
                 Id = emp.Id,
                 Name = emp.Name,
                 Email = emp.Email,
                 Phone = emp.Phone,
                 DeptName = emp.Department.Name,
                 Salary = emp.BaseSalary,
                 DeptId = emp.DeptId,
                 EndDate = emp.EndDate,
                 StartDate = emp.StartDate,
                 RoleId = emp.RoleId,
                 RoleName = emp.Role.Name,
             })
             .ToListAsync();
                var res = await (from emp in empDbContext.Employees
                                 join role in empDbContext.Roles
                                 on emp.RoleId equals role.Id
                                 join dept in empDbContext.Departments
                                 on emp.DeptId equals dept.Id
                                 where (!emp.IsDeleted && emp.Id != user && role.Name.ToLower() == "employee")
                                 select new EmpDto
                                 {
                                     Id = emp.Id,
                                     Name = emp.Name,
                                     Email = emp.Email,
                                     Phone = emp.Phone,
                                     DeptName = dept.Name,
                                     Salary = emp.BaseSalary,
                                     DeptId = emp.DeptId,
                                     EndDate = emp.EndDate,
                                     StartDate = emp.StartDate,
                                     RoleId = emp.RoleId,
                                     RoleName = role.Name,
                                 }).ToListAsync();
                emps = res;
                                 
            }
          
            if (!emps.Any())
                return NotFound();
         
            return Ok(emps);
        }

        [HttpGet]
        [Route("api/Employee/GetById/{id}")]
        [Authorize]
        public async Task<IHttpActionResult> GetById(int id)
        {   var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;
            var monthStart = new DateTime(year, month, 1);
            var nextMonth = monthStart.AddMonths(1);

            var allDays = Enumerable.Range(0, (nextMonth - monthStart).Days)
                                    .Select(offset => monthStart.AddDays(offset));

            var workingDays = allDays.Where(d => d.DayOfWeek != DayOfWeek.Friday &&
                                                 d.DayOfWeek != DayOfWeek.Saturday)
                                     .ToList();

            int totalWorkingDays = workingDays.Count;
            const int hoursPerDay = 8;
            int expectedWorkHours = totalWorkingDays * hoursPerDay;
            var attendances = (await empDbContext.Attendances
              .Where(a => a.EmployeeId == id &&
                          a.Status == true &&
                          a.CheckInTime.HasValue &&
                          a.CheckOutTime.HasValue &&
                          !a.IsDeleted &&
                          a.CheckInTime.Value >= monthStart &&
                          a.CheckInTime.Value < nextMonth)
              .ToListAsync())
              .Where(x => x.Date.DayOfWeek != DayOfWeek.Friday && x.Date.DayOfWeek != DayOfWeek.Saturday)
             ;

            int totalWorkHours = 0;
            foreach (var att in attendances)
            {
                DateTime checkIn = DateTime.SpecifyKind(att.CheckInTime.Value, DateTimeKind.Local);
                DateTime checkOut = DateTime.SpecifyKind(att.CheckOutTime.Value, DateTimeKind.Local);
                var diff = checkOut - checkIn;
                if ((int)diff.TotalHours > hoursPerDay)
                    totalWorkHours += 8;
                else
                    totalWorkHours += (int)diff.TotalHours;
            }

            int totalAbsentHours = expectedWorkHours - totalWorkHours;
            if (IsAdmin() || IsManager())
            {
                var employee = await (from emp in empDbContext.Employees
                                      where (emp.Id == id && !emp.IsDeleted)
                                      select new EmpDto
                                      {
                                          Id = emp.Id,
                                          Name = emp.Name,
                                          Email = emp.Email,
                                          Phone = emp.Phone,
                                          DeptName = emp.Department.Name,
                                          Salary = emp.BaseSalary,
                                          DeptId = emp.DeptId,
                                          EndDate = emp.EndDate,
                                          StartDate = emp.StartDate,
                                          RoleId = emp.RoleId,
                                          RoleName = emp.Role.Name,
                                          expectedWorkHours = expectedWorkHours,
                                          workingHours = totalWorkHours
                                      }
                                      ).FirstOrDefaultAsync();
                if (employee == null) return NotFound();
          
                return Ok(employee);
            }
            else if(GetCurrentUserId()==id)
            {
                var employee = await (from emp in empDbContext.Employees
                                      where (emp.Id == id && !emp.IsDeleted)
                                      select new EmpDto
                                      {
                                          Id = emp.Id,
                                          Name = emp.Name,
                                          Email = emp.Email,
                                          Phone = emp.Phone,
                                          DeptName = emp.Department.Name,
                                          Salary = emp.BaseSalary,
                                          DeptId = emp.DeptId,
                                          EndDate = emp.EndDate,
                                          StartDate = emp.StartDate,
                                          RoleId = emp.RoleId,
                                          RoleName = emp.Role.Name,
                                          expectedWorkHours = expectedWorkHours,
                                          workingHours = totalWorkHours
                                      }
                                      ).FirstOrDefaultAsync(); ;
                if (employee == null) return NotFound();
          ;
                return Ok(employee);
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
            //await empDbContext.Employees.AnyAsync(e => e.Email.ToLower() == model.Email.ToLower());
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var checkEmail = await (from emp in empDbContext.Employees
                                    where (!emp.IsDeleted && emp.Email.ToLower() == model.Email.ToLower())
                                    select emp.Email
                                    ).AnyAsync();
            if(model.DeptId==0)
                return BadRequest("plz select the department");
            if (model.RoleId == 0)
                return BadRequest("plz select the role");
            if (checkEmail)
                return BadRequest($"email {model.Email} already founded");
            var recored = new Employee
            {
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                DeptId = model.DeptId,
                RoleId=model.RoleId,
                EndDate = model.EndDate,
                StartDate = model.StartDate,

            };
            if (model.Salary != 0 )
                recored.BaseSalary = model.Salary;
            if(model.Password != null)
            {
                var hash = HashingService.HashPassword(model.Password);
                recored.Password = hash;
            }
            else
            {
                var hash = HashingService.HashPassword("hamza");
                recored.Password = hash;
            }
            try
            {
                empDbContext.Employees.Add(recored);
                await empDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");

            }
            var role = await empDbContext.Roles
                .Where(x => x.Id == recored.RoleId)
                .Select(x => x.Name)
                .FirstOrDefaultAsync();

            var dept = await empDbContext.Departments
              .Where(x => x.Id == recored.DeptId)
              .Select(x => x.Name)
              .FirstOrDefaultAsync();

            var response = new EmpDto
            {
                Id = recored.Id,
                Name = recored.Name ?? "",
                Email = recored.Email ?? "",
                Phone = recored.Phone,
                RoleId = recored.RoleId,
                DeptId = recored.DeptId,
                DeptName = dept ?? "",
                RoleName = role ?? "",
                 EndDate = recored.EndDate,
                StartDate = recored.StartDate,
                Salary = recored.BaseSalary

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

            var employee = await (from emp in empDbContext.Employees
                             where emp.Id == id && !emp.IsDeleted
                             select new  EmpDto
                             {
                              Id=   emp.Id,
                                Name=  emp.Name,
                                 Email= emp.Email,
                                 Phone= emp.Phone,
                                  Salary= emp.BaseSalary,
                                  DeptId= emp.DeptId,
                                  EndDate= emp.EndDate,
                                StartDate=  emp.StartDate,
                                RoleId= emp.RoleId,
                                DeptName = emp.Department.Name,
                                RoleName=emp.Role.Name
                             }).FirstOrDefaultAsync();
            if (employee == null)
                return NotFound();
            var empres = Mapping.MapTo(employee);
            var checkEmail = await (from emp in empDbContext.Employees
                                    where (!emp.IsDeleted && emp.Email.ToLower() == model.Email.ToLower() && emp.Id != id)
                                    
                                    select emp.Email
                                  )
                                  
                                  .AnyAsync();
            if (checkEmail)
                return BadRequest($"email {model.Email} already founded");
            empres.Email = model.Email;
            empres.Phone = model.Phone;
            empres.Name = model.Name;
           
            if(IsAdmin() || IsManager())
            {
                empres.DeptId = model.DeptId;
              

            }
            if (IsAdmin() )
            {
                empres.RoleId = model.RoleId;
                if(model.Salary != 0)
                    empres.BaseSalary = model.Salary;
                if (model.StartDate.ToString() != null)
                    empres.StartDate = model.StartDate;
                if (model.EndDate.ToString() != null)
                    empres.EndDate = model.EndDate;
                if (!string.IsNullOrEmpty(model.Password))
                    try
                    {
                        var hash = HashingService.HashPassword(model.Password);
                        empres.Password = hash;
                    }
                    catch (Exception ex) 
                    {
                        return BadRequest(ex.Message);
                    }
            }
            else
            {

                var hash = HashingService.HashPassword("hamza");
                empres.Password = hash;
            }
                
            await empDbContext.SaveChangesAsync();
             empDbContext.Employees.AddOrUpdate(empres);
            var response = Mapping.MapTo(empres);
            response.RoleName = employee.RoleName;
            response.DeptName = employee.DeptName;
            await empDbContext.SaveChangesAsync();
            return Ok(response);
        }

        [HttpDelete]
        [Route("api/Employee/Delete/{id}")]
       [Authorize(Roles = "Admin,Manager")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var emp = await (from e in empDbContext.Employees
                             where (e.Id == id && !e.IsDeleted) 
                             select e).FirstOrDefaultAsync();
            if (emp == null)
                return BadRequest($"No employee founded with id = {id}");

            emp.IsDeleted = true;
            await empDbContext.SaveChangesAsync();
            return Ok();
        }
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
