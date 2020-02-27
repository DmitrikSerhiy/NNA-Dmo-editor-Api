import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Routes, RouterModule } from '@angular/router';
import { DmoCollectionComponent } from './dmo-collection.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgbCarouselModule, NgbAlertModule } from '@ng-bootstrap/ng-bootstrap';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';

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
        MatSortModule,
        ReactiveFormsModule,
        FormsModule,
        MatProgressSpinnerModule,
        MatCheckboxModule
    ],
    declarations: [
        DmoCollectionComponent
    ]
})
export class DmoCollectionModule {}
