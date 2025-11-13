using EmployeeApi.Data;
using EmployeeApi.Models;
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
    [Authorize(Roles = "Admin")]

    [AllowAnonymous]
    public class RoleController : SharedController
    {
        private readonly EmpDbContext context;
        public RoleController() { context = new EmpDbContext(); }

        [HttpGet]
        [Route("api/Role/GetAll")]
        public async Task<IHttpActionResult> GetAll()
        {
            var roles = await context.Roles
                .Where(r=> !r.IsDeleted)
                .Select(r=> new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name,
                })
                .ToListAsync();
            if (!roles.Any())
                return NotFound();
            return Ok(roles);
        }
   
        [HttpGet]
        [Route("api/Role/GetById/{id}")]
        public async Task<IHttpActionResult> GetById(int id)
        {
            var role = await context.Roles
                .Where(r => r.Id == id && !r.IsDeleted)
                .Select(r=> new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    
                }).FirstOrDefaultAsync();
                
            if (role == null) return NotFound();
            return Ok(role);
        }
        [HttpGet]
        [Route("api/Role/GetByName")]
        public async Task<IHttpActionResult> GetByName([FromBody]RoleDto model)
        {
            var role = await context.Roles
                .Where(r=> r.Name.ToLower() == model.Name.ToLower() && !r.IsDeleted)
                .Select(r=> new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name,
                }).FirstOrDefaultAsync();  
            if (role == null) return NotFound();
            return Ok(role);
        }
        [HttpPut]
        [Route("api/Role/Update/{id}")]
        public async Task<IHttpActionResult> Update(int id,[FromBody]RoleDto model)
        {
            if(!ModelState.IsValid) 
                return BadRequest(ModelState);
            var  exisistRole = await context.Roles.FindAsync(id);
            if(exisistRole == null) return NotFound();
            var result = await context.Roles.AnyAsync(r => r.Name.ToLower() == model.Name.ToLower());
            if (!result )
            {
                exisistRole.Name = model.Name;
                try
                {
                    await context.SaveChangesAsync();
                    return Ok(exisistRole);
                }
                catch (Exception ex)
                {
                    return BadRequest($"Error: {ex.Message}");
                }
            }
            return BadRequest($"the role {model.Name} already founded");
        }
        [HttpDelete]
        [Route("api/Role/Delete/{id}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var role = await context.Roles.FindAsync(id);
            if (role == null) return NotFound();
            role.IsDeleted= true;
            await context.SaveChangesAsync();
            return Ok();

        }
        [HttpPost]
        [Route("api/Role/Create")]
        public async Task<IHttpActionResult> Create([FromBody]RoleDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = context.Roles
                .Any(r => r.Name.ToLower() == model.Name.ToLower());
            if (result)
                return BadRequest($"the role {model.Name} already founded ");
            try
            {
                var role = new Role
                {
                    Name = model.Name,
                };
                context.Roles.Add(role);
                await context.SaveChangesAsync();
                return Ok(role);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
        [HttpGet]
        [Route("api/Role/getUserRole")]
        [Authorize]
        public  async Task<IHttpActionResult> GetUserRole()
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized();
            var user = await context.Employees
                .FirstOrDefaultAsync(p => p.Id == userId);
            if (user == null) return NotFound();
            var role = user.Role.Name;
            return Ok(role);
        }


    }
}
