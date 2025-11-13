import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Employee } from '../Models/employee';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class EmployeeService {

  constructor(private client: HttpClient) { }
  getAllEmployees():Observable<Employee[]>
  {
    return this.client.get<Employee[]>(`${environment.baseUrl}/Employee/GetAll`);
  } 
  addNewEmp(emp : Employee):Observable<any>
  {
    return this.client.post<any>(`${environment.baseUrl}/Employee/Create`,emp);
  }
  editEmp(id:number,model:Employee):Observable<any>
  {
    return this.client.put<any>(`${environment.baseUrl}/Employee/Update/${id}`,model)
  }
  deleteEmp(id:number):Observable<any>
  {
    return this.client.delete<any>(`${environment.baseUrl}/Employee/Delete/${id}`)
  }
  getById(id:number):Observable<Employee>
  {
    return this.client.get<Employee>(`${environment.baseUrl}/Employee/GetById/${id}`)
  }
}
