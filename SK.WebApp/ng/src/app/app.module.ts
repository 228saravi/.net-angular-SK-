import { NgModule, ErrorHandler } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from "./app-routing.module";
import { SkCommonModule } from "./sk-common/sk-common.module";

import { AppComponent } from './app.component';
import { ErrorHandler as SkErrorHandler } from './_services/sk-error-handler.service';

import 'devextreme-intl';
import { locale, loadMessages } from 'devextreme/localization';

import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { ApiDtoProcessorInterceptor } from './_services/sk-api-dto-processor.service';

var ruMessages = require('devextreme/localization/messages/ru.json');
loadMessages(ruMessages);
locale("ru"); // Можно в зависимости от локали браузера тут взять

@NgModule({
  declarations: [
    AppComponent,
  ],
  imports: [
    BrowserModule.withServerTransition({
      appId: 'sk-web-app'
    }),
    AppRoutingModule,
    SkCommonModule,
  ],
  providers: [
    { provide: ErrorHandler, useClass: SkErrorHandler },
    { provide: HTTP_INTERCEPTORS, useClass: ApiDtoProcessorInterceptor, multi: true },
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
