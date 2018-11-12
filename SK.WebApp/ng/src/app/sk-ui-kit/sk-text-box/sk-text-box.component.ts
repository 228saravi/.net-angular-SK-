import { Component, OnInit, ViewChild, forwardRef, Input, Output, EventEmitter } from '@angular/core';
import { NG_VALUE_ACCESSOR, ControlValueAccessor, AbstractControl } from '@angular/forms';

import { DxTextBoxComponent } from 'devextreme-angular';

@Component({
  selector: 'sk-text-box',
  templateUrl: './sk-text-box.component.html',
  styleUrls: ['./sk-text-box.component.scss'],
  providers: [{
    provide: NG_VALUE_ACCESSOR,
    useExisting: forwardRef(() => SkTextBoxComponent),
    multi: true
  }]
})
export class SkTextBoxComponent implements OnInit, ControlValueAccessor {
  @ViewChild("dxTextBox") dxTextBox: DxTextBoxComponent;

  @Output() valueChange: EventEmitter<string> = new EventEmitter<string>();

  private _value: string = null;
  private _placeholder: string;
  private _mode: "text" | "password" | "number" = null;
  private _showClearButton: boolean;
  private _errorMessage: string = null;

  private _onChangeFn: (value: string) => {} = null;
  private _onTouchedFn: () => {} = null;

  private propagateChange(newValue: string): void {
    if (this._onChangeFn) {
      this._onChangeFn(newValue);
    }

    this.valueChange.emit(newValue);
  }

  public constructor() { }

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

  public get mode(): "text" | "password" | "number" {
    return this._mode;
  }

  @Input()
  public set mode(val: "text" | "password" | "number") {
    if (val !== this._mode) {
      this._mode = val;
    }
  }

  public get showClearButton(): boolean {
    return this._showClearButton;
  }

  @Input()
  public set showClearButton(val: boolean) {
    if (val !== this._showClearButton) {
      this._showClearButton = val;
    }
  }

  public get errorMessage(): string {
    return this._errorMessage;
  }

  @Input()
  public set errorMessage(val: string) {
    if (val !== this._errorMessage) {
      this._errorMessage = val;
    }
  }

  public ngOnInit() { }

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
    this.dxTextBox.instance.option("disabled", isDisabled);
  }

  // #endregion

}
