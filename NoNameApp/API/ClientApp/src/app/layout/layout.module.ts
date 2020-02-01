import { AuthGuard } from '../shared/auth.guards';
import { SidebarComponent } from './sidebar/sidebar.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { LayoutComponent } from './layout.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import {MatSidenavModule} from '@angular/material/sidenav';

import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { DmoCollectionsComponent } from './dmo-collections/dmo-collections.component';

const routes: Routes = [
    {
        path: '',
        component: LayoutComponent,
        children: [
            { path: '', redirectTo: 'dashboard', pathMatch: 'prefix' },
            { path: 'dashboard', loadChildren: () => import('./dashboard/dashboard.module').then(m => m.DashboardModule) },
            { path: 'dmoCollections', component: DmoCollectionsComponent, canActivate: [AuthGuard] },
        ]
    }
];

@NgModule({
    declarations: [LayoutComponent, NavMenuComponent, SidebarComponent, DmoCollectionsComponent],
    imports: [
        CommonModule,
        RouterModule.forChild(routes),
        NgbDropdownModule,
        MatSidenavModule
    ]
})
export class LayoutModule {}
