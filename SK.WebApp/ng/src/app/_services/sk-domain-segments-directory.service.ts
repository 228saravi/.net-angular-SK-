import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, publishReplay, shareReplay } from 'rxjs/operators';

import { ApiBaseUrlProvider } from './sk-api-base-url-provider.service';
import { Observable } from 'rxjs';

export class Segment {
  public id: string;
  public name: string;
}

export class Res {
  segments: Segment[] = [];
}

@Injectable({
  providedIn: 'root'
})
export class SegmentsDirectory {
  private _queryObservable: Observable<Res> = null;

  public constructor(private _http: HttpClient, private _baseUrlProvider: ApiBaseUrlProvider) {
    this._queryObservable = this._http.get(this._baseUrlProvider.concatWith("SegmentsDirectory/GetAll"))
      .pipe(
        map(res => res as Res),
        shareReplay(1)
      );
  }

  public async getAll(): Promise<Res> {
    return await this._queryObservable.toPromise();
  }
}

