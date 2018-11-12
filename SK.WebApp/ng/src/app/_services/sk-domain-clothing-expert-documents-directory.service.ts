import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, publishReplay, shareReplay } from 'rxjs/operators';

import { ApiBaseUrlProvider } from './sk-api-base-url-provider.service';
import { Observable } from 'rxjs';

export class Document {
  public id: string;
  public name: string;
}

export class Res {
  documents: Document[] = [];
}

@Injectable({
  providedIn: 'root'
})
export class ExpertDocumentsDirectory {
  private _res$: Observable<Res>;

  public constructor(private _http: HttpClient, private _baseUrlProvider: ApiBaseUrlProvider) {
    this._res$ = this._http.get(this._baseUrlProvider.concatWith("ExpertDocumentsDirectory/GetAll"))
      .pipe(
        map(res => res as Res),
        shareReplay(1)
    );
  }

  public async getAll(): Promise<Res> {
    return await this._res$.toPromise();
  }
}
