import { Component, OnInit, Input, ViewEncapsulation } from '@angular/core';

@Component({
  selector: 'sk-button',
  templateUrl: './sk-button.component.html',
  styleUrls: ['./sk-button.component.scss'],
  //encapsulation: ViewEncapsulation.None
})
export class SkButtonComponent implements OnInit {
  @Input() text: string;
  @Input() useSubmitBehavior: boolean = false;
  @Input() disabled: boolean = false;

  constructor() { }

  ngOnInit() {
  }

}
