import { RightMenues } from './../right-menues';
import { Component, OnInit, Output, EventEmitter, Renderer2 } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { UserManager } from 'src/app/shared/services/user-manager';

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

  @Output() collapsedEvent = new EventEmitter<boolean>();
  @Output() toggleRightMenu = new EventEmitter<RightMenues>();
  isAuthorized = false;

  constructor(
    public router: Router,
    public userManager: UserManager,
    private render2: Renderer2) {
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
    this.setSelected($event);
    this.toggleRightMenu.emit(RightMenues.dmoCollections);
  }

  setSelected($event) {
    const previouslySelected = this.render2.selectRootElement('.router-link-active', true);
    this.render2.removeClass(previouslySelected, 'router-link-active');

    const selected = $event.target.parentNode.localName === 'a'
      ? this.render2.selectRootElement(`#${$event.target.parentNode.id}`, true)
      : this.render2.selectRootElement(`#${$event.target.id}`, true);

      this.render2.addClass(selected, 'router-link-active');
  }
}
