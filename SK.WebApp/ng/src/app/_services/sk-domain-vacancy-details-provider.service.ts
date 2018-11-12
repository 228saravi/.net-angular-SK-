import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map } from 'rxjs/operators';

import { ApiBaseUrlProvider } from './sk-api-base-url-provider.service';
import { ApiDtoProcessor } from './sk-api-dto-processor.service';
import { VacanciesSearcher, Req as VacanciesSearcherReq } from './sk-domain-vacancies-searcher.service';

export class Req {
  vacancyId: number;

  public constructor(vacancyId: number) {
    this.vacancyId = vacancyId;
  }
}

export class Experience {
  public id: string;
  public name: string;
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
  public id: string;
  public name: string;
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
  public eventFormat: EventFormat;
  public eventType: EventType;
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

  public company: Company;
}

export class Connection {
  public id: number;
  public type: string;
  public status: string;
}

export class Vacancy {
  public id: number;
  public isPublished: boolean;
  public isPublic: boolean;
  public event: Event;
  public speciality: Speciality;
  public languages: Language[];
  public documents: Document[];
  public startTime: Date;
  public experience: Experience;
  public ratePerHour: number;
  public workingHours: number;
  public rating: number;
  public amount: number;
  public aboutVacancyHtml: string;
  public connection: Connection;
}

export class Res {
  public foundVacancy: Vacancy;
}

@Injectable({
  providedIn: 'root'
})
export class VacancyDetailsProvider {
  public constructor(private _http: HttpClient, private _baseUrlProvider: ApiBaseUrlProvider, private _apiDtoProcessor: ApiDtoProcessor) {

  }

  public async get(req: Req): Promise<Res> {
    var httpParams = new HttpParams().set("vacancyId", req.vacancyId.toString())

    return await this._http.get(this._baseUrlProvider.concatWith("VacancyDetailsProvider/Get"), { params: httpParams })
      .pipe(
        map(res => {
          var r = res as Res;
          this._apiDtoProcessor.processRes(r);
          return r;
        })
      )
      .toPromise();
  }
}
