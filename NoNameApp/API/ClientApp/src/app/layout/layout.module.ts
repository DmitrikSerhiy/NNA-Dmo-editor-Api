import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthGuard } from '../shared/services/auth.guards';
import { SidebarComponent } from './sidebar/sidebar.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { LayoutComponent } from './layout.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import {MatSidenavModule} from '@angular/material/sidenav';

import { NgbDropdownModule, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { DmoCollectionsComponent } from './dmo-collections/dmo-collections.component';
import { ReactiveFormsModule } from '@angular/forms';

const routes: Routes = [
    {
        path: '',
        component: LayoutComponent,
        children: [
            { path: '', redirectTo: 'dashboard', pathMatch: 'prefix' },
            { path: 'dashboard', loadChildren: () => import('./dashboard/dashboard.module').then(m => m.DashboardModule) },
            { path: 'dmo', loadChildren: () => import('./dmo-collection/dmo-collection.module').then(m => m.DmoCollectionModule) },
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
        ReactiveFormsModule,
        MatSidenavModule,
        MatProgressSpinnerModule,
        NgbModule
    ]
})
export class LayoutModule {}
