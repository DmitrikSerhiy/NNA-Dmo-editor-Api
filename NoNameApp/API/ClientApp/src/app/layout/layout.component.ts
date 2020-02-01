import { RightMenues } from './right-menues';
import { Component, OnInit } from '@angular/core';

@Component({
    selector: 'app-layout',
    templateUrl: './layout.component.html',
    styleUrls: ['./layout.component.scss']
})
export class LayoutComponent implements OnInit {

    collapedSideBar: boolean;
    toggleRightMenu: RightMenues;
    currentMenuName: string;

    constructor() {}

    ngOnInit() { }

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
