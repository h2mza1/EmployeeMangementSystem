import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Department } from '../Models/department';

@Injectable({
  providedIn: 'root'
})
export class DepartmentService {

  constructor(private client:HttpClient) { }

  getAll():Observable<Department[]>
  {
    return this.client.get<Department[]>(`${environment.baseUrl}/Department/GetAll`)
  }
  addDept(name:string):Observable<Department>
  {
    const dept: Department = { Id: 0, Name: name };
   
    return this.client.post<Department>(`${environment.baseUrl}/Department/Create`,dept);
  }
  deleteDept(id:number):Observable<Department>
  {
    return this.client.delete<Department>(`${environment.baseUrl}/Department/Delete/${id}`);
  }
  updateDept(id:number , Name:string):Observable<Department>
  {
    const dept : Department = {Id:id , Name : Name}
    return this.client.put<Department>(`${environment.baseUrl}/Department/Update/${id}`,dept)
  }
}
