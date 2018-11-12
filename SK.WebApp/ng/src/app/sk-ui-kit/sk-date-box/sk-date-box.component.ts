import { Component, OnInit, forwardRef, ViewChild, Output, EventEmitter, Input, ViewEncapsulation, Directive, OnChanges } from '@angular/core';
import { NG_VALUE_ACCESSOR, ControlValueAccessor, Validator, NG_VALIDATORS, FormControl } from '@angular/forms';
import { DxDateBoxComponent } from 'devextreme-angular';


function normalizeToMinutes(date: Date): Date {
  var res = new Date(date.getFullYear(), date.getMonth(), date.getDate(), date.getHours(), date.getMinutes());
  return res;
}

@Directive({
  selector: '[min][formControlName],[min][formControl],[min][ngModel]',
  providers: [{ provide: NG_VALIDATORS, useExisting: MinDateDirective, multi: true }]
})
export class MinDateDirective implements Validator {
  @Input()
  min: Date;

  validate(c: FormControl): { [key: string]: any } {
    let v = c.value as Date;

    if (v && this.min) {
      var res = (v.valueOf() < normalizeToMinutes(this.min).valueOf()) ? { "min": true } : null;
      return res;
    }

    return null;
  }
}

@Directive({
  selector: '[max][formControlName],[max][formControl],[max][ngModel]',
  providers: [{ provide: NG_VALIDATORS, useExisting: MaxDateDirective, multi: true }]
})
export class MaxDateDirective implements Validator {
  @Input()
  max: Date;

  validate(c: FormControl): { [key: string]: any } {
    let v = c.value as Date;

    if (v && this.max) {
      var res = v && this.max && (v.valueOf() > normalizeToMinutes(this.max).valueOf()) ? { "max": true } : null;
      return res;
    }

    return null;
  }
}

type Type = "date" | "datetime";

@Component({
  selector: 'sk-date-box',
  templateUrl: './sk-date-box.component.html',
  styleUrls: ['./sk-date-box.component.scss'],
  providers: [{
    provide: NG_VALUE_ACCESSOR,
    useExisting: forwardRef(() => SkDateBoxComponent),
    multi: true
  }],
  encapsulation: ViewEncapsulation.None,
})
export class SkDateBoxComponent implements OnInit, ControlValueAccessor {
  @ViewChild("dxDateBox") dxDateBox: DxDateBoxComponent;
  @Output() valueChange: EventEmitter<Date> = new EventEmitter<Date>();

  private _value: Date = null;
  private _min: Date = null;
  private _max: Date = null;
  private _type: Type = "date";

  private _onChangeFn: (value: Date) => {} = null;
  private _onTouchedFn: () => {} = null;

  private adjustValueToMinAndMax(): void {

    if (this._value && this._min) {
      var normalizedMin = normalizeToMinutes(this._min);
      if (this._value.valueOf() < normalizedMin.valueOf()) {
        this._value = normalizedMin;
      }
    }

    if (this._value && this._max) {
      var normalizedMax = normalizeToMinutes(this._max);
      if (this._value.valueOf() > normalizedMax.valueOf()) {
        this._value = normalizedMax;
      }
    }

  }

  private propagateChange(newValue: Date): void {
    if (this._onChangeFn) {
      this._onChangeFn(newValue);
    }

    this.valueChange.emit(newValue);
  }

  public constructor() {

  }

  public get value(): Date {
    return this._value;
  }

  @Input()
  public set value(val: Date) {
    this._value = val;
    this.adjustValueToMinAndMax();
    this.propagateChange(this._value);
  }

  public get min(): Date {
    return this._min;
  }

  @Input()
  public set min(val: Date) {
    var normalized = val ? normalizeToMinutes(val) : val;

    if (normalized !== this._min) {
      this._min = normalized;
    }

    this.adjustValueToMinAndMax();
  }

  public get max(): Date {
    return this._max;
  }

  @Input()
  public set max(val: Date) {
    var normalized = val ? normalizeToMinutes(val) : val;

    if (normalized !== this._max) {
      this._max = normalized;
    }

    this.adjustValueToMinAndMax();
  }

  public get type(): Type {
    return this._type;
  }

  @Input()
  public set type(val: Type) {
    if (val !== this._type) {
      this._type = val;
    }
  }

  public ngOnInit(): void {

  }

  // #region ControlValueAccessor

  writeValue(obj: Date): void {
    this.value = obj;
  }
  registerOnChange(fn: any): void {
    this._onChangeFn = fn;
  }
  registerOnTouched(fn: any): void {
    this._onTouchedFn = fn;
  }
  setDisabledState?(isDisabled: boolean): void {
    this.dxDateBox.instance.option("disabled", isDisabled);
  }

  // #endregion

}
