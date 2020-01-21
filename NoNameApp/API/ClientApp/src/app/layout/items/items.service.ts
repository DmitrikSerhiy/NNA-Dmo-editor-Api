import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class ItemsService {

  serverUrl = 'http://localhost:50680/api/Items/test';
  constructor(private http: HttpClient) { }

  post(data: string): Observable<string> {
      return this.http
      .post(this.serverUrl, {data: data})
      .pipe(map((response: string) => response));
  }
}
