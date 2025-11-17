import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NavbarComponent } from './components/navbar/navbar.component';
import { FooterComponent } from './components/footer/footer.component';
import { MainComponent } from './components/employee/main/main.component';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { IndexComponent } from './components/department/index/index.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { EntryComponent } from './components/department/entry/entry.component';
import { MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { DeleteComponent } from './components/department/delete/delete.component';
import { LoginComponent } from './components/login/login.component';
import { mainInterceptor } from './Interceptors/mainInterceptor';
import { HomeComponent } from './components/empHome/home/home.component';
import { AttendanceComponent } from './components/attend/attendance/attendance.component';
import { IndexComponent as ProfileComponent } from './components/Shared/profile/index/index.component';

import { CommonModule } from '@angular/common';
import { SalaryComponent } from './components/Finance/salary/salary.component';
import { EntryComponent as SalaryEntry } from './components/Finance/entry/entry.component';
import { EmpListComponent } from './components/Shared/emp-list/emp-list.component';
import { DateSelectComponent } from './components/attend/date-select/date-select.component';
import { DeleteComponent as SharedDelete } from './components/Shared/delete/delete.component';
import { EntryComponent as AttendEntry } from './components/attend/entry/entry.component';
import { IndexComponent as HistoryIndex } from './components/history/index/index.component';
import { EntryComponent as EmployeeEntry } from './components/employee/entry/entry.component';
import { LeaveComponent } from './leave/leave.component';
import { LeaveReqComponent } from './components/empHome/leave-req/leave-req.component';
import { MatSelectModule } from '@angular/material/select';
import { SalarydetailsComponent } from './components/empHome/salarydetails/salarydetails.component';
@NgModule({
  declarations: [
    AppComponent,
    NavbarComponent,
    FooterComponent,
    MainComponent,
    IndexComponent,
    EntryComponent,
    DeleteComponent,
    LoginComponent,
    HomeComponent,
    AttendanceComponent,
    ProfileComponent,
    SalaryComponent,
    SalaryEntry,
    EmpListComponent,
    DateSelectComponent,
    SharedDelete,
    AttendEntry,
    HistoryIndex,
    EmployeeEntry,
    LeaveComponent,
    LeaveReqComponent,
    SalarydetailsComponent
    
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    BrowserAnimationsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    ReactiveFormsModule,
    CommonModule,
    MatSelectModule
    
  
  ],
  providers: [{provide:HTTP_INTERCEPTORS,useClass:mainInterceptor,multi:true}],
  bootstrap: [AppComponent]
})
export class AppModule { }
