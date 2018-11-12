import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';

import { ApiBaseUrlProvider } from './sk-api-base-url-provider.service';

export class Specialization {
  public id: string;
  public name: string;
  public rank: number;
}

export class Skill {
  public id: string;
  public name: string;
  public rank: number;
  public groupName: string;
}

export class SkillsGroup {
  public name: string;
  public skills: Skill[];
}

export class Speciality {
  public id: string;
  public name: string;
  public specializations: Specialization[] = [];
  public skills: Skill[] = [];
  public skillsGroups: SkillsGroup[] = [];
}

export class SkillsMatrix {
  public specialities: Speciality[] = [];
}

@Injectable({
  providedIn: 'root'
})
export class SkillsMatrixDirectory {

  public constructor(private _http: HttpClient, private _baseUrlProvider: ApiBaseUrlProvider) {

  }

  public async get(): Promise<SkillsMatrix> {
    return await this._http.get(this._baseUrlProvider.concatWith("SkillsMatrixDirectory/Get"))
      .pipe(map(res => res as SkillsMatrix))
      .toPromise();
  }
}
