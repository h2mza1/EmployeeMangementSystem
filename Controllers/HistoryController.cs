using EmployeeApi.Data;
using EmployeeApi.Models;
using EmployeeApi.ViewModels;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
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


    public class HistoryController : ApiController
    {
        private readonly EmpDbContext context;
        public HistoryController() { context = new EmpDbContext(); }

        [HttpPost]
        [Route("api/History/Restore")]
        public async Task<IHttpActionResult> Restore([FromBody] History model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (model.EntityType.ToLower() == "employee")
            {
                //await context.Employees.FirstOrDefaultAsync(x => x.Id == model.Id);
                var emp = await (from x in context.Employees
                                 where (x.Id == model.Id)
                                 select x).FirstOrDefaultAsync();
                if (emp == null)
                {
                    return NotFound();
                }
                emp.IsDeleted = false;
                await context.SaveChangesAsync();
                return Ok();
            }
            else if (model.EntityType.ToLower() == "attendance")
            {
                var attend = await (from x in context.Attendances
                                    where (x.Id == model.Id)
                                    select x).FirstOrDefaultAsync();
                if (attend == null)
                {
                    return BadRequest(ModelState);
                }
                attend.IsDeleted = false;
                await context.SaveChangesAsync();
                return Ok();
            }
            else if (model.EntityType.ToLower() == "department")
            {
                var dept = await (from x in context.Departments
                                  where (x.Id == model.Id)
                                  select x).FirstOrDefaultAsync();
                if (dept == null)
                    return BadRequest(ModelState);
                dept.IsDeleted = false;
                await context.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpGet]
        [Route("api/History/GetAll")]
        public async Task<IHttpActionResult> GetAll()
        {
            //context.Employees
            //    .Where(x => x.IsDeleted)
            //    .Select(x => new History
            //    {
            //        Id = x.Id,
            //        Name = x.Name,
            //        EntityType = "Employee",
            //        Description = "Department : " + x.Department.Name + " Phone : " + x.Phone.ToString() + " Email : " + x.Email,
            //        DeletedAt = x.DeletedAt ?? null,

            //    }).ToListAsync();
            var response = new HistoryDto();
            var empList = await (from x in context.Employees
                                 where (x.IsDeleted)
                                 select new History
                                 {
                                     Id = x.Id,
                                     Name = x.Name,
                                     EntityType = "Employee",
                                     Description = "Department : " + x.Department.Name + " Phone : " + x.Phone.ToString() + " Email : " + x.Email,
                                     DeletedAt = x.DeletedAt ?? null,
                                 }).ToListAsync();
            //context.Attendances
            //    .Where(x => x.IsDeleted)
            //    .Select(x => new History
            //    {
            //        Id = x.Id,
            //        EntityType = "Attendance",
            //        Name = x.Employee.Name ?? "",
            //        Description = " Phone : " + x.Employee.Phone.ToString() + " Email: " + x.Employee.Email,
            //        ExtraInfo = "Date :" + x.Date,
            //        DeletedAt = x.DeletedAt ?? null,

            //    }).ToListAsync();
            var attendList = await (from x in context.Attendances
                                    where (x.IsDeleted)
                                    select new History
                                    {
                                        Id = x.Id,
                                        EntityType = "Attendance",
                                        Name = x.Employee.Name ?? "",
                                        Description = " Phone : " + x.Employee.Phone.ToString() + " Email: " + x.Employee.Email,
                                        ExtraInfo = "Date :" + x.Date,
                                        DeletedAt = x.DeletedAt ?? null,
                                    }).ToListAsync();
            //await context.Departments
            //   .Where(x => x.IsDeleted)
            //   .Select(x => new History
            //   {
            //       Id = x.Id,
            //       Name = x.Name,
            //       EntityType = "Department",
            //       DeletedAt = x.DeletedAt ?? null,


            //   }).ToListAsync();
            var deptList = await (from x in context.Departments
                                  where (x.IsDeleted)
                                  select new History
                                  {

                                      Id = x.Id,
                                      Name = x.Name,
                                      EntityType = "Department",
                                      DeletedAt = x.DeletedAt ?? null,
                                  }).ToListAsync();
            if (empList.Any())
                response.Employee.AddRange(empList);
            if (attendList.Any())
                response.Attendance.AddRange(attendList);
            if (deptList.Any())
                response.Department.AddRange(deptList);
            return Ok(response);
        }

        [HttpDelete]
        [Route("api/History/Delete")]
        public async Task<IHttpActionResult> Delete(History model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (model.EntityType.ToLower() == "employee")
            {
                var emp = await (from e in context.Employees
                                 where e.Id == model.Id
                                 select e).FirstOrDefaultAsync();
                if (emp == null)
                {
                    return NotFound();
                }
                context.Employees.Remove(emp);
                await context.SaveChangesAsync();
                return Ok();
            }
            else if (model.EntityType.ToLower() == "attendance")
            {
                var attend = await (from x in context.Attendances where (x.Id == model.Id) select x).FirstOrDefaultAsync();
                if (attend == null)
                {
                    return BadRequest(ModelState);
                }
                context.Attendances.Remove(attend);
                await context.SaveChangesAsync();
                return Ok();
            }
            else if (model.EntityType.ToLower() == "department")
            {
                var dept = await (from x in context.Departments where (x.Id == model.Id) select x).FirstOrDefaultAsync();
                if (dept == null)
                    return BadRequest(ModelState);
                context.Departments.Remove(dept);
                await context.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
        //    [HttpGet]
        //    [Route("api/History/GetByEntity")]
        //    public async Task<IHttpActionResult> GetByEntity([FromBody]string entity)
        //    {
        //        if (entity == null)
        //            return BadRequest("Entity name is required");
        //        switch (entity.ToLower())
        //        {
        //            case "employee":
        //                {
        //                    var response = await context.Employees
        //                        .Where(x => x.IsDeleted)
        //                        .Select(x => new History
        //                        {
        //                            Id = x.Id,
        //                            Name = x.Name,

        //                        }
        //        }
        //    }
    } //}
}
