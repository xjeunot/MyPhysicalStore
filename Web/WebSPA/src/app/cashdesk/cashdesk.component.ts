import { Component, OnInit } from '@angular/core';
import { HttpErrorResponse} from '@angular/common/http';

import { throwError } from 'rxjs';
import { catchError, retry } from 'rxjs/operators';

import { CashDesk} from '../shared/models/cashdesk';
import { CashDeskService } from '../shared/services/cash-desk.service'
import { resetComponentState } from '@angular/core/src/render3/state';

@Component({
  selector: 'app-cashdesk',
  templateUrl: './cashdesk.component.html',
  styleUrls: ['./cashdesk.component.css'],
  providers:  [ CashDeskService ]
})

export class CashdeskComponent implements OnInit {

  cashdeskStates = ['open', 'last_customer', 'close'];

  cashDeskList : Array<CashDesk>;
  cashDeskDetail : CashDesk;
  cashDeskEdit : CashDesk;

  constructor(private service: CashDeskService) {
    this.cashDeskList = new Array<CashDesk>();
  }

  ngOnInit() {
    this.viewList();
  }

  reset(): void {
    this.cashDeskList.splice(0);
    this.cashDeskDetail = null;
    this.cashDeskEdit = null;
  }

  viewList() : void {
    // Reset.
    this.reset();

    // Load List From Services.
    this.service.getCashDesks()
    .pipe(
      retry(3),
      catchError(this.handleError)
    )
    .subscribe(
      (data: CashDesk[]) => this.cashDeskList = data,
      error => this.handleError(error) // error path
    );
  }

  viewNew() : void {
    // Reset.
    this.reset();

    // Create the new Item.
    this.cashDeskEdit = new CashDesk();
  }

  viewDetail(id : string) : void {
    // Reset.
    this.reset();

    // Load Element From Services.
    this.service.getCashDesk(id)
    .pipe(
      retry(3),
      catchError(this.handleError)
    )
    .subscribe(
      (data: CashDesk) => this.cashDeskDetail = data,
      error => this.handleError(error) // error path
    );
  }

  viewEdit(id : string) : void {
    // Reset.
    this.reset();

    // Load Element From Services.
    this.service.getCashDesk(id)
    .pipe(
      retry(3),
      catchError(this.handleError)
    )
    .subscribe(
      (data: CashDesk) => this.cashDeskEdit = data,
      error => this.handleError(error) // error path
    );
  }

  viewDelete(id : string) : void {
    // Load Element From Services.
    this.service.deleteCashDesk(id)
    .pipe(
      retry(3),
      catchError(this.handleError)
    )
    .subscribe(
      (data: CashDesk) => this.viewList(),
      error => this.handleError(error) // error path
    );
  }

  onSubmit() : void {
    // Create Mode.
    if (this.cashDeskEdit.id == "")
    {
      this.service.addCashDesk(this.cashDeskEdit)
      .pipe(
        retry(3),
        catchError(this.handleError)
      )
      .subscribe(
        (data: CashDesk) => this.viewList(),
        error => this.handleError(error) // error path
      );
    }
    // Edit Mode.
    if (this.cashDeskEdit.id != "")
    {
      this.service.updateCashDesk(this.cashDeskEdit)
      .pipe(
        retry(3),
        catchError(this.handleError)
      )
      .subscribe(
        (data: CashDesk) => this.viewDetail(this.cashDeskEdit.id),
        error => this.handleError(error) // error path
      );
    }
  }

  handleError(error: HttpErrorResponse) : any {
    if (error.error instanceof ErrorEvent) {
      // A client-side or network error occurred. Handle it accordingly.
      console.error('An error occurred:', error.error.message);
    }
    else {
      // The backend returned an unsuccessful response code.
      // The response body may contain clues as to what went wrong,
      console.error(
        `Backend returned code ${error.status}, ` +
        `body was: ${error.error}`);
    }
    // return an observable with a user-facing error message
    return throwError(
      'Something bad happened; please try again later.');
  };
}