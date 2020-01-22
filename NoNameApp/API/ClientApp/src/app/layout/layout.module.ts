import { AuthGuard } from '../shared/auth.guards';
import { SidebarComponent } from './sidebar/sidebar.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { LayoutComponent } from './layout.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule, CanActivate } from '@angular/router';

import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { ItemsComponent } from './items/items.component';

const routes: Routes = [
    {
        path: '',
        component: LayoutComponent,
        children: [
            { path: '', redirectTo: 'dashboard', pathMatch: 'prefix' },
            { path: 'dashboard', loadChildren: () => import('./dashboard/dashboard.module').then(m => m.DashboardModule) },
            { path: 'items', component: ItemsComponent, canActivate: [AuthGuard] },
        ]
    }
];

@NgModule({
    declarations: [LayoutComponent, NavMenuComponent, SidebarComponent, ItemsComponent],
    imports: [
        CommonModule,
        RouterModule.forChild(routes),
        NgbDropdownModule
    ]
})
export class LayoutModule {}
