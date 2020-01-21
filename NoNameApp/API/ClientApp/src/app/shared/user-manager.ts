import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable()
export class UserManager {

    constructor(private http: HttpClient) { }

    isAuthorized() {
        return localStorage.getItem('user access token') !== null;
    }

    getJWT() {
        return localStorage.getItem('user access token');
    }

}
