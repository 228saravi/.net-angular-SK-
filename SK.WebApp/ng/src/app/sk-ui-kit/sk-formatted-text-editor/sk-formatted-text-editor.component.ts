import { Component, OnInit, ViewChild, forwardRef, Input, Output, EventEmitter } from '@angular/core';
import { NG_VALUE_ACCESSOR, ControlValueAccessor, AbstractControl } from '@angular/forms';
import { CKEditorComponent } from 'ng2-ckeditor';

@Component({
  selector: 'sk-formatted-text-editor',
  templateUrl: './sk-formatted-text-editor.component.html',
  styleUrls: ['./sk-formatted-text-editor.component.scss'],
  providers: [{
    provide: NG_VALUE_ACCESSOR,
    useExisting: forwardRef(() => SkFormattedTextEditorComponent),
    multi: true
  }]
})
export class SkFormattedTextEditorComponent implements OnInit, ControlValueAccessor {
  @ViewChild("editor") editor: CKEditorComponent;

  private _value: string = null;
  private _valueChangeEventEmitter: EventEmitter<string> = new EventEmitter<string>();

  private _options: any = {
    // Define the toolbar: http://docs.ckeditor.com/ckeditor4/docs/#!/guide/dev_toolbar
    // The standard preset from CDN which we used as a base provides more features than we need.
    // Also by default it comes with a 2-line toolbar. Here we put all buttons in a single row.
    language: "ru",
    //toolbar: [
    //  { name: 'clipboard', items: ['Undo', 'Redo'] },
    //  { name: 'styles', items: ['Styles', 'Format'] },
    //  { name: 'basicstyles', items: ['Bold', 'Italic', 'Strike', '-', 'RemoveFormat'] },
    //  { name: 'paragraph', items: ['NumberedList', 'BulletedList', '-', 'Outdent', 'Indent', '-', 'Blockquote'] },
    //  { name: 'links', items: ['Link', 'Unlink', ] },
    //],
    contentsCss: ['https://cdn.ckeditor.com/4.8.0/standard-all/contents.css'],
  };

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
      //console.log(this.editor.config);
      this._value = val;
      this.propagateChange(this._value);
    }
  }

  @Output()
  public get valueChange(): EventEmitter<string> {
    return this._valueChangeEventEmitter;
  }

  public get options(): any {
    return this._options;
  }

  public ngOnInit() {
  }

  // #region ControlValueAccessor

  public writeValue(obj: any): void {
    this.value = obj;
  }

  public registerOnChange(fn: any): void {
    this._onChangeFn = fn;
  }

  public registerOnTouched(fn: any): void {
    this._onTouchedFn = fn;
  }

  public setDisabledState?(isDisabled: boolean): void { }

  // #endregion

}
