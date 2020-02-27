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
import { concatMap, map, takeUntil, finalize, catchError } from 'rxjs/operators';
import { throwError, Observable, Subject } from 'rxjs';
import { DmoCollectionsService } from 'src/app/shared/services/dmo-collections.service';
import { SelectionModel } from '@angular/cdk/collections';

@Component({
  selector: 'app-dmo-collection',
  templateUrl: './dmo-collection.component.html',
  styleUrls: ['./dmo-collection.component.scss']
})
export class DmoCollectionComponent implements OnInit, OnDestroy {

  // collection table variables
  currentDmoCollection: DmoCollectionDto;
  shouldCollectionShowTable = false;
  collectionTable: MatTableDataSource<DmoShortDto>;
  collectionTableColumn: string[];
  collectionLength = 0;
  selectedDmoInCollection: DmoShortDto;
  @ViewChild('collectionPaginator', { static: true }) collectionPaginator: MatPaginator;
  @ViewChild('collectionSort', { static: true }) collectionSorter: MatSort;
  @ViewChild('removeFullCollectionModal', { static: true }) removeCollectionModal: NgbActiveModal;
  @ViewChild('addDmoToCollectionModal', { static: true }) addToCollectionModal: NgbActiveModal;

  // dmos table variables
  dmosTable: MatTableDataSource<DmoShortDto>;
  allOtherDmos: DmoShortDto[];
  isAwaitingForDmos = true;
  dmosTableColumn: string[];
  dmosCount = 0;
  @ViewChild('dmosPaginator', { static: true }) dmosPaginator: MatPaginator;
  @ViewChild('dmosSort', { static: true }) dmosSorter: MatSort;
  selectedDmo = new SelectionModel<DmoShortDto>(true, []);



  editCollectionNameForm: FormGroup;
  get collectionName() { return this.editCollectionNameForm.get('collectionName'); }
  showEditForm = false;
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

    if (this.selectedDmoInCollection && this.selectedDmoInCollection === row) {
      this.selectedDmoInCollection = null;
      return;
    }
    this.selectedDmoInCollection = row;
  }

  redirectToDmo() {
    if (!this.selectedDmoInCollection) {
      return;
    }
    console.log('dmo editor is not implement yet');
  }

  removeFromCollection() {
    if (!this.selectedDmoInCollection) {
      return;
    }

    const removeFromCollection$ = this.dmoCollectionService
      .removeFromCollection(this.selectedDmoInCollection.id, this.currentDmoCollection.id);

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

  async onAddDmo() {
    const modalRef = this.modalService.open(this.addToCollectionModal);
    
    const collectionId = this.route.snapshot.paramMap.get('id');
    await this.dmoCollectionService.getExcludedDmos(collectionId).toPromise()
      .then((res) => this.allOtherDmos = res )
      .catch((err) => this.toastr.error(err));
      this.initializeDmosTable();



    this.isAwaitingForDmos = false;

    const shouldSendRemoveRequest = await modalRef.result.then(() => true, () => false);

    const sdr = this.selectedDmo.selected;
    console.log(sdr);
    console.log('close');

    this.resetDmosTable();

    if (!shouldSendRemoveRequest) {
      return;
    }

    
    // const addAndRefresh$ = this.dmoCollectionService
  }

  async onRemoveCollection() {
    const modalRef = this.modalService.open(this.removeCollectionModal);
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
    this.selectedDmoInCollection = null;
  }

  applyCollectionFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.collectionTable.filter = filterValue.trim().toLowerCase();

    if (this.collectionTable.paginator) {
      this.collectionTable.paginator.firstPage();
    }
  }


  isAllDmoSelected() {
    const numSelected = this.selectedDmo.selected.length;
    const numRows = this.dmosTable.data.length;
    return numSelected === numRows;
  }

  dmosTableToggle() {
    this.isAllDmoSelected() ?
        this.selectedDmo.clear() :
        this.dmosTable.data.forEach(row => this.selectedDmo.select(row));
  }

  // checkboxLabel(row?: DmoShortDto): string {
  //   if (!row) {
  //     return `${this.isAllDmoSelected() ? 'select' : 'deselect'} all`;
  //   }
  //   return `${this.selectedDmo.isSelected(row) ? 'deselect' : 'select'} row ${row.position + 1}`;
  // }

  private redirectToDashboard() {
    this.collectionManager.setCollectionId('');
    this.router.navigateByUrl('/');
  }

  private loadDmos() {
    this.resetCollectionTable();
    const collectionId = this.route.snapshot.paramMap.get('id');
    return this.dmoCollectionService.getWithDmos(collectionId)
      .subscribe({
        next: (response: DmoCollectionDto) => {
          this.currentDmoCollection = response;
          this.initializeCollectionTable(this.currentDmoCollection.dmos);
          this.collectionManager.setCollectionId(collectionId); // this will trigger collections reload
        },
        error: (err) => { this.toastr.error(err); },
      });
  }

  private resetCollectionTable() {
    this.currentDmoCollection = null;
    this.shouldCollectionShowTable = false;
    this.collectionTable = null;
    this.collectionTableColumn = null;
    this.collectionLength = 0;
    this.selectedDmoInCollection = null;
    this.hideEditCollectionNameForm();
    this.showEditForm = false;
  }

  private initializeCollectionTable(dataSource: DmoShortDto[]) {
    this.collectionTableColumn = ['movieTitle', 'name', 'dmoStatus', 'shortComment', 'mark'];
    this.collectionTable = new MatTableDataSource(dataSource);
    this.collectionTable.paginator = this.collectionPaginator;
    this.collectionTable.sort = this.collectionSorter;
    this.collectionLength = this.currentDmoCollection.dmos.length;
    this.shouldCollectionShowTable = true;
  }

  private resetDmosTable() {
    this.dmosTableColumn = null;
    this.dmosTable = null;
    this.dmosCount = null;
    this.isAwaitingForDmos = true;
    this.selectedDmo.clear();
  }

  private initializeDmosTable() {
    this.dmosTableColumn = ['select', 'movieTitle', 'name'];
    this.dmosTable = new MatTableDataSource(this.allOtherDmos);
    this.dmosTable.paginator = this.dmosPaginator;
    this.dmosTable.sort = this.dmosSorter;
    this.dmosCount = this.allOtherDmos.length;
  }

}
