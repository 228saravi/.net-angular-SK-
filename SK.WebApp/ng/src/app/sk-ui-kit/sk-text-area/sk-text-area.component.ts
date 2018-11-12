import { Component, OnInit, ViewChild, forwardRef, Input, Output, EventEmitter } from '@angular/core';
import { NG_VALUE_ACCESSOR, ControlValueAccessor, AbstractControl } from '@angular/forms';

import { DxTextAreaComponent } from 'devextreme-angular';

@Component({
  selector: 'sk-text-area',
  templateUrl: './sk-text-area.component.html',
  styleUrls: ['./sk-text-area.component.scss'],
  providers: [{
    provide: NG_VALUE_ACCESSOR,
    useExisting: forwardRef(() => SkTextAreaComponent),
    multi: true
  }]
})
export class SkTextAreaComponent implements OnInit {

  @ViewChild("dxTextArea") dxTextArea: DxTextAreaComponent;

  @Output() valueChange: EventEmitter<string> = new EventEmitter<string>();

  private _value: string = null;
  private _placeholder: string;

  private _onChangeFn: (value: string) => {} = null;
  private _onTouchedFn: () => {} = null;

  private propagateChange(newValue: string): void {
    if (this._onChangeFn) {
      this._onChangeFn(newValue);
    }

    this.valueChange.emit(newValue);
  }

  public constructor() {
  }

  public get value(): string {
    return this._value;
  }

  @Input()
  public set value(val: string) {
    if (val !== this._value) {
      this._value = val;
      this.propagateChange(this._value);
    }
  }

  public get placeholder(): string {
    return this._placeholder;
  }

  @Input()
  public set placeholder(val: string) {
    this._placeholder = val;
  }

  public ngOnInit(): void {
  }

  public onEnter($event): void {
    console.log("enter!");
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
    this.dxTextArea.instance.option("disabled", isDisabled);
  }

  // #endregion

}
