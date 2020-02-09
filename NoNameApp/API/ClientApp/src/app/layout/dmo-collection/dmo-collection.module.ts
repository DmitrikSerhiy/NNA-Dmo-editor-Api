import { Routes, RouterModule } from '@angular/router';
import { DmoCollectionComponent } from './dmo-collection.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgbCarouselModule, NgbAlertModule } from '@ng-bootstrap/ng-bootstrap';

const routes: Routes = [
    { path: '', component: DmoCollectionComponent }
];

@NgModule({
    imports: [
        CommonModule,
        NgbCarouselModule,
        NgbAlertModule,
        RouterModule.forChild(routes)
    ],
    declarations: [
        DmoCollectionComponent
    ]
})
export class DmoCollectionModule {}
