import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { login } from '../Models/Login';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private logedIn = new BehaviorSubject<boolean>(this.hasValidToken());
  isLogedIn$ = this.logedIn.asObservable();

  private userInfo = new BehaviorSubject<any | null>(this.decodeToken());
  userInfo$ = this.userInfo.asObservable();

  constructor(private db: HttpClient) {}

  // 🔹 تسجيل الدخول
  login(model: login): Observable<{ success: string; data: string }> {
    return this.db.post<{ success: string; data: string }>(
      `${environment.baseUrl}/Auth/Login`,
      model
    );
  }

  setToken(token: string) {
    localStorage.setItem('token', token);
    this.logedIn.next(true);
    this.userInfo.next(this.decodeToken());
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  isLogged(): boolean {
    return this.hasValidToken();
  }

  private hasValidToken(): boolean {
    const token = this.getToken();
    if (!token) return false;

    const decoded = this.decodeToken();
    if (!decoded || !decoded.exp) return false;

    const isExpired = Date.now() >= decoded.exp * 1000;
    return !isExpired;
  }

  logout() {
    localStorage.removeItem('token');
    this.logedIn.next(false);
    this.userInfo.next(null);
  }

private decodeToken(): any {
  const token = this.getToken();
  if (!token) return null;

  try {
    // تأكد إنه JWT فعلاً
    const parts = token.split('.');
    if (parts.length !== 3) {
      console.warn('Token is not a valid JWT');
      return null;
    }

    const payload = parts[1];
    // دعم Base64 URL safe decoding
    const base64 = payload.replace(/-/g, '+').replace(/_/g, '/');
    const decoded = JSON.parse(atob(base64));
    return decoded;
  } catch (e) {
    console.error('Invalid token:', e);
    return null;
  }
}


  // 🔹 جلب بيانات المستخدم الحالي
  getUserInfo(): any {
    return this.decodeToken();
  }
}
