import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CollectionsManagerService {

  private currentCollectionIdSource = new BehaviorSubject('');
  currentCollectionId = this.currentCollectionIdSource.asObservable();

  constructor() { }

  setCollectionId(collectionId: string) {
    this.currentCollectionIdSource.next(collectionId);
  }
}
