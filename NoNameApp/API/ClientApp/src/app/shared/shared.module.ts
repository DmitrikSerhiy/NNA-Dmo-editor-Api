import { MatDialogModule } from '@angular/material/dialog';
import { RemoveCollectionPopupComponent } from './components/remove-collection-popup/remove-collection-popup.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

@NgModule({
  declarations: [RemoveCollectionPopupComponent],
  imports: [
    CommonModule,
    MatDialogModule
  ],
  exports: [RemoveCollectionPopupComponent]
})
export class SharedModule { }
