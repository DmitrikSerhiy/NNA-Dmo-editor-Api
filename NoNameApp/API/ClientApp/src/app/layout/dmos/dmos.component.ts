import { Subject } from 'rxjs';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { Toastr } from './../../shared/services/toastr.service';
import { DmoShortDto } from './../models';
import { DmosService } from './dmos.service';
import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { MatSort } from '@angular/material/sort';

@Component({
  selector: 'app-dmos',
  templateUrl: './dmos.component.html',
  styleUrls: ['./dmos.component.scss']
})
export class DmosComponent implements OnInit, OnDestroy {

  allDmos: DmoShortDto[];
  shouldShowDmosTable = false;
  dmosTable: MatTableDataSource<DmoShortDto>;
  dmosTableColumns: string[];
  dmosCount = 0;
  @ViewChild('dmosPaginator', { static: true }) dmosPaginator: MatPaginator;
  @ViewChild('dmosSorter', { static: true }) dmosSorter: MatSort;
  private unsubscribe$: Subject<void> = new Subject();
  selectedDmo: DmoShortDto;


  constructor(
    private dmosService: DmosService,
    private toastr: Toastr) { }

  ngOnInit() {

    this.loadDmos();
  }

  ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  private loadDmos() {
    this.resetDmosTable();
    this.dmosService.getAlldmos().subscribe({
      next: (result: DmoShortDto[]) => {
        this.allDmos = result;
        this.initializeDmosTable(this.allDmos);
    },
      error: (err) => { this.toastr.error(err); }
    });

  }



  onRowSelect(row: DmoShortDto) {
    if (this.selectedDmo && this.selectedDmo === row) {
      this.selectedDmo = null;
      return;
    }
    this.selectedDmo = row;
  }

  applyDmosFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dmosTable.filter = filterValue.trim().toLowerCase();

    if (this.dmosTable.paginator) {
      this.dmosTable.paginator.firstPage();
    }
  }

  private resetDmosTable() {
    this.shouldShowDmosTable = false;
    this.dmosTable = null;
    this.dmosTableColumns = null;
    this.dmosCount = 0;
    this.selectedDmo = null;
  }

  private initializeDmosTable(dmos: DmoShortDto[]) {
    this.dmosTable = new MatTableDataSource(dmos);
    this.dmosTable.paginator = this.dmosPaginator;
    this.dmosTable.sort = this.dmosSorter;
    this.dmosTableColumns = ['movieTitle', 'name', 'dmoStatus', 'shortComment', 'mark'];
    this.dmosCount = dmos.length;
    this.shouldShowDmosTable = true;
  }
}
