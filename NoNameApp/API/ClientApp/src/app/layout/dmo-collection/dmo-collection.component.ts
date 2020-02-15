import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Toastr } from './../../shared/services/toastr.service';
import { DmoCollectionDto, DmoShortDto } from './../models';
import { DmoCollectionService } from './dmo-collection.service';
import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { switchMap } from 'rxjs/operators';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { MatPaginator } from '@angular/material/paginator';

@Component({
  selector: 'app-dmo-collection',
  templateUrl: './dmo-collection.component.html',
  styleUrls: ['./dmo-collection.component.scss']
})
export class DmoCollectionComponent implements OnInit {

  currentDmoCollection: DmoCollectionDto;
  shouldShowTable = false;
  table: MatTableDataSource<DmoShortDto>;
  displayedColumns: string[];
  resultsLength = 0;
  selectedDmo: DmoShortDto;
  clickedRow: any;

  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;

  editCollectionNameForm: FormGroup;
  get collectionName() { return this.editCollectionNameForm.get('collectionName'); }
  showEditForm = false;
  searchValue: any;

  constructor(
    private dmoCollectionService: DmoCollectionService,
    private route: ActivatedRoute,
    private toastr: Toastr,
    private router: Router) { }

  ngOnInit() {
    this.editCollectionNameForm = new FormGroup({
      'collectionName': new FormControl('', [Validators.required, Validators.maxLength(20)])
    });

    let dmoObserver = this.loadDmos();

    const params$ = this.route.params;
    params$.subscribe(p => {
      if (!p['id']) {
        return;
      }
      dmoObserver = this.loadDmos();
    });
  }


  onRowSelect(row) {
    this.clickedRow = row;
    //todo: to some shit with it
  }

  onEditCollectionName() {
    console.log('submit');
  }

  hideEditCollectionNameForm() {
    this.editCollectionNameForm.reset();
    this.showEditForm = false;
  }

  showEditCollectionNameForm() {
    this.editCollectionNameForm.get('collectionName').setValue(this.currentDmoCollection.collectionName);
    this.showEditForm = true;
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.table.filter = filterValue.trim().toLowerCase();

    if (this.table.paginator) {
      this.table.paginator.firstPage();
    }
  }

  private loadDmos() {
    this.resetTable();
    return this.dmoCollectionService.getWithDmos(this.route.snapshot.paramMap.get('id'))
      .subscribe((response: DmoCollectionDto) => {
        this.currentDmoCollection = response;
        this.initializeTable(this.currentDmoCollection.dmos);
      },
        (error) => this.toastr.error(error));
  }

  private resetTable() {
    this.currentDmoCollection = null;
    this.shouldShowTable = false;
    this.table = null;
    this.displayedColumns = null;
    this.resultsLength = 0;
    this.selectedDmo = null;
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
