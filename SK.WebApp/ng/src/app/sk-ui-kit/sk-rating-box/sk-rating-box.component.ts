import { Component, OnInit, Output, EventEmitter, Input, forwardRef } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

@Component({
  selector: 'sk-rating-box',
  templateUrl: './sk-rating-box.component.html',
  styleUrls: ['./sk-rating-box.component.scss'],
  providers: [{
    provide: NG_VALUE_ACCESSOR,
    useExisting: forwardRef(() => SkRatingBoxComponent),
    multi: true
  }]
})
export class SkRatingBoxComponent implements OnInit, ControlValueAccessor {

  @Output() valueChange: EventEmitter<number> = new EventEmitter<number>();

  private _value: number = null;
  private _errorMessage: string = null;

  private _onChangeFn: (value: number) => {} = null;
  private _onTouchedFn: () => {} = null;

  private propagateChange(newValue: number): void {
    if (this._onChangeFn) {
      this._onChangeFn(newValue);
    }

    this.valueChange.emit(newValue);
  }

  constructor() { }

  public get errorMessage(): string {
    return this._errorMessage;
  }

  @Input()
  public set errorMessage(val: string) {
    if (val !== this._errorMessage) {
      this._errorMessage = val;
      this.propagateChange(this._value);
    }
  }

  public get value(): number {
    return this._value;
  }

  @Input()
  public set value(val: number) {
    if (val !== this._value) {
      this._value = val;
      this.propagateChange(this._value);
    }
  }

  public ngOnInit(): void {
  }

  public updateValue(value: number): void {
    this.value = value;
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
    //this.dxTextBox.instance.option("disabled", isDisabled);
  }

  // #endregion

}
