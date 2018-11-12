import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map } from 'rxjs/operators';

import { ApiBaseUrlProvider } from './sk-api-base-url-provider.service';

export class Req {
  public companyId: number;
  public withVacancies: boolean;
}

export class City {
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

export class Vacancy {
  public id: number;
  public isPublished: boolean;
  public speciality: Speciality;
  public ratePerHour: number;
  public startTime: Date;
  public workingHours: number;
  public experience: Experience;

  public amount: number;
  public connectedExpertsCount: number;
  public connectionsCount: number;
  public incomingConnectionsCount: number;
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

export class Event {
  public id: number;
  public name: string;
  public isPublished: boolean;
  public segment: Segment;
  public format: EventFormat;
  public type: EventType;
  public city: City;
  public address: string;
  public startTime: Date;
  public endTime: Date;
  public logoImageUrl: string;
  public estimatedGuestsCount: number;
  public estimatedAverageCheck: number;
  public totalCooksCount: number;
  public totalBarmansCount: number;
  public totalWaitersCount: number;
  public connectedCooksCount: number;
  public connectedBarmansCount: number;
  public connectedWaitersCount: number;
  public connectionsCount: number;
  public incomingConnectionsCount: number;
  public vacancies: Vacancy[];
}

export class Company {
  public id: number;
  public name: string;
  public isPublished: boolean;
  public city: City;
  public rating: number;
  public logoImageUrl: string;
  public thumbnailImageUrl: string;
  public aboutCompanyHtml: string;
  public events: Event[];
}

export class Res {
  public company: Company;
}

@Injectable({
  providedIn: 'root'
})
export class CompanyDetailsProvider {
  public constructor(private _http: HttpClient, private _baseUrlProvider: ApiBaseUrlProvider) {

  }

  public async get(req: Req): Promise<Res> {
    var httpParams = new HttpParams().set("companyId", req.companyId.toString()).set("withVacancies", req.withVacancies ? "true" : "false");
    return await this._http.get(this._baseUrlProvider.concatWith("CompanyDetailsProvider/Get"), { params: httpParams })
      .pipe(map(res => res as Res))
      .toPromise();
  }
}
