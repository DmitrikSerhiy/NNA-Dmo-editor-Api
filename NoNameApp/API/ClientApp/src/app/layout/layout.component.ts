import { Observable } from 'rxjs';
import { RightMenues } from './right-menues';
import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { MatSidenav } from '@angular/material/sidenav';

@Component({
    selector: 'app-layout',
    templateUrl: './layout.component.html',
    styleUrls: ['./layout.component.scss']
})
export class LayoutComponent implements OnInit, AfterViewInit  {

    @ViewChild('rightMenu', {static: true}) rightMenu: MatSidenav;
    collapedSideBar: boolean;
    toggleRightMenu: RightMenues;
    currentMenuName: string;
    rightMenuIsClosing$: Observable<void>;

    constructor() {}

    ngOnInit() { }

    ngAfterViewInit(): void {
        this.rightMenuIsClosing$ = this.rightMenu.closedStart;
    }

    receiveCollapsed($event) {
        this.collapedSideBar = $event;
    }

    receiveRightMenu($event) {
        if ($event === RightMenues.dmoCollections) {
            this.currentMenuName = RightMenues.dmoCollections;
        }
        this.toggleRightMenu = $event;
    }
}
