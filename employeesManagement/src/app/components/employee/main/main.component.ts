import { Component, OnInit } from '@angular/core';
import { Employee } from 'src/app/Models/employee';
import { EmployeeService } from 'src/app/Services/employee.service';
import { MatDialog } from '@angular/material/dialog';
import { EntryComponent } from '../entry/entry.component';
import { DeleteComponent } from '../../Shared/delete/delete.component';
import { filter, switchMap, tap } from 'rxjs/operators';
import { Department } from 'src/app/Models/department';
import { role } from 'src/app/Models/Role';

@Component({
  selector: 'app-main',
  templateUrl: './main.component.html',
})
export class MainComponent implements OnInit {
  // 🔹 بيانات الموظفين
  empList: Employee[] = [];
  filteredList: Employee[] = [];
  originalList: Employee[] = [];
  status = false;

  // 🔹 الفلاتر
  searchName: string = '';
  selectedDept: string = '';
  selectedRole: string = '';
  startDate: string = '';

  // 🔹 الأقسام والأدوار للـ select
  departments: string[] = [];
  roles: string[] = [];

  constructor(private db: EmployeeService, private dialog: MatDialog) {}

  ngOnInit() {
    this.loadEmployees();
  }

  // 🔹 تحميل كل الموظفين
  loadEmployees() {
    this.db.getAllEmployees().subscribe((res) => {
      this.empList = res;
      this.originalList = [...res];
      this.filteredList = [...res];
      this.status = res.length > 0;

      // استخراج الأقسام والأدوار
      this.departments = [...new Set(res.map((e) => e.DeptName))];
      this.roles = [...new Set(res.map((e) => e.RoleName))];
    });
  }

  // 🔹 فتح نافذة إضافة أو تعديل
  openAddOrUpdate(emp?: Employee) {
    const dialogRef = this.dialog.open(EntryComponent, {
      width: 'auto',
      data: emp ? { ...emp } : { Id: 0 },
    });

    dialogRef.afterClosed().subscribe(
    {
      next:(res)=>
      {
        if(res==true)
          this.loadEmployees()
      }
    }
    )
  }

  // 🔹 حذف موظف
  delete(id: number) {
    this.dialog
      .open(DeleteComponent, { width: '400px' })
      .afterClosed()
      .pipe(
        filter((res) => !!res),
        switchMap(() => this.db.deleteEmp(id)),
        tap(() => this.loadEmployees())
      )
      .subscribe();
  }

  // 🔹 تطبيق الفلاتر
  applyFilters() {
    this.filteredList = this.originalList.filter((emp) => {
      const matchesName =
        !this.searchName ||
        emp.Name.toLowerCase().includes(this.searchName.toLowerCase());
      const matchesDept =
        !this.selectedDept || emp.DeptName === this.selectedDept;
      const matchesRole =
        !this.selectedRole || emp.RoleName === this.selectedRole;
      const matchesDate =
        !this.startDate || emp.StartDate >= this.startDate;

      return matchesName && matchesDept && matchesRole && matchesDate;
    });
  }

  // 🔹 إعادة ضبط الفلاتر
  resetFilters() {
    this.searchName = '';
    this.selectedDept = '';
    this.selectedRole = '';
    this.startDate = '';
    this.filteredList = [...this.originalList];
  }
}
