using EmployeeApi.Data;
using EmployeeApi.Models;
using EmployeeApi.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace EmployeeApi.Controllers
{
    public class LeaveController : SharedController
    {
        private readonly EmpDbContext context;
        public LeaveController() { context = new EmpDbContext(); }

        // Helper to map LeaveRequest to DTO
        private async Task<LeaveRequestDto> MapToDto(LeaveRequest record)
        {
            string approvedByName = record.ApprovedById.HasValue
                ? await context.Employees
                    .Where(e => e.Id == record.ApprovedById.Value)
                    .Select(e => e.Name)
                    .FirstOrDefaultAsync()
                : "";

            return new LeaveRequestDto
            {
                Id = record.Id,
                EmployeeId = record.EmployeeId,
                EmployeeName = record?.Employee?.Name ?? "",
                Type = record?.Vacation.Name ?? "",
                Status = record.Status.ToString(),
                StartDate = record.StartDate,
                EndDate = record.EndDate,
                FromTime = record.FromTime,
                ToTime = record.ToTime,
                Reason = record.Reason,
                ApprovedByName = approvedByName ?? "",
                CreatedAt = record.CreatedAt,
                VacationId = record.VacationId,
            };
        }

        [HttpPost]
        [Route("api/Leave/Add")]
        public async Task<IHttpActionResult> Add(LeaveRequestDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            bool check = await context.LeaveRequests
                .AnyAsync(x => x.StartDate == model.StartDate && x.EmployeeId == model.EmployeeId && !x.IsDeleted);

            if (check)
                return BadRequest("The Employee has a leave record on this day");
            if(model.StartDate.DayOfWeek == DayOfWeek.Friday || model.StartDate.DayOfWeek == DayOfWeek.Saturday)
                return BadRequest("Friday and Sunday is a forma vacation");

            var leave = new LeaveRequest
            {
                EmployeeId = model.EmployeeId,
                Status = RequestStatus.Pending,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                FromTime = model.FromTime,
                ToTime = model.ToTime,
                Reason = model.Reason,
                CreatedAt = DateTime.Now,
                VacationId = model.VacationId,
            };

            context.LeaveRequests.Add(leave);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        [Route("api/Leave/GetAll")]
        public async Task<IHttpActionResult> GetAll()
        {
            var requests = await context.LeaveRequests
                .Where(l => !l.IsDeleted)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();

            var result = new List<LeaveRequestDto>();
            foreach (var r in requests)
                result.Add(await MapToDto(r));

            return Ok(result);
        }

        [HttpGet]
        [Route("api/Leave/GetByEmployee/{id}")]
        public async Task<IHttpActionResult> GetByEmployee(int id)
        {
            var requests = await context.LeaveRequests
                .Where(l => l.EmployeeId == id && !l.IsDeleted)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();

            var result = new List<LeaveRequestDto>();
            foreach (var r in requests)
                result.Add(await MapToDto(r));

            return Ok(result);
        }

        [HttpPut]
        [Route("api/Leave/Approve/{id}")]
        public async Task<IHttpActionResult> Approve(int id)
        {
            var record = await context.LeaveRequests.FirstOrDefaultAsync(x => x.Id == id);
            if (record == null) return BadRequest("No Request Found");

            record.Status = RequestStatus.Approved;
            record.ApprovedById = GetCurrentUserId();
            await context.SaveChangesAsync();

            return Ok(await MapToDto(record));
        }

        [HttpPut]
        [Route("api/Leave/Reject/{id}")]
        public async Task<IHttpActionResult> Reject(int id)
        {
            var record = await context.LeaveRequests.FirstOrDefaultAsync(x => x.Id == id);
            if (record == null) return BadRequest("No Request Found");

            record.Status = RequestStatus.Rejected;
            record.ApprovedById = GetCurrentUserId();
            await context.SaveChangesAsync();

            return Ok(await MapToDto(record));
        }

        [HttpDelete]
        [Route("api/Leave/Delete/{id}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var record = await context.LeaveRequests.FirstOrDefaultAsync(x => x.Id == id);
            if (record == null) return BadRequest("No Request Found");

            record.IsDeleted = true;
            await context.SaveChangesAsync();

            return Ok(await MapToDto(record));
        }

        [HttpPut]
        [Route("api/Leave/Update/{id}")]
        public async Task<IHttpActionResult> Update(int id, LeaveRequestDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var record = await context.LeaveRequests.FirstOrDefaultAsync(x => x.Id == id);
            if (record == null) return BadRequest("No Request Found");

            record.StartDate = model.StartDate;
            record.EndDate = model.EndDate;
            record.FromTime = model.FromTime;
            record.ToTime = model.ToTime;
            record.Reason = model.Reason;
            //record.Type =  model.;
            record.Status = (RequestStatus)Enum.Parse(typeof(RequestStatus), model.Status);
            record.VacationId = model.VacationId;

            await context.SaveChangesAsync();

            return Ok(await MapToDto(record));
        }

        [HttpGet]
        [Route("api/Leave/getRequestsByUserId/{id}")]
        public async Task<IHttpActionResult> GetByUserId(int? id)
        {
            if (!id.HasValue) return BadRequest("Invalid Employee Id");

            var records = await context.LeaveRequests
                .Where(x => !x.IsDeleted && x.EmployeeId == id)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            var result = new List<LeaveRequestDto>();
            foreach (var r in records)
                result.Add(await MapToDto(r));

            return Ok(result);
        }
        //Vacation
        [HttpGet]
        [Route("api/Vacation/GetAll")]
        public async Task<IHttpActionResult> GetAllVacations()
        {
            var records = await context.Vacations.ToListAsync();
            if(records.Any())
            {
                var result = records.Select(x => new VacationDto
                {
                    Duration = x.Duration,
                    Id = x.Id,
                    Name = x.Name,
                });
                return Ok(result);
            }
            return Ok(new List<VacationDto>()); 
        }
    }
}
