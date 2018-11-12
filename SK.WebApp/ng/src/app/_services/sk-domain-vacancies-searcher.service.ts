import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map } from 'rxjs/operators';

import { ApiBaseUrlProvider } from './sk-api-base-url-provider.service';

export class Req {
  vacancyId: number;
  cityId: string = null;
  minRatePerHour: number = null;
  specialitiesIds: string[] = [];
  specializationIds: string[] = [];
  skillIds: string[] = [];

  take: number;
  skip: number;

  public constructor(
    vacancyId: number,
    cityId: string,
    minRatePerHour: number,
    specialitiesIds: string[],
    specializationIds: string[],
    skillIds: string[],
    take: number = null,
    skip: number = null,
  ) {
    this.vacancyId = vacancyId;
    this.cityId = cityId;
    this.minRatePerHour = minRatePerHour;
    this.specialitiesIds = specialitiesIds;
    this.specializationIds = specializationIds;
    this.skillIds = skillIds;

    this.take = take;
    this.skip = skip;
  }
}

export class City {
  public id: string;
  public name: string;
}

export class Language {
  public id: string;
  public name: string;
}

export class Document {
  public id: string;
  public name: string;
}

export class Experience {
  public id: string;
  public name: string;
}

export class Segment {
  public id: string;
  public name: string;
}

export class EventType {
  public id: string;
  public name: string;
}

export class EventFormat {
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
  public Id: string;
  public Name: string;
}

export class Speciality {
  public id: string;
  public name: string;
  public specialization: Specialization;
  public skillsGroups: SkillsGroup[];
}

export class Company {
  public id: number;
  public name: string;
  public thumbnailImageUrl: string;
  public rating: number;
}

export class Event {
  public id: number;
  public name: string;
  public segment: Segment;
  public format: EventFormat;
  public type: EventType;
  public city: City;
  public address: string;
  public startTime: Date;
  public endTime: Date;
  public estimatedGuestsCount: number;
  public estimatedAverageCheck: number;

  public totalCooksCount: number;
  public totalBarmansCount: number;
  public totalWaitersCount: number;

  public connectedCooksCount: number;
  public connectedBarmansCount: number;
  public connectedWaitersCount: number;

  public company: Company;
}

export class Connection {
  public id: number;
  public status: string;
}

export class Vacancy {
  public id: number;
  public event: Event;
  public speciality: Speciality;
  public languages: Language[];
  public documents: Document[];
  public experience: Experience;
  public ratePerHour: number;
  public workingHours: number;
  public amount: number;
  public connection: Connection;
}

export class Res {
  public foundVacancies: Vacancy[];
  public totalCount: number;
}

@Injectable({ 
  providedIn: 'root'
})
export class VacanciesSearcher {
  public constructor(private _http: HttpClient, private _baseUrlProvider: ApiBaseUrlProvider) {

  }

  public async search(req: Req): Promise<Res> {
    var searchParams = new HttpParams();

    if (req.vacancyId) {
      searchParams = searchParams.set("vacancyId", req.vacancyId.toString());
    }

    if (req.cityId) {
      searchParams = searchParams.set("cityId", req.cityId);
    }

    if (req.minRatePerHour) {
      searchParams = searchParams.set("minRatePerHour", req.minRatePerHour.toString());
    }
    

    for (var specialityId of req.specialitiesIds) {
      searchParams = searchParams.append("specialityIds", specialityId);
    }

    for (var specializationId of req.specializationIds) {
      searchParams = searchParams.append("specializationIds", specializationId);
    }

    for (var skillId of req.skillIds) {
      searchParams = searchParams.append("skillIds", skillId);
    }


    if (req.take) {
      searchParams = searchParams.append("take", req.take.toString());
    }

    if (req.skip) {
      searchParams = searchParams.append("skip", req.skip.toString());
    }

    
    return await this._http.get(this._baseUrlProvider.concatWith("VacanciesSearcher/Search"), { params: searchParams })
      .pipe(map(res => res as Res))
      .toPromise();
  }
}
