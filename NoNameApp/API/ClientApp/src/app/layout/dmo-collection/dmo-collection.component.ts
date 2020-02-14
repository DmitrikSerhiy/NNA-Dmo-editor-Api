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

  @ViewChild(MatPaginator, {static: true}) paginator: MatPaginator;
  @ViewChild(MatSort, {static: true}) sort: MatSort;

  constructor(
    private dmoCollectionService: DmoCollectionService,
    private route: ActivatedRoute,
    private toastr: Toastr) { }

  ngOnInit() {
    const dmoCollection$ = this.dmoCollectionService.getWithDmos(this.route.snapshot.paramMap.get('id'));
    dmoCollection$.subscribe(
      (response: DmoCollectionDto) => {
        this.currentDmoCollection = response;
        this.displayedColumns = ['movieTitle', 'name', 'dmoStatus', 'shortComment', 'mark'];
        this.table = new MatTableDataSource(this.currentDmoCollection.dmos);
        this.table.paginator = this.paginator;
        this.table.sort = this.sort;
        this.resultsLength = this.currentDmoCollection.dmos.length;
        this.shouldShowTable = true; },
      (error) => this.toastr.error(error));
  }


  onRowSelect(row: DmoShortDto) {
    this.selectedDmo = row;
    console.log(row);
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.table.filter = filterValue.trim().toLowerCase();

    if (this.table.paginator) {
      this.table.paginator.firstPage();
    }
  }

}
