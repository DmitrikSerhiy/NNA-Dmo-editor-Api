import { BehaviorSubject } from 'rxjs';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class RightMenuGrabberService {

  private shouldShowGrabber = false;
  private shouldShowGrabberSource = new BehaviorSubject(false);
  shouldShowGrabber$ = this.shouldShowGrabberSource.asObservable();

  showGrabber() {
    this.shouldShowGrabberSource.next(true);
    this.shouldShowGrabber = true;
  }

  hideGrabber() {
    this.shouldShowGrabberSource.next(false);
    this.shouldShowGrabber = false;
  }

  isGrabbershouldBeShowen() {
    return this.shouldShowGrabber;
  }

  constructor() { }
}
