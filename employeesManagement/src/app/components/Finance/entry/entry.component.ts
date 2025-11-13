import { Component, Inject,OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { Employee } from 'src/app/Models/employee';
import { Salary } from 'src/app/Models/salary';
import { EmployeeService } from 'src/app/Services/employee.service';
import { SalaryService } from 'src/app/Services/salary.service';
@Component({
  selector: 'app-entry',
  templateUrl: './entry.component.html',
  styleUrls: ['./entry.component.css'],
})
export class EntryComponent implements OnInit {
    date : Date = new Date

  salaryForm:FormGroup 
   empList:Employee[]=[]
  constructor(
    private dialogRef: MatDialogRef<EntryComponent>,
    @Inject(MAT_DIALOG_DATA) public data: Salary,
    private service: SalaryService,
    private empService:EmployeeService
  ) {
    this.salaryForm = new FormGroup
    ({
      Id:new FormControl('',[]),
      EmployeeId:new FormControl('',[Validators.required]),
      Day : new FormControl('',[Validators.required])
    });
  }
  ngOnInit(): void {
    this.empService.getAllEmployees()
    .subscribe((res)=> this.empList=res)
  }
close()
{
  this.dialogRef.close(false);
}
save()
{
  if(this.salaryForm.valid)
  {
    var salary = this.salaryForm.value;
    salary.Day = this.date
    salary.Month = this.date.getMonth
    salary.Year = this.date.getFullYear
    salary.Id = 0 
    this.service.addSalary(salary)
    .subscribe(
      {
        next:(res)=>
        {
          this.dialogRef.close(true)
        },
        error:(err)=>
        {
          console.log(err)
        }
      }
    )
    
  }
}
}
