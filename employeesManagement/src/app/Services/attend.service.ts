import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Attendance } from '../Models/attendance';

@Injectable({
  providedIn: 'root'
})
export class AttendService {
  private isCheckedOut = new BehaviorSubject<boolean>(false);
  isCheckedOut$ = this.isCheckedOut.asObservable();

  constructor(private _context: HttpClient) {
  }

  /** Load current check-out state from the API */
  public loadCheckOutState(): void {
    this.isCheckOut().subscribe({
      next: res => this.isCheckedOut.next(res),
      error: err => console.error('Failed to load checkout state', err)
    });
  }

  getAll(): Observable<Attendance[]> {
    return this._context.get<Attendance[]>(`${environment.baseUrl}/Attendance/GetAllAttendance`);
  }

  getAttendanceForThisDay(): Observable<Attendance[]> {
    return this._context.get<Attendance[]>(`${environment.baseUrl}/Attendance/GetAttendanceForThisDay`);
  }

  getAttendanceByDate(date: Date): Observable<Attendance[]> {
    const dateStr = date.toISOString();
    return this._context.get<Attendance[]>(`${environment.baseUrl}/Attendance/GetAttendanceByDate?date=${dateStr}`);
  }

  delete(id: number): Observable<Attendance[]> {
    return this._context.delete<Attendance[]>(`${environment.baseUrl}/Attendance/Delete/${id}`);
  }

  add(model: Attendance): Observable<Attendance> {
    return this._context.post<Attendance>(`${environment.baseUrl}/Attendance/Add`, model);
  }

  getById(id: number): Observable<Attendance> {
    return this._context.get<Attendance>(`${environment.baseUrl}/Attendance/GetAttendanceById/${id}`);
  }

  update(id: number, model: Attendance): Observable<Attendance> {
    return this._context.put<Attendance>(`${environment.baseUrl}/Attendance/Update/${id}`, model);
  }

  /** ✅ Check out and update the BehaviorSubject */
  checkOut(): Observable<any> {
    return this._context.post(`${environment.baseUrl}/Attendance/CheckOut`, {}).pipe(
      tap(() => this.isCheckedOut.next(true))
    );
  }

  /** Returns true if user already checked out */
  isCheckOut(): Observable<boolean> {
    return this._context.get<boolean>(`${environment.baseUrl}/Attendance/IsCheckOut`);
  }
  getCountPresentDay(id:number):Observable<number>
  {
    return this._context.get<number>(`${environment.baseUrl}/Attendance/GetCountOfPresentDay/${id}`)
  }
   getByUserId(id: number , month?:number , year?:number): Observable<Attendance[]> {

return this._context.get<Attendance[]>(
  `${environment.baseUrl}/Attendance/GetAttendanceByMonthAndUserId/${id}?month=${month}&year=${year}`
);
  }
  
}
