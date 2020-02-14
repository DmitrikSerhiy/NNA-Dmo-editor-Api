import { DmoCollectionShortDto } from './../models';
import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

import { Toastr } from './../../shared/services/toastr.service';
import { DmoCollectionsService } from './dmo-collections.service';

import { concatMap, map, catchError, } from 'rxjs/operators';
import { throwError, Observable  } from 'rxjs';

import { Component, OnInit, Input, ViewChild, Output, EventEmitter } from '@angular/core';
import { FormGroup, Validators, FormControl } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-dmo-collections',
  templateUrl: './dmo-collections.component.html',
  styleUrls: ['./dmo-collections.component.scss']
})
export class DmoCollectionsComponent implements OnInit {

  addCollectionForm: FormGroup;
  dmoLists: DmoCollectionShortDto[];
  showAddButton = true;
  isFormProcessing = false;
  selectedDmoCollectionName: DmoCollectionShortDto;
  get collectionName() { return this.addCollectionForm.get('collectionName'); }
  @Input() rightMenuIsClosing$: Observable<void>;
  @Output() closeRightMenu = new EventEmitter<void>();
  @ViewChild('removeCollectionModal', {static: true}) removeModal: NgbActiveModal;

  constructor(
    private dmoCollectionsService: DmoCollectionsService,
    private toastr: Toastr,
    private modalService: NgbModal,
    private router: Router) { }


  ngOnInit() {
    this.rightMenuIsClosing$.subscribe(() => {
      this.toggleAddCollectionForm(true);
    });

    this.addCollectionForm = new FormGroup({
      'collectionName': new FormControl('', [Validators.required, Validators.maxLength(20)])
    });

    this.showLoader();
    this.dmoCollectionsService.getAll()
      .subscribe(
        (response: DmoCollectionShortDto[]) => this.dmoLists = response,
        (error) => this.toastr.error(error),
        () => this.hideLoader() );
  }

  openCollection(id: string) {
    this.closeRightMenu.emit();
    this.router.navigate(['/dmo', { id: id}]);
  }

  onAddCollection() {
    if (this.addCollectionForm.valid) {
      const collectionName = this.addCollectionForm.get('collectionName').value;
      this.showLoader();

      const add$ = this.dmoCollectionsService.addCollection(collectionName);
      const getAll$ = this.dmoCollectionsService.getAll() ;

      const addAndRefresh$ =
        add$.pipe(
          catchError(innerError => { this.hideLoader(); this.resetAddCollectionForm(); return throwError(innerError); } ),
          concatMap(() => getAll$.pipe(map((response: DmoCollectionShortDto[]) => { this.dmoLists = response; } )) ));

          this.subscribe(addAndRefresh$);
    }
  }

  async onDeleteCollection(dmoList: DmoCollectionShortDto) {
    this.selectedDmoCollectionName = dmoList;
    const modalRef = this.modalService.open(this.removeModal);
    const sendRemoveRequest = await modalRef.result.then(() => true, () => false);

    if (!sendRemoveRequest) {
      return;
    }
    this.showLoader();
    const delete$ = this.dmoCollectionsService.deleteCollection(this.selectedDmoCollectionName.id);
    const getAll$ = this.dmoCollectionsService.getAll() ;

    const deleteAndRefresh$ =
      delete$.pipe(
        catchError(innerError => { this.hideLoader(); this.resetAddCollectionForm(); return throwError(innerError); } ),
        concatMap(() => getAll$.pipe(map((response: DmoCollectionShortDto[]) => { this.dmoLists = response; } )) ));

        this.subscribe(deleteAndRefresh$) ;
  }

  private subscribe(obserabable$: Observable<void>) {
    obserabable$.subscribe(
      () => {},
      (error) => this.toastr.error(error),
      () => { this.hideLoader(); this.toggleAddCollectionForm(true); } );
  }

  private toggleAddCollectionForm(close = false) {
    if (close) {
      this.showAddButton = true;
    } else {
      this.showAddButton = !this.showAddButton;
    }
    this.resetAddCollectionForm();

    if (!this.showAddButton) {
      //todo: set focus on field here. It does not work
    }
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
