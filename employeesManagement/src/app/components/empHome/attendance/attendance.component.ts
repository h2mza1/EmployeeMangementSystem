import { Component, Inject, inject, OnInit, Optional } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import { Attendance } from 'src/app/Models/attendance';
import { AttendService } from 'src/app/Services/attend.service';
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { AuthService } from 'src/app/Services/auth.service';

@Component({
  selector: 'app-emp-attendance',
  templateUrl: './attendance.component.html',
  styleUrls: ['./attendance.component.css'],

})
export class AttendanceComponent implements OnInit {
attendList:Attendance[]=[]
months = [
  { value: 1, name: "January" },
  { value: 2, name: "February" },
  { value: 3, name: "March" },
  { value: 4, name: "April" },
  { value: 5, name: "May" },
  { value: 6, name: "June" },
  { value: 7, name: "July" },
  { value: 8, name: "August" },
  { value: 9, name: "September" },
  { value: 10, name: "October" },
  { value: 11, name: "November" },
  { value: 12, name: "December" }
];
selectedMonth: number | null = null;
selectedYear: number | null = null;
years: number[] = [];
date:Date = new Date
  constructor(private attendService:AttendService,
    private authService:AuthService,
    @Inject(MAT_DIALOG_DATA) public data: number   ,
  @Optional() private dialogRef?: MatDialogRef<AttendanceComponent>,)
    {
      
     }
generateYears() {
  const currentYear = new Date().getFullYear();
  for (let y = currentYear; y >= currentYear - 10; y--) {
    this.years.push(y);
  }
}
  ngOnInit(): void {
      this.generateYears();

    this.attendService.getByUserId(this.data)
    .subscribe(
      {
        next:(res)=>
        {
          this.attendList=res
 this.attendList.forEach(element => {
        const checkInDate = new Date(element.CheckIn);  
        const checkOutDate = new Date(element.CheckOut);

        element.Duration = (checkOutDate.getHours() - checkInDate.getHours()).toString();
        if(Number(element.Duration)<0)
          element.Duration="0";
      });    
    
      }
    }
    )
    
  }
applyFilter() {

  if (this.selectedMonth != null && this.selectedYear != null) {
    return this.filter(this.selectedMonth, this.selectedYear);
  }

  if (this.selectedMonth != null) {
    return this.filter(this.selectedMonth, undefined);
  }

  if (this.selectedYear != null) {
    return this.filter(undefined, this.selectedYear);
  }

  this.filter();
}


filter(month?: number, year?: number) {
  this.attendService.getByUserId(this.data, month, year)
    .subscribe(res => this.attendList = res);
}

}
