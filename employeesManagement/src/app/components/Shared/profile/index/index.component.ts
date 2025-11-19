import { Component, Inject, OnInit, Optional } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Employee } from 'src/app/Models/employee';
import { EmployeeService } from 'src/app/Services/employee.service';

@Component({
  selector: 'app-index',
  templateUrl: './index.component.html',
  styleUrls: ['./index.component.css']
})
export class IndexComponent implements OnInit {
  Emp!: Employee;

  constructor(
     private _context: EmployeeService,
    @Optional() private dialogRef?: MatDialogRef<IndexComponent>,  
    @Optional() @Inject(MAT_DIALOG_DATA) public data?: number,     
   
  ) {}

  ngOnInit(): void {
    if (this.data) {
      this._context.getById(this.data).subscribe({
        next: (res) => 
        {  this.Emp = res
          
        }

        ,
        error: (err) => {}
      });
    }
  }
  save()
  {
    if(this.data !=null)
    {  this._context.editEmp(this.data,this.Emp)
    .subscribe(
      {
        next:(res)=>
        {
          this.dialogRef?.close(true);
        }
      }
    )}
  }
  close() {
    this.dialogRef?.close(false);
  }
}
