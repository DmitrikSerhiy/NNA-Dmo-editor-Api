import { DmoCollectionsService } from './dmo-collections.service';
import { DmoListDto } from './dmoList.dto';

import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-dmo-collections',
  templateUrl: './dmo-collections.component.html',
  styleUrls: ['./dmo-collections.component.scss']
})
export class DmoCollectionsComponent implements OnInit {

  dmoLists: DmoListDto[];
  constructor(private dmoCollectionsService: DmoCollectionsService) { }


  ngOnInit() {
    this.dmoCollectionsService.getAll().subscribe((response: DmoListDto[]) => {
      this.dmoLists = response;
    });
  }

}
