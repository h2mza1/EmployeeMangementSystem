import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Salary } from '../Models/salary';
import { environment } from 'src/environments/environment';
import { env } from 'process';
import { basename } from 'path';
import { __param } from 'tslib';

@Injectable({
  providedIn: 'root'
})
export class SalaryService {

  constructor(private context:HttpClient) { }
  getAll():Observable<any>
  {
    return this.context.get<any>(`${environment.baseUrl}/EmpSal/All`);
  }
  addSalaryForAllEmp():Observable<Salary[]>
  {
        return this.context.post<Salary[]>(`${environment.baseUrl}/EmpSal/AddAllSalaries`,{});

  }
  getSalaryForThisMonth():Observable<any>
  {
    return this.context.get<any>(`${environment.baseUrl}/EmpSal/GetSalaryForThisMonth`);
  }
  getSalriesByDate(date:Date):Observable<any>
  {
        const dateStr = date.toISOString();

    return this.context.get<any>(`${environment.baseUrl}/EmpSal/GetSalariesByDate?date=${dateStr}`
     
    )
  }
  addSalary(model:Salary):Observable<Salary>
  {
    return this.context.post<Salary>(`${environment.baseUrl}/EmpSal/AddSalary`,model)
  }
  delete(id:number):Observable<any>
  {
    return this.context.delete<any>(`${environment.baseUrl}/EmpSal/Delete/${id}`);
  }
  getSalaryByUserAndDate(id?: number, date?: Date): Observable<Salary[]> {

  return this.context.get<Salary[]>(
    `${environment.baseUrl}/EmpSal/GetSalaryByUserAndDate?id=${id}&date=${date}`
  );
}

}
