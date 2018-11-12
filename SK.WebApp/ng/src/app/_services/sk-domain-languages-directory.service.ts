import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map, shareReplay } from 'rxjs/operators';

import { ApiBaseUrlProvider } from './sk-api-base-url-provider.service';

export class Language {
  public id: string;
  public name: string;
}

export class Res {
  languages: Language[] = [];
}

@Injectable({
  providedIn: 'root'
})
export class LanguagesDirectory {
  private _res$: Observable<Res>;

  public constructor(private _http: HttpClient, private _baseUrlProvider: ApiBaseUrlProvider) {
    this._res$ = this._http.get(this._baseUrlProvider.concatWith("LanguagesDirectory/GetAll"))
      .pipe(
        map(res => res as Res),
        shareReplay(1)
      );
  }

  public async getAll(): Promise<Res> {
    return await this._res$.toPromise();
  }
}
