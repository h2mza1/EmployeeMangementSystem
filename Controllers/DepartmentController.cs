using EmployeeApi.Data;
using EmployeeApi.Models;
using EmployeeApi.Services;
using EmployeeApi.ViewModels;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;


namespace EmployeeApi.Controllers
{


    [EnableCors(origins: "http://localhost:4200", headers: "*", methods: "*")]

    [Authorize]
    public class DepartmentController : ApiController
    {
        private readonly EmpDbContext context;
        //public DepartmentController(EmpDbContext dbContext) { context = dbContext; }
        public DepartmentController() { context = new EmpDbContext(); }

        [HttpGet]
       [Authorize(Roles = "Admin,Manager")]
        [Route("api/Department/GetAll")]
        public async Task<IHttpActionResult> getAll()
        {
            var depts = await (from dept in context.Departments
                               where (!dept.IsDeleted)
                               select new DeptDto
                               {
                                   Id = dept.Id,
                                   Name = dept.Name
                               }).AsNoTracking()
                               .ToListAsync();
            if(!depts.Any())
                return NotFound();
         
            return Ok(depts);
        }
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        [Route("api/Department/GetById/{id}")]
        public async Task<IHttpActionResult> GetById(int id)


        {   //await context.Departments
            //    .AsNoTracking()
            //    .Where(x => !x.IsDeleted && x.Id == id)
            //    .FirstOrDefaultAsync();
            var dept = await (from d in context.Departments
                              where (d.Id == id && !d.IsDeleted)
                              select new DeptDto
                              {
                                  Id = d.Id,
                                  Name = d.Name
                              }).AsNoTracking()
                              .ToListAsync();
            

            if (dept == null)
                return NotFound();
         
            return Ok(dept);
        }
        [HttpPost]
       [Authorize(Roles ="Admin")]
        [Route("api/Department/Create")]
        public async Task<IHttpActionResult> Create([FromBody]DeptDto model)
        {
            if (string.IsNullOrEmpty(model.Name))
                return BadRequest("Name is required");
            var dept = new Department { Name = model.Name };
            var checkName = await context.Departments
                .AnyAsync(x=> x.Name.ToLower().Trim() == model.Name.ToLower().Trim());
            if(checkName)
            return BadRequest($"the department {model.Name} already founded");
            context.Departments.Add(dept);
            await context.SaveChangesAsync();
            return Ok(Mapping.MapTo(dept));

        }
        [HttpDelete]
        [Route("api/Department/Delete/{id}")]
       [Authorize(Roles = "Admin")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var dept = await (from d in context.Departments
                              where (d.Id == id && !d.IsDeleted)
                              select d).FirstOrDefaultAsync();
                              
            if (dept == null)
                return BadRequest($"Department with id : {id} not found" );
            var check = await (from e in context.Employees
                               where (e.DeptId == dept.Id)
                               select e).AnyAsync();
            if (check)
                return BadRequest($"Department {dept.Name} has many employee");
            dept.IsDeleted= true;
            await context.SaveChangesAsync();

            return Ok(Mapping.MapTo(dept));
        }
        [HttpPut]
        [Route("api/Department/update/{id}")]
    [Authorize(Roles = "Admin")]
        public async Task<IHttpActionResult> Update(int id,[FromBody] DeptDto model)
        {
            if (string.IsNullOrEmpty(model.Name))
                return BadRequest();
            var dept = await (from d in context.Departments
                              where (d.Id == id && !d.IsDeleted)
                              select d).FirstOrDefaultAsync();
            if (dept == null)
                return BadRequest($"Department with id : {id} not found");
            dept.Name = model.Name;
            await context.SaveChangesAsync();
            return Ok(Mapping.MapTo(dept));
        }
    }
}
