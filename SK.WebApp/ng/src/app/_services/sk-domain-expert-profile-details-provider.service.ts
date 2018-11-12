import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map } from 'rxjs/operators';

import { ApiBaseUrlProvider } from './sk-api-base-url-provider.service';
import { ExpertsSearcher, Req as ExpertsSearcherReq, Expert } from './sk-domain-experts-searcher.service';

export class Req {
  public expertProfileId: number;
}

export class LanguageRes {
  public id: string;
  public name: string;
}

export class CityRes {
  public id: string;
  public name: string;
}

export class Document {
  public id: string;
  public name: string;
}

export class ClothingSizeRes {
  public id: string;
  public name: string;
}

export class Experience {
  public id: string;
  public name: string;
}

export class Skill {
  public id: string;
  public name: string;
}

export class SkillsGroup {
  public name: string;
  public skills: Skill[];
}

export class Specialization {
  public id: string;
  public name: string;
}

export class SpecialityRes {
  public id: string;
  public name: string;
  public specialization: Specialization;
  public skillsGroups: SkillsGroup[];
}

export class ExpertProfileRes {
  public id: number;
  public name: string;
  public photoImageUrl: string;
  public rating: number;
  public city: CityRes;
  public languages: LanguageRes[];
  public documents: Document[];
  public speciality: SpecialityRes;
  public clothingSize: ClothingSizeRes;
  public ratePerHour: number;
  public experience: Experience;
  public aboutMeHtml: string;
}

export class Res {
  public expertProfile: ExpertProfileRes;
}

@Injectable({
  providedIn: 'root'
})
export class ExpertProfileDetailsProvider {
  public constructor(private _expertsSearcher: ExpertsSearcher, private _http: HttpClient, private _baseUrlProvider: ApiBaseUrlProvider) {

  }

  public async get(req: Req): Promise<Res> {

    var httpParams = new HttpParams().set("expertProfileId", req.expertProfileId.toString());

    return await this._http.get(this._baseUrlProvider.concatWith("ExpertProfileDetailsProvider/Get"), { params: httpParams })
      .pipe(map(res => res as Res))
      .toPromise();
  }
}
