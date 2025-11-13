import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';

@Component({
  selector: 'app-delete',
  templateUrl: './delete.component.html',
  styleUrls: ['./delete.component.css'],
})
export class DeleteComponent implements OnInit {
     result : boolean = false 
  constructor(private dialogRef: MatDialogRef<DeleteComponent>) {}
  ngOnInit(): void {}

  delete()
  {
    this.dialogRef.close(true);

  }
  close() {
     this.dialogRef.close(false);
 
  }
}
