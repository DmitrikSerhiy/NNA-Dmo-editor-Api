import { DmoListDto } from './dmoList.dto';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class DmoCollectionsService {

  serverUrl = 'http://localhost:50680/api/dmoList/';
  constructor(private http: HttpClient) { }

  getAll(): Observable<DmoListDto[]> {
    return this.http
      .get(this.serverUrl + 'all')
      .pipe(map((response: DmoListDto[]) => response));
  }


}
