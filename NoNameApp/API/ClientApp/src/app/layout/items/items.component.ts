import { ItemsService } from './items.service';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-items',
  templateUrl: './items.component.html',
  styleUrls: ['./items.component.scss']
})
export class ItemsComponent implements OnInit {

  constructor(private itemsService: ItemsService) { }

  ngOnInit() {
    this.itemsService.post('secure LOL from secure client').subscribe((response: string) => {
      console.log(response);
    });
  }

}
