import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Department } from 'src/app/Models/department';
import { Employee } from 'src/app/Models/employee';
import { role } from 'src/app/Models/Role';
import { AuthService } from 'src/app/Services/auth.service';
import { DepartmentService } from 'src/app/Services/department.service';
import { EmployeeService } from 'src/app/Services/employee.service';
import { RoleService } from 'src/app/Services/role.service';

@Component({
  selector: 'app-entry',
  templateUrl: './entry.component.html',
  styleUrls: ['./entry.component.css'],
})
export class EntryComponent implements OnInit {
    model: Employee = {
    Id: 0,
    Name: '',
    Email: '',
    Phone: "",
    DeptId: 0,
    DeptName: '',
    Password: '',
    EndDate:"",
    StartDate: "",
    IsDeleted:false,
    RoleId:0,
    RoleName:"",
    Salary:0
  };
  isSalaryEnabled = false; 
    deptList: Department[] = [];
    isLogin: boolean = false;
    roleList:role[]=[];
    roleName:string=""
    empList:Employee[]=[]
  constructor(
    private dialogRef: MatDialogRef<EntryComponent>,
    @Inject(MAT_DIALOG_DATA) public data: Employee,
    private db: EmployeeService,
    private dept: DepartmentService,
    private auth: AuthService,
    private role: RoleService
  ) {}


  save() {
    if (this.data.Id != 0) {
      this.data.StartDate =new Date(this.data.StartDate);
      this.data.EndDate = new Date(this.data.EndDate)
      this.db
        .editEmp(this.data.Id, this.data)
        .subscribe((res) => this.dialogRef.close(true));
      return;
    }
    this.db.addNewEmp(this.data).subscribe({
      next: (res) => {
        this.dialogRef.close(true);
      },
    });
  }
  close() {
    this.dialogRef.close(false);
  }
    ngOnInit() {
    this.getALLDepts();
    this.auth.isLogedIn$.subscribe((res) => (this.isLogin = res));
      this.getAllRole()
      console.log(this.roleList)
      if(this.isLogin)
      {
   this.auth.userInfo$
      .subscribe({
        next:(res)=>
        {
          this.roleName=res.role
          console.log(this.roleName)
        }
      })
      
      }
      if(this.data.Id!=0)
        this.data.StartDate =this.toLocalDateString(new Date(this.data.StartDate))
              this.data.EndDate =this.toLocalDateString(new Date(this.data.EndDate))

    
  }
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
  resetModel() {
    this.model = {
      Id: 0,
      Name: '',
      Email: '',
      Phone: "",
      DeptId: 0,
      DeptName: '',
      Password: '',
      EndDate: "",
      StartDate: "",
      IsDeleted: false,
      RoleId : 3,
      RoleName:"",
      Salary:0
    };
  }
  loadEmployees() {
    this.db.getAllEmployees().subscribe((res) => {
      this.empList = res;
   
    });
  }
  getALLDepts() {
    this.dept.getAll().subscribe({
      next: (res) => {
        this.deptList = res;
      },
    });
  }
   getAllRole()
  {
  this.role.getAll()
    .subscribe(
      {next:(res)=>
      {
        this.roleList=res
      },
    error:(err)=>
      {
        console.log(err);
      }}
      
    )
  }
}
