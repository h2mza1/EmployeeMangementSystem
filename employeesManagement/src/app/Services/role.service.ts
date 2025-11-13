import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { role } from '../Models/Role';

@Injectable({
  providedIn: 'root'
})
export class RoleService {

  constructor(private context:HttpClient) { }

  getAll():Observable<role[]>
  {
    return this.context.get<role[]>(`${environment.baseUrl}/Role/GetAll`);
  }
  getById(id:number):Observable<role>
  {
      return this.context.get<role>(`${environment.baseUrl}/Role/GetById/${id}`);
  }
  getUserRole():Observable<string>
  {
    return this.context.get<string>(`${environment.baseUrl}/Role/getUserRole`)
  }
}
