import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
  HttpResponse
} from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, tap, finalize } from 'rxjs';
import { AuthService } from '../Services/auth.service';
import { Router } from '@angular/router';
declare const swal: any;

@Injectable()
export class mainInterceptor implements HttpInterceptor {

  private authService = inject(AuthService);
  private router = inject(Router);

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const token = this.authService.getToken(); 
    let authReq = req;
    if (token) {
      authReq = req.clone({
        setHeaders: { Authorization: `Bearer ${token}` }
      });
    }

    const skipLoader =
      
      req.url.toLowerCase().includes('login') ||
      req.url.toLowerCase().includes('ischeck');
    const skipError = req.url.toLowerCase().includes('ischeck');

    if (!skipLoader) {
      swal({
        title: 'Processing...',
        text: 'Please wait',
        closeOnClickOutside: false,
        closeOnEsc: false,
        buttons: false,
        content: {
          element: 'div',
          attributes: {
            innerHTML: `
              <div class="loader" style="
                border: 4px solid #f3f3f3;
                border-top: 4px solid #3498db;
                border-radius: 50%;
                width: 40px;
                height: 40px;
                animation: spin 1s linear infinite;
                margin: 0 auto;">
              </div>
              <style>
                @keyframes spin {
                  0% { transform: rotate(0deg); }
                  100% { transform: rotate(360deg); }
                }
              </style>
            `
          }
        }
      });
    }

    return next.handle(authReq).pipe(
      tap({
        next: (res) => {
          // if (res instanceof HttpResponse  && !skipLoader && res.status!=200) {
          //   if(res.)
          //   swal({
          //     icon: 'success',
          //     title: 'Success',
          //     text: 'The operation completed successfully!',
          //     timer: 1500,
          //     buttons: false, 
          //   });
          // }
        },
        error: (error: HttpErrorResponse) => {
          if (!skipLoader && !skipError) {
            swal.close?.();
          }
          let errMsg = 'An unexpected error occurred';

          if (error.status === 401 || error.status === 403) {
                        errMsg="seision is timeout"

            this.authService.logout();
            this.router.navigateByUrl('/Login');

          }

          if (error.error) {
            if (typeof error.error === 'string') {
              errMsg = error.error;
            } else if (error.error.Message) {
              errMsg = error.error.Message;
            } else {
              errMsg = 'An unexpected error occurred';
            }
          } else errMsg = 'An unexpected error occurred';

          if (!skipError) {
            swal({
              icon: 'error',
              title: 'Error',
              text: errMsg,
              buttons: false,
            });
          }

          throw error;
        }
      }),
      finalize(() => {
        if (!skipLoader) {
          swal.close?.();
        }
      })
    );
  }
}
