import { Component, OnInit, ContentChildren, QueryList, Input, Output, EventEmitter } from '@angular/core';


@Component({
  selector: 'sk-dropdown-button-item',
  template: '',
})
export class SkDropdownButtonItemComponent {
  @Input()
  public text: string = null;

  @Output()
  public clicked: EventEmitter<void> = new EventEmitter<void>();
}


@Component({
  selector: 'sk-dropdown-button',
  templateUrl: './sk-dropdown-button.component.html',
  styleUrls: ['./sk-dropdown-button.component.scss']
})
export class SkDropdownButtonComponent implements OnInit {

  @ContentChildren(SkDropdownButtonItemComponent, { descendants: true }) items: QueryList<SkDropdownButtonItemComponent>;

  constructor() { }

  ngOnInit() {
  }

  public onItemClick($event): void {
    var text = $event.itemData.text;

    var item = this.items.find(i => i.text == text);

    if (item) {
      item.clicked.emit(null);
    }
  }
}
