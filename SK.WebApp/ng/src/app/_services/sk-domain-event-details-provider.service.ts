import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map } from 'rxjs/operators';

import { ApiBaseUrlProvider } from './sk-api-base-url-provider.service';

export class Req {
  public eventId: number;
  public withNotFullVacancies?: boolean;
  public withConnectionsForExpert?: number;
}

export class Segment {
  public id: string;
  public name: string;
}

export class Type {
  public id: string;
  public name: string;
}
export class Format {
  public id: string;
  public name: string;
}
export class Company {
  public id: number;
  public name: string;
  public logoImageUrl: string;
  public rating: number;
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
export class Connection {
  public id: number;
}
export class Experience {
  public id: string;
  public name: string;
}
export class Vacancy {
  public id: number;
  public isPublished: boolean;
  public speciality: Speciality;
  public ratePerHour: number;
  public startTime: Date;
  public workingHours: number;
  public experience: Experience;
  public rating: number;
  public connection: Connection;
}
export class City {
  public id: string;
  public name: string;
}
export class Event {
  public id: number;
  public name: string;
  public isPublic: boolean;
  public isPublished: boolean;
  public city: City;
  public address: string;
  public startTime: Date;
  public endTime: Date;
  public segment: Segment;
  public type: Type;
  public format: Format;
  public logoImageUrl: string;
  public descriptionHtml: string;
  public estimatedGuestsCount: number;
  public estimatedAverageCheck: number;
  public withtDelivery: boolean;
  public withAccomodation: boolean;
  public totalCooksCount: number;
  public totalBarmansCount: number;
  public totalWaitersCount: number;
  public company: Company;
  public vacancies: Vacancy[];
}

export class Res {
  public event: Event;
}

@Injectable({
  providedIn: 'root'
})
export class EventDetailsProvider {
  public constructor(private _http: HttpClient, private _baseUrlProvider: ApiBaseUrlProvider) {

  }

  public async get(req: Req): Promise<Res> {
    var httpParams = new HttpParams().set("eventId", req.eventId.toString());

    if (req.withNotFullVacancies) {
      httpParams = httpParams.set("withNotFullVacancies", "true");
    }

    if (req.withConnectionsForExpert) {
      httpParams = httpParams.set("withConnectionsForExpert", req.withConnectionsForExpert.toString());
    }

    return await this._http.get(this._baseUrlProvider.concatWith("EventDetailsProvider/Get"), { params: httpParams })
      .pipe(map(res => res as Res))
      .toPromise();
  }
}
