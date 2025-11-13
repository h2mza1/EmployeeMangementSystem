import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { IndexComponent } from './components/department/index/index.component';
import { MainComponent } from './components/employee/main/main.component';
import { LoginComponent } from './components/login/login.component';
import { HomeComponent } from './components/empHome/home/home.component';
import { AdminGuardService } from './guards/admin-guard.service';
import { ManagerGuardService } from './guards/manager-guard.service';
import { AttendanceComponent } from './components/attend/attendance/attendance.component';
import { IndexComponent as profile } from './components/Shared/profile/index/index.component';
import { SalaryComponent } from './components/Finance/salary/salary.component';
import { DateSelectComponent } from './components/attend/date-select/date-select.component';
import { IndexComponent as HistoryIndex } from './components/history/index/index.component';
import { LeaveComponent } from './leave/leave.component';
import { EmployeeGuard } from './guards/employee.guard';

const routes: Routes = [
  {path:"Department",component:IndexComponent ,canActivate:[AdminGuardService] },
  {path:"Employee",component:MainComponent ,canActivate:[ManagerGuardService]},
  {path:"Login",component:LoginComponent},
  {path:"Home",component:HomeComponent , canActivate:[EmployeeGuard]},
  {path:"Attendance",component:DateSelectComponent  ,canActivate:[AdminGuardService]},
  {path:"Profile",component:profile},
  {path:"Finance",component:SalaryComponent , canActivate:[ManagerGuardService]},
  {path:"History",component:HistoryIndex,canActivate:[AdminGuardService]},
  {path:"Leave",component:LeaveComponent,canActivate:[ManagerGuardService]},
  {path:"",component: LoginComponent} 
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
