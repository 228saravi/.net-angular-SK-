import { Component, OnInit, Input, ElementRef } from '@angular/core';
import { SpecialityIds } from './../../_services/sk-specialityIds';

@Component({
  selector: 'sk-speciality-icon',
  templateUrl: './sk-speciality-icon.component.html',
  styleUrls: ['./sk-speciality-icon.component.scss']
})
export class SkSpecialityIconComponent implements OnInit {
  private _elementRef: ElementRef;

  public SpecialityIds = SpecialityIds;

  @Input() public specialityId: string;
  @Input() public height: number = 20;

  public constructor(elementRef: ElementRef) {
    this._elementRef = elementRef;
  }

  public ngOnInit() {
  }

}
