import { Toastr } from './../../shared/services/toastr.service';
import { DmoCollectionsService } from './dmo-collections.service';
import { DmoListDto } from './dmoList.dto';

import { concatMap, map, catchError, } from 'rxjs/operators';
import { throwError, Observable } from 'rxjs';

import { Component, OnInit, Input } from '@angular/core';
import { FormGroup, Validators, FormControl } from '@angular/forms';

@Component({
  selector: 'app-dmo-collections',
  templateUrl: './dmo-collections.component.html',
  styleUrls: ['./dmo-collections.component.scss']
})
export class DmoCollectionsComponent implements OnInit {

  addCollectionForm: FormGroup;
  dmoLists: DmoListDto[];
  showAddButton = true;
  isFormProcessing = false;
  get collectionName() { return this.addCollectionForm.get('collectionName'); }
  @Input() rightMenuIsClosing$: Observable<void>;

  constructor(
    private dmoCollectionsService: DmoCollectionsService,
    private toastr: Toastr) { }


  ngOnInit() {
    this.rightMenuIsClosing$.subscribe(() => {
      this.toggleAddCollectionForm();
    });

    this.addCollectionForm = new FormGroup({
      'collectionName': new FormControl('', [Validators.required, Validators.maxLength(20)])
    });

    this.showLoader();
    this.dmoCollectionsService.getAll()
      .subscribe(
        (response: DmoListDto[]) => this.dmoLists = response,
        (error) => this.toastr.error(error),
        () => this.hideLoader() );
  }

  onAddCollection() {
    if (this.addCollectionForm.valid) {
      const collectionName = this.addCollectionForm.get('collectionName').value;
      this.showLoader();

      const add$ = this.dmoCollectionsService.addCollection(collectionName);
      const getAll$ = this.dmoCollectionsService.getAll() ;

      const addAndRefresh =
        add$.pipe(
          catchError(innerError => { this.hideLoader(); this.resetAddCollectionForm(); return throwError(innerError); } ),
          concatMap(() => getAll$.pipe(map((response: DmoListDto[]) => { this.dmoLists = response; } )) ));

        addAndRefresh.subscribe(
          () => {},
          (error) => this.toastr.error(error),
          () => { this.hideLoader(); this.toggleAddCollectionForm(); } );
    }
  }


  private toggleAddCollectionForm() {
    this.showAddButton = !this.showAddButton;
    this.resetAddCollectionForm();
  }

  private resetAddCollectionForm() {
    this.addCollectionForm.reset();
  }

  private showLoader() {
      this.isFormProcessing = true;
  }

  private hideLoader() {
      this.isFormProcessing = false;
  }
}
