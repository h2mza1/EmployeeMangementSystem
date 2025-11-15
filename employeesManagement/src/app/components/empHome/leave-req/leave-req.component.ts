import { Component, Inject, OnInit, Optional } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA  } from '@angular/material/dialog';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { Employee } from 'src/app/Models/employee';
import { Leave } from 'src/app/Models/leave';
import { EmployeeService } from 'src/app/Services/employee.service';
import { LeaveService } from 'src/app/Services/leave.service';
import { error, log } from 'console';
import { Vacation } from 'src/app/Models/vacation';


export enum RequestType {
  AnnualLeave = 1,
  SickLeave = 2,
  Sick = 3,
  Annual=4
}
@Component({
  selector: 'app-leave-req',
  templateUrl: './leave-req.component.html',
  styleUrls: ['./leave-req.component.css'],
})
export class LeaveReqComponent implements OnInit {
  reqForm: FormGroup;
  type:RequestType=RequestType.Annual
    RequestType = RequestType;
    date:Date=new Date
    vacationList:Vacation[]=[]
  constructor(
    private leaveService: LeaveService,
    
    @Optional() private dialogRef?: MatDialogRef<LeaveReqComponent>,
    @Inject(MAT_DIALOG_DATA) public data?: {id:number , name:string}
  ) {
    this.reqForm = new FormGroup({
      Id: new FormControl(0),
      EmployeeName: new FormControl(data?.name, [Validators.required],),
      EmployeeId: new FormControl(data?.id, [Validators.required]),
      VacationId: new FormControl(0, [Validators.required]), 
      StartDate: new FormControl(new Date(), []),
      EndDate: new FormControl(""),
      FromTime: new FormControl(""),
      ToTime: new FormControl(""),
      Reason: new FormControl('', [Validators.required]),
    });
  }
onTypeChange(event: any) {
  this.type = +event.target.value; // + لتحويل string إلى number
  if (this.type === this.RequestType.Annual) {
    this.reqForm.get('FromTime')?.reset();
    this.reqForm.get('ToTime')?.reset();
  } else {
    this.reqForm.get('StartDate')?.reset();
    this.reqForm.get('EndDate')?.reset();
  }
}

  ngOnInit(): void 
  {
    this.leaveService.getVacations()
    .subscribe(
      {
        next:(res)=>
        {
          this.vacationList=res
        }
      }
    )
  }
  send() {
    if(this.reqForm.valid)  
    {
      const req = this.reqForm.value;
      
      this.leaveService.request(req)
      .subscribe(
        {
          next:(res)=>
          {
            console.log(res)
          },
          error:(err)=>
          {
            console.log(err)
          }
        }
      )
    }
  }
  close() {
    this.dialogRef?.close(false)
  }
}
