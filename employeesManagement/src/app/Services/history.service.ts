import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { HistoryModel } from '../Models/history-model';
import { History } from '../Models/historyModel';

@Injectable({
  providedIn: 'root'
})
export class HistoryService {

  constructor(private context:HttpClient) { }

  getAll():Observable<HistoryModel>
  {
    return this.context.get<HistoryModel>(`${environment.baseUrl}/History/GetAll`)
  }
  restore(model:History):Observable<History>
  {
    return this.context.post<History>(`${environment.baseUrl}/History/Restore`,model)
  }
 delete(model: History): Observable<any> {
  return this.context.request('delete', `${environment.baseUrl}/History/Delete`, {
    body: model
  });
}

}
