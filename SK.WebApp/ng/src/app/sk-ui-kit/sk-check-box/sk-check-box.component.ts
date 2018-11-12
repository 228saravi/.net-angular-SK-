import { Component, OnInit, ViewChild, forwardRef, Input, Output, EventEmitter } from '@angular/core';
import { NG_VALUE_ACCESSOR, ControlValueAccessor, AbstractControl } from '@angular/forms';

import { DxCheckBoxComponent } from 'devextreme-angular';

@Component({
  selector: 'sk-check-box',
  templateUrl: './sk-check-box.component.html',
  styleUrls: ['./sk-check-box.component.scss'],
  providers: [{
    provide: NG_VALUE_ACCESSOR,
    useExisting: forwardRef(() => SkCheckBoxComponent),
    multi: true
  }]
})
export class SkCheckBoxComponent implements OnInit, ControlValueAccessor {
  @ViewChild("dxCheckBox") dxCheckBox: DxCheckBoxComponent;
  @Output() valueChange: EventEmitter<boolean> = new EventEmitter<boolean>();

  private _value: boolean = null;
  private _text: string = null;
  private _readOnly: boolean = null;

  private _onChangeFn: (value: boolean) => {} = null;
  private _onTouchedFn: () => {} = null;

  private propagateChange(newValue: boolean): void {
    if (this._onChangeFn) {
      this._onChangeFn(newValue);
    }

    this.valueChange.emit(newValue);
  }

  public constructor() { }

  public get value(): boolean {
    return this._value;
  }

  @Input()
  public set value(val: boolean) {
    if (val !== this._value) {
      this._value = val;
      this.propagateChange(this._value);
    }
  }

  public get text(): string {
    return this._text;
  }

  @Input()
  public set text(val: string) {
    this._text = val;
  }

  public get readOnly(): boolean {
    return this._readOnly;
  }

  @Input()
  public set readOnly(val: boolean) {
    this._readOnly = val;
  }

  public ngOnInit() {
  }

  // #region ControlValueAccessor

  writeValue(obj: any): void {
    this.value = obj;
  }
  registerOnChange(fn: any): void {
    this._onChangeFn = fn;
  }
  registerOnTouched(fn: any): void {
    this._onTouchedFn = fn;
  }
  setDisabledState?(isDisabled: boolean): void {
    this.dxCheckBox.instance.option("disabled", isDisabled);
  }

  // #endregion

}
