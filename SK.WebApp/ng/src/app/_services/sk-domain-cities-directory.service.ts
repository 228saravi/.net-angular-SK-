import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, publishReplay, shareReplay } from 'rxjs/operators';

import { ApiBaseUrlProvider } from './sk-api-base-url-provider.service';
import { Observable } from 'rxjs';

export class City {
  public id: string;
  public name: string;
}

export class Res {
  cities: City[] = [];
}

@Injectable({
  providedIn: 'root'
})
export class CitiesDirectory {
  private _res$: Observable<Res>;

  public constructor(private _http: HttpClient, private _baseUrlProvider: ApiBaseUrlProvider) {
    this._res$ = this._http.get(this._baseUrlProvider.concatWith("CitiesDirectory/GetAll"))
      .pipe(
        map(res => res as Res),
        shareReplay(1)
    );
  }

  public async getAll(): Promise<Res> {
    return await this._res$.toPromise();
  }
}
