import { Component, OnInit, Output } from '@angular/core';
import { Attendance } from 'src/app/Models/attendance';
import { AttendService } from 'src/app/Services/attend.service';

@Component({
  selector: 'app-date-select',
  templateUrl: './date-select.component.html',
  styleUrls: ['./date-select.component.css']
})
export class DateSelectComponent implements OnInit {
  @Output() date:string ="" 
  @Output() absent:boolean=false;
  @Output() present:boolean=false;
  @Output() searchItem:string="";
  AttenList: Attendance[] = [];
  constructor(private service:AttendService) {
   }

  ngOnInit(): void {
    this.loadEmployee()
  }

loadEmployee()
{
   this.service
      .getAttendanceForThisDay()
      .subscribe((res) => (this.AttenList = res))
}
  filter() {

  }
}
