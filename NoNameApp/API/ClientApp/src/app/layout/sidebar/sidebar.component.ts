import { RightMenues } from './../right-menues';
import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { UserManager } from 'src/app/shared/user-manager';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss']
})
export class SidebarComponent implements OnInit {
  isActive: boolean;
  collapsed: boolean;
  showMenu: string;
  pushRightClass: string;
  dmoCollectionsSelected = false;
  menues: any[] = [];

  @Output() collapsedEvent = new EventEmitter<boolean>();
  @Output() toggleRightMenu = new EventEmitter<RightMenues>();
  isAuthorized = false;

  constructor(
    public router: Router,
    public userManager: UserManager) {
    this.isAuthorized = userManager.isAuthorized();
    this.router.events.subscribe(val => {
      if (
        val instanceof NavigationEnd &&
        window.innerWidth <= 992 &&
        this.isToggled()
      ) {
        this.toggleSidebar();
      }
    });
  }

  ngOnInit() {
    this.isActive = false;
    this.collapsed = false;
    this.showMenu = '';
    this.pushRightClass = 'push-right';
    Object.values(RightMenues).forEach(menu => {
      this.menues.push({name: menu, isSelected: false});
    });
  }

  eventCalled() {
    this.isActive = !this.isActive;
  }

  addExpandClass(element: any) {
    if (element === this.showMenu) {
      this.showMenu = '0';
    } else {
      this.showMenu = element;
    }
  }

  toggleCollapsed() {
    this.collapsed = !this.collapsed;
    this.collapsedEvent.emit(this.collapsed);
  }

  isToggled(): boolean {
    const dom: Element = document.querySelector('body');
    return dom.classList.contains(this.pushRightClass);
  }

  toggleSidebar() {
    const dom: any = document.querySelector('body');
    dom.classList.toggle(this.pushRightClass);
  }

  sendDmoCollectionsEvent($event) {
    //this.menues.map(m => m.isSelected = m.name === $event.target.id);
    this.dmoCollectionsSelected = true;
    this.toggleRightMenu.emit(RightMenues.dmoCollections);
  }

  // isSelected(tabName) {
  //   console.log('is selected' + tabName);
  //   return this.menues.find(m => m.name === tabName).isSelected;
  // }

}
