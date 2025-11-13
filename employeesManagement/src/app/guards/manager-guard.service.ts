import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Router, RouterStateSnapshot, CanActivate } from '@angular/router';
import { AuthService } from '../Services/auth.service';
import { map } from 'rxjs/operators';
@Injectable({
  providedIn: 'root'
})
export class ManagerGuardService {

   constructor(private router: Router, private authService: AuthService) { }
  
    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
      const token = this.authService.getToken();
      if (!token) {
        this.router.navigateByUrl('/');
        return false;
      }
  
      return this.authService.userInfo$.pipe(
        map(user => {
          if (user && user.role == 'Manager' || user.role==="Admin") {
            return true;
          } else {
            this.router.navigateByUrl('/');
            return false;
          }
        })
      );
    }
}
