import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { DepartmentService } from 'src/app/Services/department.service';
import { Department } from 'src/app/Models/department';
import { EntryComponent } from '../entry/entry.component';
import { DeleteComponent } from "../delete/delete.component";

@Component({
  selector: 'app-index',
  templateUrl: './index.component.html',
  styleUrls: ['./index.component.css']
})
export class IndexComponent implements OnInit {

  departments: Department[] = [];
    delete:boolean=false

  constructor(
    private service: DepartmentService,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.loadDepartments();
  }

  loadDepartments() {
    this.service.getAll().subscribe(data => {
      this.departments = data;
    });
  }

  openDialog(dep?: Department) {
    const dialogRef = this.dialog.open(EntryComponent, {
      width: '400px',
      data: dep ? { ...dep } : { Id: 0, Name: ''} 
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadDepartments();
      }
    });
  }
openDeleteDialog(id:number)
{
  const ref = this.dialog.open(DeleteComponent,
    {
      width:'400px',
      data:id,
    }
  )
  ref.afterClosed().subscribe(result => {
      if (result) {
        this.loadDepartments();
      }
    });
  
}
  deleteDepartment(id: number) {
    if (confirm('Are you sure you want to delete this department?')) {
      this.service.deleteDept(id).subscribe(() => {
        this.loadDepartments();
      });
    }
  }
}
