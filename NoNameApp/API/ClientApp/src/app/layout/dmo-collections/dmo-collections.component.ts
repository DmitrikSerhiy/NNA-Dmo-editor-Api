import { DmoCollectionsService } from './dmo-collections.service';
import { DmoListDto } from './dmoList.dto';

import { Component, OnInit, Renderer2 } from '@angular/core';
import { FormGroup, Validators, FormControl } from '@angular/forms';

@Component({
  selector: 'app-dmo-collections',
  templateUrl: './dmo-collections.component.html',
  styleUrls: ['./dmo-collections.component.scss']
})
export class DmoCollectionsComponent implements OnInit {

  addCollectionForm: FormGroup;
  get collectionName() { return this.addCollectionForm.get('collectionName'); }

  dmoLists: DmoListDto[];
  showAddButton = true;
  constructor(private dmoCollectionsService: DmoCollectionsService, private render: Renderer2) { }


  ngOnInit() {
    this.addCollectionForm = new FormGroup({
      'collectionName': new FormControl('', [Validators.required, Validators.maxLength(20)])
    });

    this.dmoCollectionsService.getAll().subscribe((response: DmoListDto[]) => {
      this.dmoLists = response;
    });
  }

  toggleAddCollectionForm() {
    this.showAddButton = !this.showAddButton;
    this.addCollectionForm.reset();
    //todo: should I remove component when mat-slide is closing?
  }


  onAddCollection() {
    if (this.addCollectionForm.valid) {
      const newCollectionName = this.addCollectionForm.get('collectionName').value;
    }
  }


}
