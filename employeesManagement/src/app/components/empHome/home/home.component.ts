import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Employee } from 'src/app/Models/employee';
import { AuthService } from 'src/app/Services/auth.service';
import { EmployeeService } from 'src/app/Services/employee.service';
import { LeaveReqComponent } from '../leave-req/leave-req.component';
import { style } from '@angular/animations';
import { NgStyle } from '@angular/common';
import { AttendService } from 'src/app/Services/attend.service';
import { Leave } from 'src/app/Models/leave';
import { LeaveService } from 'src/app/Services/leave.service';
import { SalaryService } from 'src/app/Services/salary.service';
import { Salary } from 'src/app/Models/salary';
import { log } from 'console';
import { SalarydetailsComponent } from '../salarydetails/salarydetails.component';
import { AttendanceComponent } from '../attendance/attendance.component';
export enum RequestStatus {
  Pinding = 1,
  Reject = 2,
  Approved = 3,
}
@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
})
export class HomeComponent implements OnInit {
  user: Employee = {
    DeptId: 0,
    DeptName: '',
    Email: '',
    EndDate: new Date(),
    Id: 0,
    IsDeleted: false,
    Name: '',
    Password: '',
    Phone: '',
    RoleId: 0,
    RoleName: '',
    Salary: 0,
    StartDate: new Date(),
    expectedWorkHours : 0,
    workingHours : 0
  };
  id: number = 0;
  absentDayCount: number = 0;
  presentDayCount: number = 0;
  requests: Leave[] = [];
  latestSal!: Salary;

  request = { Type: 'Leave', StartDate: '', EndDate: '', Reason: '' };
  constructor(
    private empService: EmployeeService,
    private authService: AuthService,
    private attendService: AttendService,
    private leaveService: LeaveService,
    private salService: SalaryService,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.authService.userInfo$.subscribe({
      next: (res) => {
        this.id = res.nameid;
        console.log('id=' + this.id);
        if (this.id != 0) {
          this.attendService.getCountPresentDay(this.id).subscribe({
            next: (res) => {
              this.absentDayCount = this.calculateAbsentDays(res);
              this.presentDayCount = res;
            },
          });
          this.empService.getById(this.id).subscribe({
            next: (res) => {
              this.user = res;
            this.loadAllReq()

              this.salService.getSalaryByUserAndDate(this.user.Id,new Date())
              .subscribe(
                {
                  next:(res)=>
                  {
                    this.latestSal=res[0]
                    console.log(this.latestSal)
                  }
                }
              )
            },
          });
        }
      },
      error: (err) => {
        console.log(err);
      },
    });
  }
  calculateAbsentDays(presentDays: number): number {
    const today = new Date();
    const currentDayOfMonth = today.getDate(); // الأيام التي مرت حتى اليوم

    let workingDaysUntilToday = 0;

    for (let i = 1; i <= currentDayOfMonth; i++) {
      const current = new Date(today.getFullYear(), today.getMonth(), i);
      const weekday = current.getDay(); // 0=الأحد, 5=الجمعة, 6=السبت
      if (weekday !== 5 && weekday !== 6) {
        workingDaysUntilToday++;
      }
    }

    const absentDays = workingDaysUntilToday - presentDays;
    return absentDays >= 0 ? absentDays : 0; // نتأكد أن الناتج ليس سالبًا
  }
  submitRequest() {
    const dialogRef = this.dialog.open(LeaveReqComponent, {
      width: 'auto',
      data: { id: this.user.Id, name: this.user.Name },
    });
    dialogRef.afterClosed()
    .subscribe(
      {
        next:(res)=>
        {
          if(res==true)
          {
            this.loadAllReq()
          }
        }
      }
    )
  }
  salDetails()
  {
    const dialogRef = this.dialog.open(SalarydetailsComponent,
      {
        width:"auto",
        data:this.latestSal.EmployeeId
      }
    )
  }
openAttendacePage()
{
  const dialogRef = this.dialog.open(AttendanceComponent,
    {
      width:"auto",
      data:this.user.Id
    }
  )
}
loadAllReq()
{
    this.leaveService.getRequestsByUserId(this.user.Id).subscribe({
                next: (res) => {
                  this.requests = res;
                  console.log(res);
                },
                error: (err) => console.log(err),
              });
}
  // this.leaveService.addLeave(payload).subscribe({
  //   next: () => {
  //     Swal.fire('✅ Success', 'Your request has been submitted!', 'success');
  //     this.request = { Type: 'Leave', StartDate: '', EndDate: '', Reason: '' }; // reset
  //   },
  //   error: () => {
  //     Swal.fire('❌ Error', 'Failed to submit request.', 'error');
  //   },
  // });
}
