import { DmoListDto } from './dmoList.dto';
import { HttpClient, HttpErrorResponse, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { map } from 'rxjs/operators';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class DmoCollectionsService {

  serverUrl = 'http://localhost:50680/api/dmoList/';
  constructor(private http: HttpClient) { }

  getAll(): Observable<DmoListDto[]> {
    return this.http
      .get(this.serverUrl + 'all')
      .pipe(
        map((response: DmoListDto[]) => response),
        catchError(this.handleError));
  }

  addCollection(collectionName: string): Observable<any> {
    return this.http
      .post(this.serverUrl, { CollectionName: collectionName })
      .pipe(
        map(response => response ),
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
