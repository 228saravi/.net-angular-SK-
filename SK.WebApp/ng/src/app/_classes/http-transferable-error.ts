import { HttpErrorResponse } from '@angular/common/http';

export class HttpTransferableError extends Error {
  __proto__: Error;

  private _displayMessage: string;

  public constructor(message?: string, displayMessage?: string) {
    const trueProto = new.target.prototype;

    super(message || null);
    this._displayMessage = displayMessage || null;

    // Alternatively use Object.setPrototypeOf if you have an ES6 environment.
    this.__proto__ = trueProto;
  }

  public get displayMessage(): string {
    return this._displayMessage || null;
  }

  protected static parseFromResInternal<T>(res: HttpErrorResponse, type: string, create: (extraData: any) => T): T {
    var json = res.error;
    if (json.type == type) {
      var error = create(json.extraData);
      if (error instanceof HttpTransferableError) {
        error.message = json.message || null;
        error._displayMessage = json.displayMessage || null;
      }
      return error;
    }

    return null;
  }
}
