import { Component, OnInit, ViewChild, forwardRef, Input, Output, EventEmitter, ViewEncapsulation } from '@angular/core';
import { NG_VALUE_ACCESSOR, ControlValueAccessor } from '@angular/forms';

import { DxSelectBoxComponent } from 'devextreme-angular';

import DataSource from "devextreme/data/data_source";

@Component({
  selector: 'sk-select-box',
  templateUrl: './sk-select-box.component.html',
  styleUrls: ['./sk-select-box.component.scss'],
  providers: [{
    provide: NG_VALUE_ACCESSOR,
    useExisting: forwardRef(() => SkSelectBoxComponent),
    multi: true
  }],
  encapsulation: ViewEncapsulation.None,
})
export class SkSelectBoxComponent implements OnInit, ControlValueAccessor {
  @ViewChild("dxSelectBox") dxSelectBox: DxSelectBoxComponent;
  @Output() valueChange: EventEmitter<any> = new EventEmitter<any>();

  private _value: any = null;
  private _items: any[] = [];
  private _dataSource: DataSource = null;
  private _valueExpr: string = null;
  private _displayExpr: string = null;
  private _placeholder: string = null;
  private _showDropDownButton: boolean = true;
  private _showClearButton: boolean = false;
  private _groupingFieldName: string = null;
  private _sortingGroupsFieldName: string = null;

  private _onChangeFn: (value: any) => {} = null;
  private _onTouchedFn: () => {} = null;

  private propagateChange(newValue: any): void {
    if (this._onChangeFn) {
      this._onChangeFn(newValue);
    }

    this.valueChange.emit(newValue);
  }

  private updateDataSource(): void {
    this._dataSource = new DataSource({
      store: this.items,
      group: this.groupingFieldName,
      postProcess: (data) => {
        if (this.groupingFieldName && this._sortingGroupsFieldName) {
          data = data.sort((a, b) => {
            var aa = a.items.map(i => i[this._sortingGroupsFieldName]).reduce((min, x) => Math.min(min, x), Infinity);
            var bb = b.items.map(i => i[this._sortingGroupsFieldName]).reduce((min, x) => Math.min(min, x), Infinity);

            return aa - bb;
          })
        }

        return data;
      }
    });
  }

  public constructor() { }

  public get dataSource(): DataSource {
    return this._dataSource;
  }

  public get value(): any {
    return this._value;
  }

  @Input()
  public set value(val: any) {
    if (val !== this._value) {
      this._value = val;
      this.propagateChange(this._value);
    }
  }

  public get items(): any[] {
    return this._items;
  }

  @Input()
  public set items(val: any[]) {
    this._items = val;
    this.updateDataSource();
  }

  public get valueExpr(): string {
    return this._valueExpr;
  }

  @Input()
  public set valueExpr(val: string) {
    this._valueExpr = val;
  }

  public get displayExpr(): string {
    return this._displayExpr;
  }

  @Input()
  public set displayExpr(val: string) {
    this._displayExpr = val;
  }

  public get placeholder(): string {
    return this._placeholder;
  }

  @Input()
  public set placeholder(val: string) {
    this._placeholder = val;
  }

  public get showDropDownButton(): boolean {
    return this._showDropDownButton;
  }

  @Input()
  public set showDropDownButton(val: boolean) {
    this._showDropDownButton = val;
  }

  public get showClearButton(): boolean {
    return this._showClearButton;
  }

  @Input()
  public set showClearButton(val: boolean) {
    this._showClearButton = val;
  }

  public get groupingFieldName(): string {
    return this._groupingFieldName;
  }

  @Input()
  public set groupingFieldName(val: string) {
    this._groupingFieldName = val;
    this.updateDataSource();
  }

  public get sortingGroupsFieldName(): string {
    return this._sortingGroupsFieldName;
  }

  @Input()
  public set sortingGroupsFieldName(val: string) {
    this._sortingGroupsFieldName = val;
    this.updateDataSource();
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
    this.dxSelectBox.instance.option("disabled", isDisabled);
  }

  // #endregion

}
