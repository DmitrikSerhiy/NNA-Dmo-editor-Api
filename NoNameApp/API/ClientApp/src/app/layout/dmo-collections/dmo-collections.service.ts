import { DmoCollectionShortDto } from './../models';
import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { map } from 'rxjs/operators';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class DmoCollectionsService {

  serverUrl = 'http://localhost:50680/api/dmoCollections/';
  constructor(private http: HttpClient) { }

  getAll(): Observable<DmoCollectionShortDto[]> {
    return this.http
      .get(this.serverUrl)
      .pipe(
        map((response: DmoCollectionShortDto[]) => response),
        catchError(this.handleError));
  }

  addCollection(collectionName: string): Observable<any> {
    return this.http
      .post(this.serverUrl, { CollectionName: collectionName })
      .pipe(
        map(response => response ),
        catchError(this.handleError));
  }

  deleteCollection(collectionId: string): Observable<any> {
    return this.http
      .delete(this.serverUrl, {params: new HttpParams().set('collectionId', collectionId)})
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
