import { Routes, RouterModule } from '@angular/router';
import { DmoCollectionComponent } from './dmo-collection.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgbCarouselModule, NgbAlertModule } from '@ng-bootstrap/ng-bootstrap';
import {MatTableModule} from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';

const routes: Routes = [
    { path: '', component: DmoCollectionComponent }
];

@NgModule({
    imports: [
        CommonModule,
        NgbCarouselModule,
        NgbAlertModule,
        RouterModule.forChild(routes),
        MatTableModule,
        MatPaginatorModule,
        MatSortModule
    ],
    declarations: [
        DmoCollectionComponent
    ]
})
export class DmoCollectionModule {}
