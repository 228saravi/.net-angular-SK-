import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'sk-rating',
  templateUrl: './sk-rating.component.html',
  styleUrls: ['./sk-rating.component.scss']
})
export class SkRatingComponent implements OnInit {
  @Input() value: number;

  public constructor() { }

  public ngOnInit(): void {}

}
