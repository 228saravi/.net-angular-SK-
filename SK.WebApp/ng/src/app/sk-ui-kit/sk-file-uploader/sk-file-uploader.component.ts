import { Component, OnInit, Output, EventEmitter, ViewChild, Input, ViewRef, AfterViewInit } from '@angular/core';
import { DxFileUploaderComponent } from 'devextreme-angular';
import notify from 'devextreme/ui/notify';

@Component({
  selector: 'sk-file-uploader',
  templateUrl: './sk-file-uploader.component.html',
  styleUrls: ['./sk-file-uploader.component.scss']
})
export class SkFileUploaderComponent implements OnInit, AfterViewInit {

  @Input() uploadUrl: string = null;
  @Input() disabled: boolean = false;
  @Output() uploaded: EventEmitter<any> = new EventEmitter<any>();

  @ViewChild("uploader") uploader: DxFileUploaderComponent;

  public constructor() {

  }

  public ngOnInit(): void {

  }

  public ngAfterViewInit(): void {

  }

  public onClick(): void {
    var element = this.uploader.instance.element();
    var button = element.querySelector(".dx-fileuploader-button");

    var event = new MouseEvent('click', {
      'view': window,
      'bubbles': true,
      'cancelable': true
    });

    button.dispatchEvent(event);
  }

  public onUploadError(error): void {
    var message = "Ошибка!";

    var resAsString = error.request.response;
    var res = JSON.parse(resAsString);

    if (res.type == "FILE_IS_TOO_BIG") {
      message = res.message;
    }

    notify({ message: message, position: { my: 'bottom left', at: 'bottom left', of: window, offset: '9 -9' } }, "error");
  }

  public onUploaded(): void {
    this.uploaded.emit(null);
    notify({ message: "Фото обновлено!", position: { my: 'bottom left', at: 'bottom left', of: window, offset: '9 -9' } }, "success");
  }

}
