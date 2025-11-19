import { Component, OnInit, Inject, ViewChild } from '@angular/core';
import { FormControl, FormGroup, NgForm, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Attendance } from 'src/app/Models/attendance';
import { Employee } from 'src/app/Models/employee';
import { AttendService } from 'src/app/Services/attend.service';
import { EmployeeService } from 'src/app/Services/employee.service';

@Component({
  selector: 'app-entry',
  templateUrl: './entry.component.html',
  styleUrls: ['./entry.component.css'],
})
export class EntryComponent implements OnInit {
  attendForm: FormGroup;
  empList: Employee[] = [];
 list: Attendance = {
    Id: 0,
    CheckIn: new Date(),
    CheckOut: new Date(),
    Day: new Date(),
    Duration: '',
    EmployeeName: '',
    EmployeeId: 0,
    Status:false
  };
  @ViewChild('addForm') addForm!: NgForm;
  constructor(
    private dialogRef: MatDialogRef<EntryComponent>,
    private employeeService: EmployeeService,
    @Inject(MAT_DIALOG_DATA) public data: { id: number},
    private service: AttendService
  ) {
    this.attendForm = new FormGroup({
      Id: new FormControl(0, [Validators.required]),
      EmployeeId: new FormControl(0, [Validators.required]),
      CheckIn: new FormControl('', [Validators.required]),
      CheckOut: new FormControl(''),
      Day: new FormControl(new Date(), [Validators.required]),
    });
  }
ngOnInit(): void {
  this.loadEmployee();

  if (this.data.id && this.data.id !== 0) {
    this.service.getById(this.data.id).subscribe({
      next: (res) => {
        this.list = res;

        this.attendForm.patchValue({
          Id: res.Id,
          EmployeeId: res.EmployeeId,
          CheckIn: this.formatTime(res.CheckIn),
          CheckOut: this.formatTime(res.CheckOut),
          Day: this.toLocalDateString(res.Day), 
        });
      },
      error: (err) => console.error(err)
    });
  }
}


formatTime(dateTime: string | Date): string {
  if (!dateTime) return '';
  const d = new Date(dateTime);
 return d.toTimeString().substring(0, 5);}

formatDate(dateTime: string | Date): string {
  if (!dateTime) return '';
  const d = new Date(dateTime);
  return d.toISOString().substring(0, 10); // yyyy-MM-dd
}
private toLocalDateString(dateString: Date): string {
  const date = new Date(dateString);
  const tzOffset = date.getTimezoneOffset() * 60000; // فرق التوقيت بالـ ms
  const localISOTime = new Date(date.getTime() - tzOffset).toISOString();
  return localISOTime.slice(0, 10); // yyyy-MM-dd فقط
}

  loadEmployee() {
    this.employeeService.getAllEmployees().subscribe({
      next: (res) => {
        this.empList = res;
      },
      error: (err) => console.error(err)
    });
  }

  save() {
    if (this.attendForm.valid) {
      const attend: Attendance = this.attendForm.value;

      if (this.data.id === 0) {
        attend.Day = new Date(attend.Day)
        this.service.add(attend).subscribe({
          next: (res) => {
            this.dialogRef.close(true);
          },
          error: (err) => alert(err)
        });
      } else {
        this.service.update(this.data.id, attend).subscribe({
          next: (res) => {
            this.dialogRef.close(true);
          },
          error: (err) => alert(err)
        });
      }
    }
  }

  close() {
    this.dialogRef.close(false);
  }
}
