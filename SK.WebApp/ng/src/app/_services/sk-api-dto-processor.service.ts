import { Injectable } from '@angular/core';
import {
  HttpEvent, HttpInterceptor, HttpHandler, HttpRequest, HttpErrorResponse, HttpResponse
} from '@angular/common/http';

import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class ApiDtoProcessor {
  constructor() { }

  private dateFromString(str: string): Date {
    var a = str.split(/[^0-9]/).map(s => parseInt(s, 10));
    return new Date(a[0], a[1] - 1 || 0, a[2] || 1, a[3] || 0, a[4] || 0, a[5] || 0, a[6] || 0);
  }

  private parseDate(str: string): Date {
    var n = Date.parse(str);

    if (isNaN(n)) {
      return null;
    } else {
      return this.dateFromString(str);
    }
  }

  private formatTime(time: Date): string {
    time = new Date(time);
    var withOffset = new Date(time.setMinutes(time.getMinutes() - time.getTimezoneOffset()));
    var str = withOffset.toJSON();
    str = str.replace("Z", "");
    return str;
  }

  private formatDate(date: Date): string {
    var timeStr = this.formatTime(date);
    var dateStr = timeStr.substr(0, 10);
    return dateStr;
  }

  private processReqDates(input: any, key: any = null): void {
    if (input == null) {
      return;
    }

    var value = key ? input[key] : input;
    if (key && value instanceof Date) {
      if (key.toLowerCase().endsWith("time")) {
        input[key] = this.formatTime(value);
      } else {
        input[key] = this.formatDate(value);
      }
    }

    if (value && typeof value == "object") {
      for (var valueKey in value) {
        this.processReqDates(value, valueKey);
      }
    }

    if (Array.isArray(input)) {
      for (var i = 0; i < input.length; i++) {
        this.processReqDates(value, i);
      }
    }
  }

  private processResDates(input: any, key: any = null): void {
    if (input == null) {
      return;
    }

    var value = key ? input[key] : input;
    if (key && typeof value === "string") {
      var date = this.parseDate(value);

      if (date) {

        if (typeof key === "string") {

          if (key.toLowerCase().endsWith("date")) {
            date.setHours(0, 0, 0, 0);
            input[key] = date;
          }

          if (key.toLowerCase().endsWith("time")) {
            input[key] = date;
          }

        }

      }
    }

    if (value && typeof value == "object") {
      for (var valueKey in value) {
        this.processResDates(value, valueKey);
      }
    }

    if (Array.isArray(input)) {
      for (var i = 0; i < input.length; i++) {
        this.processResDates(value, i);
      }
    }
  }

  public processReq(req: any): void {
    this.processReqDates(req);
  }

  public processRes(res: any): void {
    this.processResDates(res);
  }
}


@Injectable()
export class ApiDtoProcessorInterceptor implements HttpInterceptor {

  constructor(private _apiDtoProcessor: ApiDtoProcessor) { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

    var newBody = { ...req.body };
    this._apiDtoProcessor.processReq(newBody);
    var newReq = req.clone({ body: newBody });

    return next.handle(newReq).pipe(tap(event => {
      if (event instanceof HttpResponse) {
        this._apiDtoProcessor.processRes(event.body);
      }
    }));
  }
}
