import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'sk-formatted-text',
  templateUrl: './sk-formatted-text.component.html',
  styleUrls: ['./sk-formatted-text.component.scss']
})
export class SkFormattedTextComponent implements OnInit {
  private _html: string;

  public constructor() { }

  public get html(): string {
    return this._html;
  }

  @Input()
  public set html(val: string) {
    this._html = val;
  }

  public ngOnInit() {
  }

}
