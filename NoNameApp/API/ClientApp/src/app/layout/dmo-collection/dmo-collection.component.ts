import { AddDmosPopupComponent } from './add-dmos-popup/add-dmos-popup.component';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { CollectionsManagerService } from './../../shared/services/collections-manager.service';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Toastr } from './../../shared/services/toastr.service';
import { DmoCollectionDto, DmoShortDto, DmoCollectionShortDto, AddDmosToCollectionDto,
   DmosIdDto, ShortDmoCollectionDto, DmoShorterDto } from './../models';
import { Component, OnInit, ViewChild, OnDestroy, ElementRef } from '@angular/core';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { MatPaginator } from '@angular/material/paginator';
import { concatMap, map, takeUntil, finalize } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { DmoCollectionsService } from 'src/app/shared/services/dmo-collections.service';
import { MatDialog } from '@angular/material/dialog';

@Component({
  selector: 'app-dmo-collection',
  templateUrl: './dmo-collection.component.html',
  styleUrls: ['./dmo-collection.component.scss']
})
export class DmoCollectionComponent implements OnInit, OnDestroy {
  currentDmoCollection: DmoCollectionDto;
  shouldCollectionShowTable = false;
  awaitingForDmos = false;
  collectionTable: MatTableDataSource<DmoShortDto>;
  collectionTableColumn: string[];
  collectionLength = 0;
  selectedDmoInCollection: DmoShortDto;
  @ViewChild('collectionPaginator', { static: true }) collectionPaginator: MatPaginator;
  @ViewChild('collectionSort', { static: true }) collectionSorter: MatSort;
  @ViewChild('removeFullCollectionModal', { static: true }) removeCollectionModal: NgbActiveModal;
  @ViewChild('addDmoToCollectionModal', { static: true }) addToCollectionModal: NgbActiveModal;
  @ViewChild('editCollectionNameField', {static: true}) collectionNameField: ElementRef;

  editCollectionNameForm: FormGroup;
  get collectionName() { return this.editCollectionNameForm.get('collectionName'); }
  showEditForm = false;
  private unsubscribe$: Subject<void> = new Subject();

  constructor(
    private dmoCollectionService: DmoCollectionsService,
    private route: ActivatedRoute,
    private modalService: NgbModal,
    public matModule: MatDialog,
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
    this.awaitingForDmos = true;
    const collectionWithDmo = new ShortDmoCollectionDto();
    collectionWithDmo.collectionName = this.currentDmoCollection.collectionName;

    const collectionId = this.route.snapshot.paramMap.get('id');
    await this.dmoCollectionService.getExcludedDmos(collectionId).toPromise()
      .then((dmos: DmoShortDto[]) => collectionWithDmo.dmos = dmos.map(d => new DmoShorterDto(d.id, d.movieTitle, d.name)))
      .catch((err) => this.toastr.error(err));

    this.awaitingForDmos = false;
    const dialogRef = this.matModule.open(AddDmosPopupComponent, {
      data: collectionWithDmo,
      minWidth: '430px'
    });

    dialogRef.afterClosed()
      .subscribe({
        next: (selectDmos: DmoShortDto[]) => {
          if (!selectDmos) {
            return;
          }

          const dto = new AddDmosToCollectionDto();
          dto.collectionId = collectionId;
          dto.dmos = selectDmos.map(d => new DmosIdDto(d.id));

          const addToCollection$ = this.dmoCollectionService.addDmosToCollection(dto);
          addToCollection$.pipe(takeUntil(this.unsubscribe$));
          addToCollection$.subscribe({
            next: () => { this.loadDmos(); },
            error: (err) => { this.toastr.error(err); }
          });
        }
      });


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

    setTimeout(() => {
      this.collectionNameField.nativeElement.focus();
    }, 100);
  }

  applyCollectionFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.collectionTable.filter = filterValue.trim().toLowerCase();

    if (this.collectionTable.paginator) {
      this.collectionTable.paginator.firstPage();
    }
  }

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
        error: (err) => { this.toastr.error(err); }
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
}
