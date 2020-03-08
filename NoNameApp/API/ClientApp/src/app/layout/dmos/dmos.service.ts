import { DmoShortDto } from './../models';
import { HttpErrorResponse, HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { map } from 'rxjs/operators';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class DmosService {

  serverUrl = 'http://localhost:50680/api/dmos/';
  constructor(private http: HttpClient) { }

  getAlldmos(): Observable<DmoShortDto[]> {
    return this.http
      .get(this.serverUrl)
      .pipe(
        map((response: DmoShortDto[]) => response),
        catchError(this.handleError));
  }


  private handleError(err: HttpErrorResponse) {
    const errorMessage = err.error.errorMessage;
    if (!errorMessage) {
      return throwError('Server error. Try later.');
    }

    return throwError(errorMessage);
  }
}
