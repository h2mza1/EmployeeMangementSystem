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
    [Authorize]
    [RoutePrefix("api/EmpSal")]
    public class SalaryController : SharedController
    {
        private readonly EmpDbContext _context;

        public SalaryController()
        {
            _context = new EmpDbContext();
        }

        [HttpPost]
        [Route("AddSalary")]
        public async Task<IHttpActionResult> AddSalary(SalaryDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var employeeExists = await _context.Employees
                .AnyAsync(x => x.Id == model.EmployeeId && !x.IsDeleted);

            if (!employeeExists)
                return BadRequest($"Employee with id {model.EmployeeId} not found.");

            var (baseSalary, deduction, netSalary, presentHours, absentHours) =
                await CalculateSalaryAsync(model.EmployeeId, model.Month, model.Year);
            var hasPaidLeave =absentHours > 0 && deduction == 0;

            if (presentHours == 0 && !hasPaidLeave)
                return BadRequest("Employee has no attendance or paid leave records for this month.");
            if (presentHours == 0)
                return BadRequest("Employee has no attendance records for this month.");

            var existingSalary = await _context.Salaries
                .FirstOrDefaultAsync(s => s.EmployeeId == model.EmployeeId && s.Month == model.Month && s.Year == model.Year);

            if (existingSalary != null)
            {
                existingSalary.BaseSalary = baseSalary;
                existingSalary.Deduction = deduction;
                existingSalary.NetSalary = netSalary;
                existingSalary.IsDeleted = false;
                _context.Entry(existingSalary).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                var employeeName = await _context.Employees
                    .Where(x => x.Id == model.EmployeeId)
                    .Select(x => x.Name)
                    .FirstOrDefaultAsync();

                var updatedResponse = new SalaryDto
                {
                    EmployeeId = existingSalary.EmployeeId,
                    EmployeeName = employeeName,
                    Year = existingSalary.Year,
                    Month = existingSalary.Month,
                    BaseSalary = existingSalary.BaseSalary,
                    Deduction = existingSalary.Deduction,
                    NetSalary = existingSalary.NetSalary
                };

                return Ok(new { message = "Salary updated successfully.", updatedResponse });
            }

            var salary = new Salary
            {
                EmployeeId = model.EmployeeId,
                Year = model.Year,
                Month = model.Month,
                BaseSalary = baseSalary,
                Deduction = deduction,
                NetSalary = netSalary
            };

            _context.Salaries.Add(salary);
            await _context.SaveChangesAsync();

            var empName = await _context.Employees
                .Where(x => x.Id == model.EmployeeId)
                .Select(x => x.Name)
                .FirstOrDefaultAsync();

            var response = new SalaryDto
            {
                EmployeeId = salary.EmployeeId,
                EmployeeName = empName,
                Year = salary.Year,
                Month = salary.Month,
                BaseSalary = baseSalary,
                Deduction = deduction,
                NetSalary = netSalary
            };

            return Ok( response );
        }

        [HttpPost]
        [Route("AddAllSalaries")]
        public async Task<IHttpActionResult> AddAllSalaries([FromBody]DateTime? date)
        {
            if (!IsAdmin() && !IsManager())
                return Unauthorized();

            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;
            if (date != null)
            {
                year = date.Value.Year;
                month = date.Value.Month;
            }

            var employees = await _context.Employees
                .Where(x => !x.IsDeleted && x.BaseSalary > 0)
                .ToListAsync();

            var addedOrUpdatedList = new List<SalaryDto>();

            foreach (var emp in employees)
            {
                var (baseSalary, deduction, netSalary, presentHours, absentHours) =
                    await CalculateSalaryAsync(emp.Id, month, year);
                var hasPaidLeave =absentHours > 0 && deduction == 0;

                if (presentHours == 0 && !hasPaidLeave) continue;

                var existingSalary = await _context.Salaries
                    .FirstOrDefaultAsync(s => s.EmployeeId == emp.Id && s.Month == month && s.Year == year);

                if (existingSalary != null)
                {
                    existingSalary.BaseSalary = baseSalary;
                    existingSalary.Deduction = deduction;
                    existingSalary.NetSalary = netSalary;
                    existingSalary.IsDeleted = false;

                    _context.Entry(existingSalary).State = EntityState.Modified;
                }
                else
                {
                    var newSalary = new Salary
                    {
                        EmployeeId = emp.Id,
                        Year = year,
                        Month = month,
                        BaseSalary = baseSalary,
                        Deduction = deduction,
                        NetSalary = netSalary
                    };
                    _context.Salaries.Add(newSalary);
                }

                addedOrUpdatedList.Add(new SalaryDto
                {
                    EmployeeId = emp.Id,
                    EmployeeName = emp.Name,
                    Year = year,
                    Month = month,
                    BaseSalary = baseSalary,
                    Deduction = deduction,
                    NetSalary = netSalary
                });
            }

            if (!addedOrUpdatedList.Any())
                return Ok("No new or updated salaries this month.");

            await _context.SaveChangesAsync();
            return Ok(addedOrUpdatedList);
        }

        [HttpGet]
        [Route("MySalary")]
        public async Task<IHttpActionResult> GetMySalary()
        {
            var userId = GetCurrentUserId();
            var empSalaries = await _context.Salaries
                .Where(x => x.EmployeeId == userId && !x.IsDeleted)
                .OrderByDescending(x => x.Year)
                .ThenByDescending(x => x.Month)
                .Select(x => new SalaryDto
                {
                    Id = x.Id,
                    EmployeeId = x.EmployeeId,
                    Month = x.Month,
                    Year = x.Year,
                    BaseSalary = x.BaseSalary,
                    Deduction = x.Deduction,
                    NetSalary = x.NetSalary
                })
                .ToListAsync();

            return Ok(empSalaries);
        }

        [HttpGet]
        [Route("All")]
        public async Task<IHttpActionResult> GetAll()
        {
            if (!IsAdmin() && !IsManager())
                return Unauthorized();

            var list = await _context.Salaries
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.Year)
                .ThenByDescending(x => x.Month)
                .Select(x => new SalaryDto
                {
                    Id = x.Id,
                    EmployeeId = x.EmployeeId,
                    Month = x.Month,
                    Year = x.Year,
                    BaseSalary = x.BaseSalary,
                    Deduction = x.Deduction,
                    NetSalary = x.NetSalary,
                    EmployeeName = x.Employee.Name ?? ""
                })
                .ToListAsync();

            return Ok(list);
        }

        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var record = await _context.Salaries.FirstOrDefaultAsync(x => x.Id == id);
            if (record == null)
                return BadRequest($"No record Found with Id : {id}");

            record.IsDeleted = true;
            await _context.SaveChangesAsync();
            return Ok();
        }


        //[HttpGet]
        //[Route("api/Salary/GetSalaryForThisMonth/{id}")]
        //public async Task<IHttpActionResult> GetSalaryForThisMonth(int id)
        //{
        //    var today = DateTime.Today;
        //    var result = await CalculateSalaryAsync(id, today.Month, today.Year);
        //    return Ok(result);
        //}


        [HttpGet]
        [Route("GetSalaryForThisMonth")]
        public async Task<IHttpActionResult> GetSalaryForThisMonth()
        {
            var today = DateTime.Today;
            int month = today.Month;
            int year = today.Year;

            // جلب الرواتب من جدول Salaries للشهر الحالي
            var salaries = await _context.Salaries
                .Include(s => s.Employee)
                .Where(s => s.Month == month && s.Year == year && !s.IsDeleted)
                .ToListAsync();

            if (!salaries.Any())
            {
                return Ok("No salary records found for this month.");
            }

            // مجموع الرواتب الصافية
            var totalNetSalary = salaries.Sum(s => s.NetSalary);

            var result = salaries.Select(s => new SalaryDto
            {
               EmployeeId= s.Employee.Id,
                EmployeeName= s.Employee.Name,
                Month= s.Month,
                Year= s.Year,
                 BaseSalary=s.BaseSalary,
                //s.TotalWorkDays,
                //s.PresentDays,
                //s.PaidLeaveDays,
                //s.UnpaidLeaveDays,
                //s.AbsentDays,
                //s.Deduction,
                NetSalary = s.NetSalary,
                Deduction= s.Deduction,
                Id = s.Id,
            });

            return Ok(new
            {
                Month = $"{month}/{year}",
                TotalEmployees = salaries.Count,
                TotalNetSalary = totalNetSalary,
                Salaries = result
            });
        }


        [HttpGet]
        [Route("GetSalaryByUserAndDate")]
        public async Task<IHttpActionResult> GetSalaryByUserAndDate(int? id, DateTime? date)
        {
            var salaries = new List<SalaryDto>();
            if (!date.HasValue && id != 0)
            {
                salaries = await _context.Salaries
                   .Where(x => x.EmployeeId == id && !x.IsDeleted)
                   .Select(x => new SalaryDto
                   {
                       Id = x.Id,
                       EmployeeId = x.EmployeeId,
                       Month = x.Month,
                       Year = x.Year,
                       BaseSalary = x.BaseSalary,
                       Deduction = x.Deduction,
                       NetSalary = x.NetSalary,
                       EmployeeName = x.Employee.Name ?? ""
                   }).ToListAsync();

                return Ok(salaries);
            }
            else if (date.HasValue && id != 0)

            {
                var month = date.Value.Month;
                var year = date.Value.Year;

                var result = await _context.Salaries
                    .Where(x => x.EmployeeId == id && x.Month == month && x.Year == year && !x.IsDeleted)
                    .Select(x => new SalaryDto
                    {
                        Id = x.Id,
                        EmployeeId = x.EmployeeId,
                        Month = x.Month,
                        Year = x.Year,
                        BaseSalary = x.BaseSalary,
                        Deduction = x.Deduction,
                        NetSalary = x.NetSalary,
                        EmployeeName = x.Employee.Name ?? ""
                    }).ToListAsync();

                return Ok(result);
            }
            else
            {
                var month = date.Value.Month;
                var year = date.Value.Year;

                var result = await _context.Salaries
                    .Where(x =>  x.Month == month && x.Year == year && !x.IsDeleted)
                    .Select(x => new SalaryDto
                    {
                        Id = x.Id,
                        EmployeeId = x.EmployeeId,
                        Month = x.Month,
                        Year = x.Year,
                        BaseSalary = x.BaseSalary,
                        Deduction = x.Deduction,
                        NetSalary = x.NetSalary,
                        EmployeeName = x.Employee.Name ?? ""
                    }).ToListAsync();

                return Ok(result);
            }
        }

        private int CalculateLeaveHours(LeaveRequest l)
        {
            // لو الإجازة بالساعات
            if (l.FromTime.HasValue && l.ToTime.HasValue)
            {
                return (int)(l.ToTime.Value - l.FromTime.Value).TotalHours;
            }

            // لو بالأيام
            DateTime end = l.EndDate ?? l.StartDate;
            int days = (int)(end - l.StartDate).TotalDays + 1;

            return days * 8; // يوم = 8 ساعات
        }

        private async Task<(decimal baseSalary, decimal deduction, decimal netSalary, int totalWorkHours, int totalAbsentHours)>
       CalculateSalaryAsync(int employeeId, int month, int year)
        {
            var monthStart = new DateTime(year, month, 1);
            var nextMonth = monthStart.AddMonths(1);

            // جميع أيام الشهر
            var allDays = Enumerable.Range(0, (nextMonth - monthStart).Days)
                                    .Select(offset => monthStart.AddDays(offset));

            // أيام العمل (الأحد - الخميس)
            var workingDays = allDays.Where(d => d.DayOfWeek != DayOfWeek.Friday &&
                                                 d.DayOfWeek != DayOfWeek.Saturday)
                                     .ToList();

            int totalWorkingDays = workingDays.Count;
            const int hoursPerDay = 8;
            int expectedWorkHours = totalWorkingDays * hoursPerDay;

            // الحضور
            var attendances = await _context.Attendances
                .Where(a => a.EmployeeId == employeeId &&
                            a.Status == true &&
                            a.CheckInTime.HasValue &&
                            a.CheckOutTime.HasValue &&
                            !a.IsDeleted &&
                            a.CheckInTime.Value >= monthStart &&
                            a.CheckInTime.Value < nextMonth)
                .ToListAsync();

            // مجموع ساعات الدوام
            int totalWorkHours = 0;
            foreach (var att in attendances)
            {
                var diff = att.CheckOutTime.Value - att.CheckInTime.Value;
                totalWorkHours += (int)diff.TotalHours;
            }

            int totalAbsentHours = expectedWorkHours - totalWorkHours;
            if (totalAbsentHours < 0) totalAbsentHours = 0;

            // ------------------------------------------------------------------
            //       حساب الإجازات المدفوعة (legal + sick) بالساعات
            // ------------------------------------------------------------------

            // 14 يوم = 112 ساعة
            int legalBalance = 14 * 8;
            int sickBalance = 14 * 8;

            // تعريف الإجازات
            var legalVac = await _context.Vacations.FirstOrDefaultAsync(x => x.Name.ToLower() == "legal leave");
            var sickVac = await _context.Vacations.FirstOrDefaultAsync(x => x.Name.ToLower() == "sick leave");

            // الإجازات الموافق عليها خلال نفس السنة
            var approvedLeaves = await _context.LeaveRequests
                .Where(l => l.EmployeeId == employeeId &&
                            l.Status == RequestStatus.Approved &&
                            !l.IsDeleted &&
                            l.StartDate.Year == year)
                .ToListAsync();

            int usedLegalHours = 0;
            int usedSickHours = 0;

            foreach (var l in approvedLeaves)
            {
                int hours = CalculateLeaveHours(l);

                if (legalVac != null && l.VacationId == legalVac.Id)
                    usedLegalHours += hours;

                if (sickVac != null && l.VacationId == sickVac.Id)
                    usedSickHours += hours;
            }

            // الحد الأعلى لكل نوع (112 ساعة)
            usedLegalHours = Math.Min(usedLegalHours, legalBalance);
            usedSickHours = Math.Min(usedSickHours, sickBalance);

            int paidLeaveHours = usedLegalHours + usedSickHours;

            // طرح الإجازات المدفوعة من الغياب
            int totalAbsentHoursAdjusted = totalAbsentHours - paidLeaveHours;
            if (totalAbsentHoursAdjusted < 0) totalAbsentHoursAdjusted = 0;

            // ------------------------------------------------------------------
            //                    حساب الراتب والخصومات
            // ------------------------------------------------------------------

            var baseSalary = await _context.Employees
                .Where(e => e.Id == employeeId && !e.IsDeleted)
                .Select(e => e.BaseSalary)
                .FirstOrDefaultAsync();

            decimal hourlyRate = baseSalary / (30 * hoursPerDay);
            decimal deduction = totalAbsentHoursAdjusted * hourlyRate;
            decimal netSalary = baseSalary - deduction;

            return (baseSalary, deduction, netSalary, totalWorkHours, totalAbsentHours);
        }


    }
}
