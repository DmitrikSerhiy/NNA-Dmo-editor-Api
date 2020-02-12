import { DmoCollectionDto } from './../models';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { map } from 'rxjs/operators';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class DmoCollectionService {

  serverUrl = 'http://localhost:50680/api/dmoCollections/';
  constructor(private http: HttpClient) { }

  getWithDmos(collectionId: string): Observable<DmoCollectionDto> {
    return this.http
      .get(this.serverUrl + collectionId)
      .pipe(
        map((response: DmoCollectionDto) => response),
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
