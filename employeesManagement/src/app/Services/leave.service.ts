import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Leave } from '../Models/leave';
import { environment } from 'src/environments/environment';
import { Vacation } from '../Models/vacation';

@Injectable({
  providedIn: 'root'
})
export class LeaveService {

  constructor(private context:HttpClient) { }
  getAll():Observable<Leave[]>
  {
    return this.context.get<Leave[]>(`${environment.baseUrl}/Leave/GetAll`)
  }
  approve(id?:number):Observable<Leave>
  {
    return this.context.put<Leave>(`${environment.baseUrl}/Leave/Approve/${id}`,{})
  }
  delete(id?:number):Observable<any>
  {
    return this.context.delete(`${environment.baseUrl}/Leave/Delete/${id}`)
  }
  reject(id?:number):Observable<Leave>
  {
    return this.context.put<Leave>(`${environment.baseUrl}/Leave/Reject/${id}`,{})
  }
  request(model:Leave):Observable<Leave>
  {
    return this.context.post<Leave>(`${environment.baseUrl}/Leave/Add`,model)
  }
  getVacations():Observable<Vacation[]>
  {
    return this.context.get<Vacation[]>(`${environment.baseUrl}/Vacation/GetAll`)
  }
  getRequestsByUserId(id: number) {
  return this.context.get<Leave[]>(`${environment.baseUrl}/Leave/getRequestsByUserId/${id}`);
}

}
