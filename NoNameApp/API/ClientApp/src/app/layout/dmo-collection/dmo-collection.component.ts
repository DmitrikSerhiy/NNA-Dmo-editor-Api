import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { CollectionsManagerService } from './../../shared/services/collections-manager.service';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Toastr } from './../../shared/services/toastr.service';
import { DmoCollectionDto, DmoShortDto, DmoCollectionShortDto } from './../models';
import { Component, OnInit, ViewChild, AfterViewInit, OnDestroy, Output, EventEmitter } from '@angular/core';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { MatPaginator } from '@angular/material/paginator';
import { concatMap, map, takeUntil, finalize } from 'rxjs/operators';
import { throwError, Observable, Subject } from 'rxjs';
import { DmoCollectionsService } from 'src/app/shared/services/dmo-collections.service';

@Component({
  selector: 'app-dmo-collection',
  templateUrl: './dmo-collection.component.html',
  styleUrls: ['./dmo-collection.component.scss']
})
export class DmoCollectionComponent implements OnInit, OnDestroy {

  currentDmoCollection: DmoCollectionDto;
  shouldShowTable = false;
  table: MatTableDataSource<DmoShortDto>;
  displayedColumns: string[];
  resultsLength = 0;
  clickedRow: DmoShortDto;

  @ViewChild('removeFullCollectionModal', { static: true }) removeModal: NgbActiveModal;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;

  editCollectionNameForm: FormGroup;
  get collectionName() { return this.editCollectionNameForm.get('collectionName'); }
  showEditForm = false;
  searchValue: any;

  private unsubscribe$: Subject<void> = new Subject();

  constructor(
    private dmoCollectionService: DmoCollectionsService,
    private route: ActivatedRoute,
    private modalService: NgbModal,
    private toastr: Toastr,
    private router: Router,
    private collectionManager: CollectionsManagerService) { }

  ngOnInit() {
    this.editCollectionNameForm = new FormGroup({
      'collectionName': new FormControl('', [Validators.required, Validators.maxLength(20)])
    });

    let dmoObserver = this.loadDmos();
    this.route.params.subscribe(p => {
      if (!p['id']) {
        return;
      }
      dmoObserver = this.loadDmos();
    });
  }

  ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  onRowSelect(row: DmoShortDto) {
    if (this.showEditForm) {
      return;
    }

    if (this.clickedRow && this.clickedRow === row) {
      this.clickedRow = null;
      return;
    }
    this.clickedRow = row;
    console.log(row);
  }

  redirectToDmo() {
    if (!this.clickedRow) {
      return;
    }
    console.log('dmo editor is not implement yet');
  }

  removeFromCollection() {
    if (!this.clickedRow) {
      return;
    }

    const removeFromCollection$ = this.dmoCollectionService
      .removeFromCollection(this.clickedRow.id, this.currentDmoCollection.id);

    const removeAndReload$ = removeFromCollection$.pipe(
      takeUntil(this.unsubscribe$),
      map(() => this.loadDmos()));

    removeAndReload$.subscribe({
      error: (err) => { this.toastr.error(err); }
    });
  }

  onEditCollectionName() {
    if (this.editCollectionNameForm.valid) {
      const newCollectionName = this.editCollectionNameForm.get('collectionName').value;
      if (this.currentDmoCollection.collectionName === newCollectionName) {
        this.hideEditCollectionNameForm();
        return;
      }

      const collectionId = this.route.snapshot.paramMap.get('id');
      const updateCollectionName$ = this.dmoCollectionService.updateCollectionName(collectionId, newCollectionName);
      const getCollectionName$ = this.dmoCollectionService.getCollectionName(collectionId);

      const updateAndGet$ =
        updateCollectionName$.pipe(
          takeUntil(this.unsubscribe$),
          finalize(() => this.hideEditCollectionNameForm()),
          concatMap(() => getCollectionName$.pipe(
            takeUntil(this.unsubscribe$),
            finalize(() => this.collectionManager.setCollectionId(collectionId)),
            map((response: DmoCollectionShortDto) => {
              this.currentDmoCollection.collectionName = response.collectionName;
            }))
          ));

      updateAndGet$.subscribe({
        error: (err) => { this.toastr.error(err); },
      });
    }
  }

  async onRemoveCollection() {
    console.log(this.currentDmoCollection);
    const modalRef = this.modalService.open(this.removeModal);
    const shouldSendRemoveRequest = await modalRef.result.then(() => true, () => false);

    if (!shouldSendRemoveRequest) {
      return;
    }

    const deleteAndRedirect$ = this.dmoCollectionService.deleteCollection(this.currentDmoCollection.id);

    deleteAndRedirect$.subscribe({
      next: () => { this.redirectToDashboard(); },
      error: (err) => { this.toastr.error(err); },
    });
  }

  hideEditCollectionNameForm() {
    this.editCollectionNameForm.reset();
    this.showEditForm = false;
  }

  showEditCollectionNameForm() {
    this.editCollectionNameForm.get('collectionName').setValue(this.currentDmoCollection.collectionName);
    this.showEditForm = true;
    this.clickedRow = null;
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.table.filter = filterValue.trim().toLowerCase();

    if (this.table.paginator) {
      this.table.paginator.firstPage();
    }
  }

  private redirectToDashboard() {
    this.collectionManager.setCollectionId('');
    this.router.navigateByUrl('/');
  }

  private loadDmos() {
    this.resetTable();
    const collectionId = this.route.snapshot.paramMap.get('id');
    return this.dmoCollectionService.getWithDmos(collectionId)
      .subscribe({
        next: (response: DmoCollectionDto) => {
          this.currentDmoCollection = response;
          this.initializeTable(this.currentDmoCollection.dmos);
          this.collectionManager.setCollectionId(collectionId); // this will trigger collections reload
        },
        error: (err) => { this.toastr.error(err); },
      });
  }

  private resetTable() {
    this.currentDmoCollection = null;
    this.shouldShowTable = false;
    this.table = null;
    this.displayedColumns = null;
    this.resultsLength = 0;
    this.clickedRow = null;
    this.hideEditCollectionNameForm();
    this.showEditForm = false;
    this.searchValue = '';
  }

  private initializeTable(dataSource: DmoShortDto[]) {
    this.displayedColumns = ['movieTitle', 'name', 'dmoStatus', 'shortComment', 'mark'];
    this.table = new MatTableDataSource(dataSource);
    this.table.paginator = this.paginator;
    this.table.sort = this.sort;
    this.resultsLength = this.currentDmoCollection.dmos.length;
    this.shouldShowTable = true;
  }

}
