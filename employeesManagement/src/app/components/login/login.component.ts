import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/Services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
 loginForm:FormGroup
  constructor(private auth:AuthService ,private route:Router) {
    this.loginForm= new FormGroup(
      {
        Email:new FormControl('',[Validators.required,Validators.email]),
        Password:new FormControl('',[Validators.required]),
      }
    )

   }

   submit()
   {

     if (this.loginForm.valid) {
    this.auth.login(this.loginForm.value).subscribe({
      next: (res) => {  
        this.auth.setToken(res.data);
        console.log(res.data)
        this.auth.userInfo$
        .subscribe(
        {
          next:(res)=>
          {
            if(res.role=="Employee")
            this.route.navigateByUrl("/Home")
            else
            {
             this.route.navigateByUrl("/Employee")

            }
          }
          
        }
        )

      },
      error: (err) => {
        console.error(err);
      }
    });
  }
   }
  ngOnInit(): void {
  }

}
