import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { DepartmentService } from 'src/app/Services/department.service';
import { Department } from 'src/app/Models/department';
import { log } from 'console';
import { empty } from 'rxjs';

@Component({
  selector: 'app-entry',
  templateUrl:'./entry.component.html',
  styleUrls: ['./entry.component.css']
})
export class EntryComponent {

  constructor(
    private dialogRef: MatDialogRef<EntryComponent>,
    @Inject(MAT_DIALOG_DATA) public data: Department,
    private service: DepartmentService
  ) {}

  save() {
    if (this.data.Id==0) {
      this.service.addDept(this.data.Name).subscribe(() => this.dialogRef.close(true));
           

    } else {
     
      this.service.updateDept(this.data.Id,this.data.Name).subscribe(() => this.dialogRef.close(true));
    }
  }

  close() {
    this.dialogRef.close(false);
  }
}
