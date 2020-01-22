import { UserDetails } from '../models/userDetails';
import { HttpClient, HttpParams, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { Observable, throwError } from 'rxjs';
import { map } from 'rxjs/operators';
import { catchError } from 'rxjs/operators';

@Injectable()
export class AuthService {

    //todo: relocate it to environment file
    serverUrl = 'http://localhost:50680/api/account/';
    constructor(private http: HttpClient) { }

    authorize(email: string, password: string): Observable<UserDetails> {
        const params = new HttpParams()
            .set('email', email)
            .set('password', password);

        return this.http
            .post(this.serverUrl + 'token', {}, { params })
            .pipe(
                map((response: UserDetails) => response),
                catchError(this.handleError));
    }

    register(userName: string, email: string, password: string): Observable<UserDetails> {
        const params = new HttpParams()
            .set('userName', userName)
            .set('email', email)
            .set('password', password);

        return this.http
            .post(this.serverUrl + 'register', {}, { params })
            .pipe(
                map((response: UserDetails) => response),
                catchError(this.handleError));
    }

    private handleError(err: HttpErrorResponse) {
        let errorMessage = '';
        if (err.error instanceof Error) {
            // A client-side or network error occurred
            errorMessage = `An error occurred: ${err.error.message}`;
        } else {
            // The backend returned an unsuccessful response code.
            errorMessage = `Server returned code: ${err.status}, error message is: ${err.message}`;
        }
        console.error(errorMessage);
        return throwError(errorMessage);
    }
}
