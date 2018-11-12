import { Component, OnInit, Input, Output, EventEmitter, ContentChildren, QueryList, ViewChild, AfterViewInit } from '@angular/core';
import { DxTabsComponent } from 'devextreme-angular';

@Component({
  selector: 'sk-tabs-item',
  template: '',
})
export class SkTabsItemComponent {
  @Input()
  public text: string = null;

  @Input()
  public badge: string = null;

  @Input()
  public selectedByDefault: boolean = false;

  @Output()
  public clicked: EventEmitter<void> = new EventEmitter<void>();
}

@Component({
  selector: 'sk-tabs',
  templateUrl: './sk-tabs.component.html',
  styleUrls: ['./sk-tabs.component.scss']
})
export class SkTabsComponent implements OnInit, AfterViewInit {

  @ViewChild("tabs")
  public tabs: DxTabsComponent;

  @ContentChildren(SkTabsItemComponent, { descendants: true })
  public items: QueryList<SkTabsItemComponent>;

  public constructor() {

  }

  public ngOnInit(): void {
  }

  public ngAfterViewInit(): void {
    var index = this.items.toArray().findIndex(i => i.selectedByDefault);

    if (index >= 0) {
      setTimeout(() => {
        this.tabs.instance.option("selectedIndex", index < this.tabs.items.length ? index : 0);
      });
    }
  }

  public onSelectionChanged($event): void {
    var text = $event.addedItems[0].text;

    var item = this.items.find(i => i.text == text);

    if (item) {
      item.clicked.emit(null);
    }
  }

  public selectTab(tabIndex: number): void {
    this.tabs.instance.option("selectedIndex", tabIndex < this.tabs.items.length ? tabIndex : 0);
  }

}
