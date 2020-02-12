import { Toastr } from './../../shared/services/toastr.service';
import { DmoCollectionDto } from './../models';
import { DmoCollectionService } from './dmo-collection.service';
import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { switchMap } from 'rxjs/operators';

@Component({
  selector: 'app-dmo-collection',
  templateUrl: './dmo-collection.component.html',
  styleUrls: ['./dmo-collection.component.scss']
})
export class DmoCollectionComponent implements OnInit {

  dmoCollection: DmoCollectionDto;

  constructor(
    private dmoCollectionService: DmoCollectionService,
    private route: ActivatedRoute,
    private toastr: Toastr) { }

  ngOnInit() {
    const dmoCollection$ = this.dmoCollectionService.getWithDmos(this.route.snapshot.paramMap.get('id'));
    dmoCollection$.subscribe(
      (response: DmoCollectionDto) => { console.log(response); this.dmoCollection = response; },
      (error) => this.toastr.error(error));

  }

}
