import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'sk-loader',
  templateUrl: './sk-loader.component.html',
  styleUrls: ['./sk-loader.component.scss']
})
export class SkLoaderComponent implements OnInit {

  @Input()
  public element: HTMLElement = null;

  constructor() { }

  ngOnInit() {
  }

}
