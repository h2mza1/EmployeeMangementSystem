import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { DepartmentService } from 'src/app/Services/department.service';

@Component({
  selector: 'app-delete',
  templateUrl: './delete.component.html',
  styleUrls: ['./delete.component.css'],
})
export class DeleteComponent implements OnInit {
  constructor(
    private dialogRef: MatDialogRef<DeleteComponent>,
    @Inject(MAT_DIALOG_DATA) public data: number,
    private service: DepartmentService
  ) {}

  ngOnInit(): void {}
  delete() {
    if (this.data != 0) {
      this.service.deleteDept(this.data).subscribe({
        next: (res) => {
          this.dialogRef.close(true);
        },
        error: (err) => {
          console.log('err');
        },
      });
    }
  }
  close() {
    this.dialogRef.close(true);
  }
}
