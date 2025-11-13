import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Route, Router } from '@angular/router';
import { error, log } from 'console';
import { Employee } from 'src/app/Models/employee';
import { AuthService } from 'src/app/Services/auth.service';
import { json } from 'stream/consumers';
import { IndexComponent } from '../Shared/profile/index/index.component';
import { AttendService } from 'src/app/Services/attend.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {
  isLogIn:boolean=false;
  info:string=""
  role :string =""
  userId:number=0 
  out:boolean=false
  constructor(private auth:AuthService ,private route:Router, private dialog:MatDialog  , private attendService:AttendService) 
  {
    auth.isLogedIn$
    .subscribe(
      {
        next:(res)=>
        {
          if(res==true)
 {  this.auth.userInfo$
    .subscribe(
     {
      next:(res)=>{ 
        this.info=res.unique_name
        this.role=res.role
        this.userId=res.nameid
      console.log(res)},
      error:(err)=>
      {
        console.log(err)
      }
    })}
        }
      }
    )
  
   }
  
ngOnInit(): void {
  this.auth.isLogedIn$.subscribe({
    next: (res) => {
      this.isLogIn = res;

      if (res) {
        this.auth.userInfo$.subscribe({
          next: (user) => {
            this.info = user.unique_name;
            this.role = user.role;
            this.userId = user.nameid;
            console.log('User Info:', user);

            // ✅ تحميل حالة الـ CheckOut من السيرفر
            this.attendService.loadCheckOutState();

            // ✅ متابعة حالة الـ CheckOut بشكل لحظي
            this.attendService.isCheckedOut$.subscribe(out => {
              this.out = out;
              console.log('CheckedOut state:', out);
            });
          },
          error: (err) => console.error('Failed to load user info', err)
        });
      }
    },
    error: (err) => alert(err)
  });
} 

logout()
{
  this.auth.logout()
  this.route.navigateByUrl("/Login")
}
openDialog(id?:number)
{
  const dialogRef = this.dialog.open(IndexComponent,
    {
      width:"500px",
      data:id
     
    }
  )
}
checkOut() {
  this.attendService.checkOut().subscribe({
    next: (res) => {console.log('Checked out successfully')
      this.out=res
    },
    error: (err) => {console.error(err)}
  });
}


}
