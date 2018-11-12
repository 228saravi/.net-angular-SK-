import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, publishReplay, shareReplay } from 'rxjs/operators';

import { ApiBaseUrlProvider } from './sk-api-base-url-provider.service';
import { Observable } from 'rxjs';

export class EventType {
  public id: string;
  public name: string;
  public rank: number;
}

export class Res {
  eventTypes: EventType[] = [];
}

@Injectable({
  providedIn: 'root'
})
export class EventTypesDirectory {
  private _res$: Observable<Res>;

  public constructor(private _http: HttpClient, private _baseUrlProvider: ApiBaseUrlProvider) {
    this._res$ = this._http.get(this._baseUrlProvider.concatWith("EventTypesDirectory/GetAll"))
      .pipe(
        map(res => res as Res),
        shareReplay(1)
    );
  }

  public async getAll(): Promise<Res> {
    return await this._res$.toPromise();
  }
}
