using EmployeeApi.Data;
using EmployeeApi.Models;
using EmployeeApi.ViewModels;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WebApplication1.Filters;

namespace EmployeeApi.Controllers
{
    [AllowCrossSiteJson]
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
            var depts = await context.Departments
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .ToListAsync();
            var result = depts.Select(d => new DeptDto
            {
                Id = d.Id,
                Name = d.Name
            });
            return Ok(result);
        }
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        [Route("api/Department/GetById/{id}")]
        public async Task<IHttpActionResult> GetById(int id)
        {
            var dept = await context.Departments
                .AsNoTracking()
                .Where (x => !x.IsDeleted && x.Id == id)
                .FirstAsync();
            if (dept == null)
                return NotFound();
            var result = new DeptDto
            {
                Id = id,
                Name = dept.Name
            };
            return Ok(result);
        }
        [HttpPost]
        [Authorize(Roles ="Admin")]
        [Route("api/Department/Create")]
        public async Task<IHttpActionResult> Create([FromBody]DeptDto model)
        {
            if (string.IsNullOrEmpty(model.Name))
                return BadRequest("Name is required");
            var dept = new Department { Name = model.Name };
            context.Departments.Add(dept);
            await context.SaveChangesAsync();
            return Ok(dept);

        }
        [HttpDelete]
        [Route("api/Department/Delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var dept = await context.Departments.SingleOrDefaultAsync(x => x.Id == id);
            if (dept == null)
                return BadRequest($"Department with id : {id} not found" );
            dept.IsDeleted= true;
            await context.SaveChangesAsync();
            return Ok(dept);
        }
        [HttpPut]
        [Route("api/Department/update/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IHttpActionResult> Update(int id,[FromBody] DeptDto model)
        {
            if (string.IsNullOrEmpty(model.Name))
                return BadRequest();
            var dept = await context.Departments.SingleOrDefaultAsync(x=> x.Id == id);
            if (dept == null)
                return BadRequest($"Department with id : {id} not found");
            dept.Name = model.Name;
            await context.SaveChangesAsync();
            return Ok(dept);
        }
    }
}
