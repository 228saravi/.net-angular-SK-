import { Component, OnInit, ViewChild, ViewEncapsulation, Input } from '@angular/core';
import { DxPopupComponent } from 'devextreme-angular';

@Component({
  selector: 'sk-publish-before-connect-popup',
  templateUrl: './sk-publish-before-connect-popup.component.html',
  styleUrls: ['./sk-publish-before-connect-popup.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class SkPublishBeforeConnectPopupComponent implements OnInit {
  @ViewChild("popup")
  public popup: DxPopupComponent;

  @Input()
  public type: "company" | "expert" = "company";

  public constructor() { }

  public ngOnInit() {
  }

  public async show(): Promise<void> {
    await (this.popup.instance.show() as Promise<void>);
  }

  public async hide(): Promise<void> {
    await (this.popup.instance.hide() as Promise<void>);
  }

}
