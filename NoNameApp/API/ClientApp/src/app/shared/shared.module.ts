import { MatDialogModule } from '@angular/material/dialog';
import { RemoveCollectionPopupComponent } from './components/remove-collection-popup/remove-collection-popup.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RightMenuGrabberComponent } from './components/right-menu-grabber/right-menu-grabber.component';

@NgModule({
  declarations: [RemoveCollectionPopupComponent, RightMenuGrabberComponent],
  imports: [
    CommonModule,
    MatDialogModule
  ],
  exports: [RemoveCollectionPopupComponent, RightMenuGrabberComponent]
})
export class SharedModule { }
