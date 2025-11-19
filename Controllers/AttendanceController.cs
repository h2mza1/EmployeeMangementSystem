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
    public class AttendanceController : SharedController
    {
        private readonly EmpDbContext context;
        public AttendanceController()
        {
            context = new EmpDbContext();
        }
        [HttpPost]
        [Route("api/Attendance/CheckIn")]
        [Authorize]

        public async Task<IHttpActionResult> CheckIn()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                    return Unauthorized();

                //await context.Attendances
                //   .FirstOrDefaultAsync(a => a.EmployeeId == userId && a.Date == today);
                DateTime today = DateTime.Today;

                var existing = await (from a in context.Attendances
                                      where (a.EmployeeId == userId && a.Date == today && !a.IsDeleted) select a).AnyAsync();

                if (existing)
                    return BadRequest("you already checked in");

                var attendance = new Attendance
                {
                    EmployeeId = userId,
                    Date = today,
                    CheckInTime = DateTime.Now,
                    Status = true,
                };

                context.Attendances.Add(attendance);
                await context.SaveChangesAsync();

                return Ok(attendance.CheckInTime);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [HttpPost]
        [Route("api/Attendance/CheckOut")]
        [Authorize]
        public async Task<IHttpActionResult> CheckOut()
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized();
                
            DateTime today = DateTime.Today;
            //await context.Attendances
            //    .FirstOrDefaultAsync(x => x.EmployeeId == userId && x.Date == today && !x.IsDeleted);
            var existing = await (from x in context.Attendances
                                  where (x.EmployeeId == userId && x.Date == today && !x.IsDeleted)
                                  select x).FirstOrDefaultAsync();

            if (existing == null)
                return BadRequest("You need to check in first");

            if (existing.CheckOutTime.HasValue)
                return BadRequest("You already checked out");

            existing.CheckOutTime = DateTime.Now;
            await context.SaveChangesAsync();

            return Ok(existing.CheckOutTime);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        [Route("api/Attendance/GetAllAttendance")]
        public async Task<IHttpActionResult> GetAllAttendance()
        {
            try
            {
                //await context.Attendances
                //    .Where(x => !x.IsDeleted)
                //    .OrderByDescending(a => a.Date)
                //    .Select(x => new AttendDto
                //    {
                //        Id = x.Id,
                //        EmployeeId = x.EmployeeId,
                //        CheckIn = x.CheckInTime,
                //        CheckOut = x.CheckOutTime,
                //        Day = x.Date,
                //        EmployeeName = x.Employee.Name,
                //    })
                //    .ToListAsync()
                var records = await (from x in context.Attendances
                                     where (!x.IsDeleted)
                                     orderby (x.Date)
                                     select x).ToListAsync();

                //var result = records.Select(x => new AttendDto
                //{
                //    Id = x.Id,
                //    EmployeeId = x.EmployeeId,
                //    CheckIn = x.CheckIn,
                //    CheckOut = x.CheckOut,
                //    Day = x.Day,
                //    EmployeeName = x.EmployeeName,
                //    Duration = x.CheckOut.HasValue
                //        ? x.CheckOut.Value - x.CheckIn.Value
                //        : (TimeSpan?)(DateTime.Now - x.CheckIn.Value)
                //                })
                //                    .ToList();
                var result = await (from e in context.Employees
                                    join a in context.Attendances
                                        .Where(x => x.Date == DateTime.Now && !x.IsDeleted)
                                        on e.Id equals a.EmployeeId into attendanceGroup
                                    from att in attendanceGroup.DefaultIfEmpty() 
                                    select new
                                    {
                                        EmployeeId = e.Id,
                                        EmployeeName = e.Name,
                                        CheckInTime = att.CheckInTime,
                                        CheckOutTime = att.CheckOutTime,
                                        Status = att != null 
                                    })
                        .ToListAsync();


                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("api/Attendance/GetByUserId/{id}")]
        [Authorize]
        public async Task <IHttpActionResult> GetByUserId(int id)
        {
            //await context.Attendances
            //    .Where(x => x.EmployeeId == id && !x.IsDeleted)
            //    .Select(x => new AttendDto
            //    {
            //        Id = x.Id,
            //        EmployeeId = x.EmployeeId,
            //        CheckIn = x.CheckInTime,
            //        CheckOut = x.CheckOutTime,
            //        Day = x.Date,
            //        EmployeeName = x.Employee.Name,

            //    })
            //    .OrderByDescending(a => a.Day)
            //    .ToListAsync()


            var result = await context.Attendances
           .Where(x => x.EmployeeId == id && !x.IsDeleted)
           .Select(x => new AttendDto
           {
               Id = x.Id,
               EmployeeId = x.EmployeeId,
               EmployeeName = x.Employee.Name,
               CheckIn = x.CheckInTime,
               CheckOut = x.CheckOutTime,
               Day = x.Date,
               Status = true,
           })
           .OrderByDescending(a => a.Day)
           .ToListAsync();

            var finalResult = result.Select(x => new AttendDto
            {
                Id = x.Id,
                EmployeeId = x.EmployeeId,
                EmployeeName = x.EmployeeName,
                CheckIn = x.CheckIn,
                CheckOut = x.CheckOut,
                Day = x.Day, // Nullable
                Status = x.Status,
                Duration = (x.CheckIn != null)
                    ? (x.CheckOut != null
                        ? x.CheckOut.Value - x.CheckIn.Value
                        : DateTime.Now - x.CheckIn.Value)
                    : (TimeSpan?)null
            }).ToList();

            return Ok(finalResult);
        }
        [HttpGet]
        [Route("api/Attendance/GetAttendanceForThisDay")]
        [Authorize(Roles = "Admin")]
        public async Task<IHttpActionResult> GetAttendanceForThisDay()
        {
            var today = DateTime.Now.Date;

            var result = await (from e in context.Employees where(!e.IsDeleted)
                                join a in context.Attendances
                                    .Where(x => DbFunctions.TruncateTime(x.Date) == today && !x.IsDeleted 
                                    )
                                    on e.Id equals a.EmployeeId  into attendanceGroup
                                from att in attendanceGroup.DefaultIfEmpty()
                                select new
                                {
                                    attId = att != null ? att.Id : (int?)null,
                                    EmployeeId = e.Id,
                                    EmployeeName = e.Name,
                                    CheckIn = att != null ? att.CheckInTime : (DateTime?)null,
                                    CheckOut = att != null ? att.CheckOutTime : (DateTime?)null,
                                    Day = att != null ? att.Date : (DateTime?)null,
                                    Status = att != null
                                })
                            .ToListAsync();

            var finalResult = result.Select(x => new AttendDto
            {
                Id = x.attId,
                EmployeeId = x.EmployeeId,
                EmployeeName = x.EmployeeName,
                CheckIn = x.CheckIn,
                CheckOut = x.CheckOut,
                Day = x.Day, // Nullable
                Status = x.Status,
                Duration = (x.CheckIn != null)
                    ? (x.CheckOut != null
                        ? x.CheckOut.Value - x.CheckIn.Value
                        : DateTime.Now - x.CheckIn.Value)
                    : (TimeSpan?)null
            }).ToList();

            return Ok(finalResult);
        }
        [HttpGet]
        [Route("api/Attendance/GetAttendanceByDate")]
        [Authorize(Roles = "Admin")]
        public async Task<IHttpActionResult> GetAttendanceByDate(DateTime date)
        {
            var day = date.Day;
            var month = date.Month;
            var year = date.Year;
            //var attend = await context.Attendances
            //    .Where(x =>
            //        !x.IsDeleted &&
            //        x.Date.Day == date.Day &&
            //        x.Date.Month == date.Month &&
            //        x.Date.Year == date.Year
            //    )
            //    .Select(x => new AttendDto
            //    {
            //        Id = x.Id,
            //        EmployeeId = x.EmployeeId,
            //        CheckIn = x.CheckInTime,
            //        CheckOut = x.CheckOutTime,
            //        EmployeeName = x.Employee.Name,
            //        Day = date
            //    })
            //    .ToListAsync();
            var result = await (from e in context.Employees
                                join a in context.Attendances
                                    .Where(x=>!x.IsDeleted &&
                                            x.Date.Day == date.Day &&
                                            x.Date.Month == date.Month &&
                                            x.Date.Year == date.Year)
                                    on e.Id equals a.EmployeeId into attendanceGroup
                                from att in attendanceGroup.DefaultIfEmpty()
                                select new
                                {
                                    attId = att != null ? att.Id : (int?)null,
                                    EmployeeId = e.Id,
                                    EmployeeName = e.Name,
                                    CheckIn = att != null ? att.CheckInTime : (DateTime?)null,
                                    CheckOut = att != null ? att.CheckOutTime : (DateTime?)null,
                                    Day = att != null ? att.Date : (DateTime?)null,
                                    Status = att != null
                                })
                            .ToListAsync();

            var finalResult = result.Select(x => new AttendDto
            {
                Id = x.attId,
                EmployeeId = x.EmployeeId,
                EmployeeName = x.EmployeeName,
                CheckIn = x.CheckIn,
                CheckOut = x.CheckOut,
                Day = x.Day, // Nullable
                Status = x.Status,
                Duration = (x.CheckIn != null)
                    ? (x.CheckOut != null
                        ? x.CheckOut.Value - x.CheckIn.Value
                        : DateTime.Now - x.CheckIn.Value)
                    : (TimeSpan?)null
            }).ToList();


            if (finalResult.Any()) return Ok(finalResult);
            return Ok(new List<AttendDto>());
        }
        [HttpGet]
        [Route("api/Attendance/GetAttendanceByMonthAndUserId/{id}")]
        [Authorize]
        public async Task<IHttpActionResult> GetAttendanceByMonthAndUserId(int id,  int? month=null,  int? year=null)
        {
            var today = DateTime.Today;
           if(month==null) month = today.Month;
           if(year==null) year = today.Year;
           var emp = await context.Employees
                .Where(x=> x.Id == id && !x.IsDeleted).FirstOrDefaultAsync();
            if (IsAdmin() | IsManager())
            {
                var daysInMonth = Enumerable.Range(1, DateTime.DaysInMonth(year.Value, month.Value))
                                            .Select(d => new DateTime(year.Value, month.Value, d))
                                            .ToList();

                var attendances = await context.Attendances
                    .Where(a => a.EmployeeId == id && !a.IsDeleted &&
                                a.Date.Year == year && a.Date.Month == month)
                    .ToListAsync();

                var result = daysInMonth.Select(day =>
                {
                    var att = attendances.FirstOrDefault(a => a.Date.Date == day.Date);
                    return new AttendDto
                    {
                        Id = att?.Id,
                        EmployeeId = id,
                        EmployeeName = att?.Employee.Name ?? context.Employees
                                            .Where(e => e.Id == id).Select(e => e.Name).FirstOrDefault(),
                        Day = day,
                        CheckIn = att?.CheckInTime,
                        CheckOut = att?.CheckOutTime,
                        Status = att != null, // True إذا فيه حضور
                        
                    };
                }).OrderByDescending(x => x.Day)
                  .ToList();

                return Ok(result);
            }
            else if (GetCurrentUserId() == id)
           {

                var daysInMonth = Enumerable.Range(1, DateTime.DaysInMonth(year.Value, month.Value))
                                            .Select(d => new DateTime(year.Value, month.Value, d))
                                            .ToList();

                var attendances = await context.Attendances
                    .Where(a => a.EmployeeId == id && !a.IsDeleted &&
                                a.Date.Year == year && a.Date.Month == month)
                    .ToListAsync();

                var result = daysInMonth.Select(day =>
                {
                    var att = attendances.FirstOrDefault(a => a.Date.Date == day.Date);
                    return new AttendDto
                    {
                        Id = att?.Id,
                        EmployeeId = id,
                        EmployeeName = att?.Employee.Name ?? context.Employees
                                            .Where(e => e.Id == id).Select(e => e.Name).FirstOrDefault(),
                        Day = day,
                        CheckIn = att?.CheckInTime,
                        CheckOut = att?.CheckOutTime,   
                        Status = att != null, // True إذا فيه حضور
                        Duration = (att != null && att.CheckInTime.HasValue)
                            ? (att.CheckOutTime.HasValue ? att.CheckOutTime.Value - att.CheckInTime.Value
                                                         : DateTime.Now - att.CheckInTime.Value)
                            : (TimeSpan?)null
                    };
                }).OrderByDescending(x => x.Day)
                  .ToList();

                return Ok(result);
            }
            else
            {
                return Unauthorized();
            }
        }
        [HttpDelete]
        [Route("api/Attendance/Delete/{id}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            //context.Attendances.FirstOrDefaultAsync(x => x.Id == id);
            var record = await (from x in context.Attendances
                                where x.Id == id && !x.IsDeleted
                                select x).FirstOrDefaultAsync();
            if (record == null)
            { 
            
                return BadRequest("No record founded"); 
            }
            record.IsDeleted=true;
            await context.SaveChangesAsync();
            var resonse = new AttendDto
            {
                CheckIn = record.CheckInTime,
                CheckOut = record.CheckOutTime,
                Day = record.Date,
                EmployeeId = record.EmployeeId
            };
            return Ok(resonse);
        }
        [HttpPost]
       [Authorize(Roles="Admin,Manager")]

        [Route("api/Attendance/Add")]
        public async Task<IHttpActionResult> Add(AttendDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //context.Attendances
            //    .AnyAsync(x => x.EmployeeId == model.EmployeeId && x.Date == model.Day && !x.IsDeleted);
            var check = await (from x in context.Attendances
                               where (x.EmployeeId == model.EmployeeId && x.Date == model.Day && !x.IsDeleted)
                               select x).AnyAsync();
            var emp = await context.Employees
                .Where(x=> x.Id == model.EmployeeId).FirstOrDefaultAsync();
            if(model.Day < emp.StartDate || model.Day > emp.EndDate)
                return BadRequest($"Date must between {emp.StartDate.Date} and {emp.EndDate.Date}");
            if (check)
                return BadRequest("This employee already cheaked");
            var attendace = new Attendance
            {
                CheckInTime = model.CheckIn,
                CheckOutTime = model.CheckOut,
                CreatedAt = DateTime.Now,
                Date = model.Day ?? DateTime.Now,
                EmployeeId = model.EmployeeId,
                Status = true,
            };
            try
            {
                context.Attendances.Add(attendace);
                await context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("api/Attendance/GetAttendanceById/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IHttpActionResult> GetById(int id)
        {
            //await context.Attendances
            //    .Where(x => x.Id == id)
            //    .Select(x => new AttendDto
            //    {
            //        Id = x.Id,
            //        CheckIn = x.CheckInTime,
            //        CheckOut = x.CheckOutTime,
            //        Day = x.Date,
            //        EmployeeId = x.EmployeeId,
            //        EmployeeName = x.Employee.Name ?? "",

            //    })
            //    .FirstOrDefaultAsync();
            var record = await (from x in context.Attendances
                                where (x.Id == id && !x.IsDeleted)
                                select new AttendDto
                                {
                                    CheckIn = x.CheckInTime,
                                    CheckOut = x.CheckOutTime,
                                    Day = x.Date,
                                    EmployeeId = x.EmployeeId,
                                    EmployeeName = x.Employee.Name,
                                    Id = x.Id , 
                                    
                                }
                                
                                ).FirstOrDefaultAsync();
            if (record == null)
                return BadRequest("no record founded");
            return Ok(record);
        }
        [HttpPut]
        [Route("api/Attendance/Update/{id}")]
    [Authorize(Roles="Admin")]
        public async Task<IHttpActionResult> Update (int id , AttendDto model)
        {
            //await context.Attendances
            //    .FirstOrDefaultAsync(x => x.Id == id);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var attend = await (from x in context.Attendances
                                where (x.Id == id && !x.IsDeleted)
                                select x).FirstOrDefaultAsync();
            if (attend == null)
                return BadRequest("no record found");
            attend.CheckInTime = model.CheckIn;
            attend.CheckOutTime = model.CheckOut;
            attend.Date = model.Day ?? DateTime.Now;
            attend.Status = true;
            model.EmployeeId = model.EmployeeId;
            await context.SaveChangesAsync();
            var date = attend.Date;

            var response = new AttendDto
            {
                CheckIn = attend.CheckInTime,
                CheckOut = attend.CheckOutTime,
                Day = date,
                EmployeeId = attend.EmployeeId,
                EmployeeName = attend.Employee.Name ?? "",
                Id = attend.Id,
                
            };
            return Ok(response);

        }
        [HttpGet]
        [Route("api/Attendance/IsCheckOut")]
        [Authorize]
        public async Task<IHttpActionResult> IsCheckOut()
        {
            var userId=GetCurrentUserId();
            var today = DateTime.Now;

            bool result = await context.Attendances
          .AnyAsync(a => a.EmployeeId == userId && DbFunctions.TruncateTime(a.CheckInTime) == DbFunctions.TruncateTime(today) && !a.IsDeleted && DbFunctions.TruncateTime(a.CheckOutTime) == DbFunctions.TruncateTime(today));

            return Ok(result);
        }
        [HttpGet]
        [Route("api/Attendance/GetCountOfPresentDay/{id}")]
        [Authorize]
        public async Task<IHttpActionResult> GetCountOfPresentDay(int id)
        {
            var day = DateTime.Now.Date;
            var count = 0;
            count = await context.Attendances
                .CountAsync(x => !x.IsDeleted && x.EmployeeId == id && x.Date.Year==day.Year && x.Date.Month==day.Month)
                ;
        return Ok(count);
        }

    }

}

