import { Injectable, Injector } from '@angular/core';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ApiBaseUrlProvider {
  constructor(private _injector: Injector) {

  }

  public getApiBaseUrl(): string {
    var origin = this._injector.get("BASE_URL", "/");
    return `${origin}api/`;
  }

  public concatWith(lastPartOfUrl: string): string {
    return this.getApiBaseUrl() + lastPartOfUrl;
  }
}
