import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment'
import { CashDesk } from '../models/cashdesk';

@Injectable({
  providedIn: 'root',
 })
export class CashDeskService {

  apiConfig = environment.api.storeApi.cashDesk;
  
  constructor(private http: HttpClient) {
  }

  getCashDesks() : Observable<CashDesk[]> {
    return this.http.get<CashDesk[]>(this.apiConfig.getAll);
  }

  getCashDesk(id : string) : Observable<CashDesk> {
    let url = this.apiConfig.getId;
    url = url.replace("{{ID}}", id);
    return this.http.get<CashDesk>(url);
  }

  addCashDesk(cashDesk : CashDesk) : Observable<CashDesk> {
    let url = this.apiConfig.post;
    return this.http.post<CashDesk>(url, cashDesk);
  }

  updateCashDesk(cashDesk : CashDesk) : Observable<CashDesk> {
    let url = this.apiConfig.put;
    return this.http.put<CashDesk>(url, cashDesk);
  }

  deleteCashDesk(id : string) : Observable<CashDesk> {
    let url = this.apiConfig.remove;
    url = url.replace("{{ID}}", id);
    return this.http.delete<any>(url);
  }
}