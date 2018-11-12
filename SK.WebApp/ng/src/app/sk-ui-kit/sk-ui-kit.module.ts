import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import {
  DxTextBoxModule,
  DxTextAreaModule,
  DxToastComponent,
  DxButtonModule,
  DxPopoverModule,
  DxNumberBoxModule,
  DxFileUploaderModule,
  DxSelectBoxModule,
  DxCheckBoxModule,
  DxDateBoxModule,
  DxPopupModule,
  DxScrollViewModule,
  DxContextMenuModule,
  DxTabsModule,
  DxLoadPanelModule,
} from 'devextreme-angular';

import { CKEditorModule } from 'ng2-ckeditor';

import { SkTextBoxComponent } from './sk-text-box/sk-text-box.component';
import { SkSelectBoxComponent } from './sk-select-box/sk-select-box.component';
import { SkButtonComponent } from './sk-button/sk-button.component';

import { SkFormattedTextEditorComponent } from './sk-formatted-text-editor/sk-formatted-text-editor.component';
import { SkCheckBoxComponent } from './sk-check-box/sk-check-box.component';
import { SkFileUploaderComponent } from './sk-file-uploader/sk-file-uploader.component';
import { SkDateBoxComponent, MinDateDirective, MaxDateDirective } from './sk-date-box/sk-date-box.component';
import { SkRatingBoxComponent } from './sk-rating-box/sk-rating-box.component';
import { SkDropdownButtonComponent, SkDropdownButtonItemComponent } from './sk-dropdown-button/sk-dropdown-button.component';
import { SkTabsComponent, SkTabsItemComponent } from './sk-tabs/sk-tabs.component';
import { SkLoaderComponent } from './sk-loader/sk-loader.component';
import { SkTextAreaComponent } from './sk-text-area/sk-text-area.component';

@NgModule({
  imports: [
    CKEditorModule,
    CommonModule,
    DxTextBoxModule,
    DxTextAreaModule,
    DxNumberBoxModule,
    DxSelectBoxModule,
    DxButtonModule,
    DxCheckBoxModule,
    DxDateBoxModule,
    DxFileUploaderModule,
    DxContextMenuModule,
    DxTabsModule,
    DxLoadPanelModule,
  ],
  declarations: [
    SkTextBoxComponent,
    SkTextAreaComponent,
    SkSelectBoxComponent,
    SkButtonComponent,
    SkFormattedTextEditorComponent,
    SkCheckBoxComponent,
    SkFileUploaderComponent,
    SkDateBoxComponent,
    MinDateDirective,
    MaxDateDirective,
    SkRatingBoxComponent,
    SkDropdownButtonComponent,
    SkDropdownButtonItemComponent,
    SkTabsComponent,
    SkTabsItemComponent,
    SkLoaderComponent,
  ],
  exports: [
    SkTextBoxComponent,
    SkTextAreaComponent,
    SkSelectBoxComponent,
    SkButtonComponent,
    SkFormattedTextEditorComponent,
    SkCheckBoxComponent,
    SkDateBoxComponent,
    MinDateDirective,
    MaxDateDirective,
    SkFileUploaderComponent,
    SkRatingBoxComponent,
    SkDropdownButtonComponent,
    SkDropdownButtonItemComponent,
    SkTabsComponent,
    SkTabsItemComponent,
    SkLoaderComponent,
    DxPopupModule, // Было бы неплохо инкапсулировать это, но не критично.
    DxPopoverModule, // Не знаю пока, как нормально инкапсулировать компоненты с темплейтами.
    DxScrollViewModule, // Тоже не понятно, как инкапсулировать.
  ]
})
export class SkUiKitModule { }
