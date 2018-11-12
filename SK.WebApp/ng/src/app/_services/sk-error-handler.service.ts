import { ErrorHandler as AngularErrorHandler, Injectable, Injector } from '@angular/core';
import { Router } from '@angular/router';
import * as Sentry from "@sentry/browser";

Sentry.init({
  dsn: "https://3e41b05cf08549deb4f9d93b41c860b9@sentry.io/1306372"
});

@Injectable({
  providedIn: 'root'
})
export class ErrorHandler implements AngularErrorHandler {
  private _injector: Injector;

  private _storedError: Error = null;

  private putErrorToStore(err: Error): void {
    this._storedError = err;
  }

  public constructor(injector: Injector) {
    this._injector = injector;
  }

  public getErrorFromStore(): Error {
    return this._storedError;
  }

  public handleError(error: Error): void {
    Sentry.captureException(error);

    //if (window) {
    //  console.log(error);
    //  window.location.href = "/error-page";
    //}

    this.putErrorToStore(error);

    const router = this._injector.get(Router);

    setTimeout(() => {
      router.navigate(["error-page"]).then(() => {
        console.error(error);
      });
    }, 0);
  }
}
