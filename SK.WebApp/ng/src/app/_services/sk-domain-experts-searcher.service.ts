import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map } from 'rxjs/operators';

import { ApiBaseUrlProvider } from './sk-api-base-url-provider.service';

export class Req {
  public expertProfileId: number;
  public cityId: string;
  public maxRatePerHour: number;
  public specialityIds: string[];
  public specializationIds: string[];
  public skillIds: string[];

  public skip: number;
  public take: number;

  public constructor(
    expertProfileId: number,
    cityId: string,
    maxRatePerHour: number,
    specialityIds: string[],
    specializationIds: string[],
    skillIds: string[],
    skip: number = null,
    take: number = null,
  ) {
    this.expertProfileId = expertProfileId;
    this.cityId = cityId;
    this.maxRatePerHour = maxRatePerHour;
    this.specialityIds = specialityIds;
    this.specializationIds = specializationIds;
    this.skillIds = skillIds;
    this.skip = skip;
    this.take = take;
  }
}

export class Language {
  public id: string;
  public name: string;
}

export class City {
  public id: string;
  public name: string;
}

export class Document {
  public id: string;
  public name: string;
}

export class ClothingSize {
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

export class Speciality {
  public id: string;
  public name: string;
  public specialization: Specialization;
  public skillsGroups: SkillsGroup[];
}

export class Expert {
  public id: number;
  public isPublished: boolean;
  public name: string;
  public photoImageUrl: string;
  public rating: number;
  public city: City;
  public languages: Language[];
  public documents: Document[];
  public speciality: Speciality;
  public clothingSize: ClothingSize;
  public ratePerHour: number;
  public experience: Experience;
  public aboutMeHtml: string;
}

export class Res {
  public experts: Expert[];
  public totalCount: number;
}

@Injectable({
  providedIn: 'root'
})
export class ExpertsSearcher {
  public constructor(private _http: HttpClient, private _baseUrlProvider: ApiBaseUrlProvider) {

  }

  public async search(req: Req): Promise<Res> {
    var searchParams = new HttpParams();

    if (req.expertProfileId) {
      searchParams = searchParams.append("expertProfileId", req.expertProfileId.toString());
    }

    if (req.cityId) {
      searchParams = searchParams.append("cityId", req.cityId);
    }

    if (req.maxRatePerHour) {
      searchParams = searchParams.append("maxRatePerHour", req.maxRatePerHour.toString());
    }
    
    for (var specialityId of req.specialityIds) {
      searchParams = searchParams.append("specialityIds", specialityId);
    }

    for (var specializationId of req.specializationIds) {
      searchParams = searchParams.append("specializationIds", specializationId);
    }

    for (var skillId of req.skillIds) {
      searchParams = searchParams.append("skillIds", skillId);
    }

    if (req.skip) {
      searchParams = searchParams.append("skip", req.skip.toString());
    }

    if (req.take) {
      searchParams = searchParams.append("take", req.take.toString());
    }

    return await this._http.get(this._baseUrlProvider.concatWith("ExpertsSearcher/Search"), { params: searchParams })
      .pipe(map(res => res as Res))
      .toPromise();
  }
}
