import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';

import { Subject, Observable } from 'rxjs';
import { map, tap } from 'rxjs/operators';

import { ApiBaseUrlProvider } from './sk-api-base-url-provider.service';
import { HttpTransferableError } from '../_classes/http-transferable-error';

import { CompanyIsNotPublishedError } from './sk-domain-company-details-updator.service';
import { ProfileIsNotPublishedError } from './sk-domain-expert-profile-details-updator.service';

export namespace ExpertConnections {
  export class ExpertConnectionsReq {
    public expertProfileId: number;
  }
  export class City {
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
  export class Company {
    public id: number;
    public name: string;
    public thumbnailImageUrl: string;
    public rating: number;
  }
  export class Event {
    public id: number;
    public name: string;
    public logoImageUrl: string;
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
  export class Experience {
    public id: string;
    public name: string;
  }
  export class Language {
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
    public event: Event;
    public speciality: Speciality;
    public languages: Language[];
    public experience: Experience;
    public startTime: Date;
    public workingHours: number;
    public ratePerHour: number;
    public rating: number;
  }
  export class Feedback {
    public id: number;
    public rating: number;
    public commentHtml: string;
  }
  export class Connection {
    public id: number;
    public type: string;
    public status: string;
    public vacancy: Vacancy;
    public feedbackForExpert: Feedback;
    public feedbackForCompany: Feedback;
  }

  export class ExpertConnectionsRes {
    public connections: Connection[];
  }
}

export namespace VacancyConnections {
  export class VacancyConnectionsReq {
    public vacancyId: number;
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

  export class City {
    public id: string;
    public name: string;
  }

  export class Experience {
    public id: string;
    public name: string;
  }

  export class Expert {
    public id: number;
    public name: string;
    public photoImageUrl: string;
    public city: City;
    public rating: number;
    public ratePerHour: number;
    public speciality: Speciality;
    public experience: Experience;
  }
  export class Feedback {
    public id: number;
    public rating: number;
    public commentHtml: string;
  }
  export class Connection {
    public id: number;
    public type: string;
    public status: string;
    public expert: Expert;
    public feedbackForExpert: Feedback;
    public feedbackForCompany: Feedback;
  }

  export class VacancyConnectionsRes {
    public connections: Connection[];
  }
}


export class RegisterExpertToVacancyConnectionReq {
  public expertProfileId: number;
  public vacancyId: number;
}

export class RegisterVacancyToExpertConnectionReq {
  public vacancyId: number;
  public expertProfileId: number;
}

export class CancelConnectionReq {
  public connectionId: number;
}

export class ApproveConnectionReq {
  public connectionId: number;
}

export class PostFeedbackReq {
  public connectionId: number;
  public rating: number;
  public commentHtml: string;
}

export class AmountIsFullError extends HttpTransferableError {
  public static parseFromRes(res: HttpErrorResponse): AmountIsFullError | null {
    return HttpTransferableError.parseFromResInternal(res, "AMOUNT_IS_FULL", (extraData) => new AmountIsFullError());
  }
}

export class AlreadyConnectedError extends HttpTransferableError {
  public static parseFromRes(res: HttpErrorResponse): AlreadyConnectedError | null {
    return HttpTransferableError.parseFromResInternal(res, "ALREADY_CONNECTED", (extraData) => new AlreadyConnectedError());
  }
}

export class NotYourProfileError extends HttpTransferableError {
  public static parseFromRes(res: HttpErrorResponse): NotYourProfileError | null {
    return HttpTransferableError.parseFromResInternal(res, "NOT_YOUR_PROFILE", (extraData) => new NotYourProfileError());
  }
}

export class NotYourCompanyError extends HttpTransferableError {
  public static parseFromRes(res: HttpErrorResponse): NotYourCompanyError | null {
    return HttpTransferableError.parseFromResInternal(res, "NOT_YOUR_COMPANY", (extraData) => new NotYourCompanyError());
  }
}

export class NotYourConnectionError extends HttpTransferableError {
  public static parseFromRes(res: HttpErrorResponse): NotYourConnectionError | null {
    return HttpTransferableError.parseFromResInternal(res, "NOT_YOUR_CONNECTION", (extraData) => new NotYourConnectionError());
  }
}

export class TooLateToCancelConnectionError extends HttpTransferableError {
  public static parseFromRes(res: HttpErrorResponse): TooLateToCancelConnectionError | null {
    return HttpTransferableError.parseFromResInternal(res, "TOO_LATE_TO_CANCEL_CONNECTION", (extraData) => new TooLateToCancelConnectionError());
  }
}

@Injectable({
  providedIn: 'root'
})
export class ConnectionsManager {

  private _connectionRegisteredSubj: Subject<void> = new Subject<void>();
  private _connectionRegistrationFailedSubj: Subject<Error> = new Subject<Error>();

  private _connectionCanceledSubj: Subject<void> = new Subject<void>();
  private _connectionCancelingFailedSubj: Subject<Error> = new Subject<Error>();

  private _connectionApprovedSubj: Subject<void> = new Subject<void>();
  private _connectionApprovingFailedSubj: Subject<Error> = new Subject<Error>();

  private _feedbackPostedSubj: Subject<void> = new Subject<void>();
  private _feedbackPostingFailed: Subject<Error> = new Subject<Error>();

  public constructor(private _http: HttpClient, private _baseUrlProvider: ApiBaseUrlProvider) {

  }

  public get connectionRegistered$(): Observable<void> {
    return this._connectionRegisteredSubj;
  }

  public get connectionRegistrationFailed$(): Observable<Error> {
    return this._connectionRegistrationFailedSubj;
  }

  public get connectionCanceled$(): Observable<void> {
    return this._connectionCanceledSubj;
  }

  public get connectionCancelingFailed$(): Observable<Error> {
    return this._connectionCancelingFailedSubj;
  }

  public get connectionApproved$(): Observable<void> {
    return this._connectionApprovedSubj;
  }

  public get connectionApprovingFailed$(): Observable<Error> {
    return this._connectionApprovingFailedSubj;
  }

  public get feedbackPosted$(): Observable<void> {
    return this._feedbackPostedSubj;
  }

  public get feedbackPostingFailed(): Observable<Error> {
    return this._feedbackPostingFailed;
  }

  public async getExpertConnections(req: ExpertConnections.ExpertConnectionsReq): Promise<ExpertConnections.ExpertConnectionsRes> {
    var httpParams = new HttpParams().set("expertProfileId", req.expertProfileId.toString());
    var result = await this._http.get(this._baseUrlProvider.concatWith("ConnectionsManager/GetExpertConnections"), { params: httpParams })
      .pipe(
        map(res => res as ExpertConnections.ExpertConnectionsRes),
        tap(res => {
          for (var c of res.connections) {
            c.vacancy.startTime = new Date(c.vacancy.startTime);
            c.vacancy.event.startTime = new Date(c.vacancy.event.startTime);
            c.vacancy.event.endTime = new Date(c.vacancy.event.endTime)
          }
        })
      ).toPromise();

    return result;
  }

  public async getVacancyConnections(req: VacancyConnections.VacancyConnectionsReq): Promise<VacancyConnections.VacancyConnectionsRes> {
    var httpParams = new HttpParams().set("vacancyId", req.vacancyId.toString());
    var result = await this._http.get(this._baseUrlProvider.concatWith("ConnectionsManager/GetVacancyConnections"), { params: httpParams })
      .pipe(map(res => res as VacancyConnections.VacancyConnectionsRes))
      .toPromise();

    return result;
  }

  public async registerExpertToVacancyConnection(req: RegisterExpertToVacancyConnectionReq): Promise<void> {
    try {
      await this._http.post(this._baseUrlProvider.concatWith("ConnectionsManager/RegisterExpertToVacancyConnection"), req).toPromise();
      this._connectionRegisteredSubj.next(null);
    } catch (e) {
      if (e instanceof HttpErrorResponse) {
        var err = AmountIsFullError.parseFromRes(e) ||
          AlreadyConnectedError.parseFromRes(e) ||
          NotYourProfileError.parseFromRes(e) ||
          ProfileIsNotPublishedError.parseFromRes(e);
        if (err) {
          this._connectionRegistrationFailedSubj.next(err);
          throw err;
        } else {
          throw e;
        }
      }
    }
  }

  public async registerVacancyToExpertConnection(req: RegisterVacancyToExpertConnectionReq): Promise<void> {
    try {
      await this._http.post(this._baseUrlProvider.concatWith("ConnectionsManager/RegisterVacancyToExpertConnection"), req).toPromise();
      this._connectionRegisteredSubj.next(null);
    } catch (e) {
      if (e instanceof HttpErrorResponse) {
        var err = AmountIsFullError.parseFromRes(e) ||
          AlreadyConnectedError.parseFromRes(e) ||
          NotYourProfileError.parseFromRes(e) ||
          CompanyIsNotPublishedError.parseFromRes(e);
        if (err) {
          this._connectionRegistrationFailedSubj.next(err);
          throw err;
        } else {
          throw e;
        }
      }
    }
  }

  public async cancelConnection(req: CancelConnectionReq): Promise<void> {
    try {
      await this._http.post(this._baseUrlProvider.concatWith("ConnectionsManager/CancelConnection"), req).toPromise();
    } catch (e) {
      if (e instanceof HttpErrorResponse) {
        var ex = NotYourConnectionError.parseFromRes(e) || TooLateToCancelConnectionError.parseFromRes(e);
        if (ex) {
          this._connectionCancelingFailedSubj.next(ex);
          throw ex;
        } else {
          throw e;
        }
      }
    }

    this._connectionCanceledSubj.next(null);
  }

  public async approveConnection(req: ApproveConnectionReq): Promise<void> {
    try {
      await this._http.post(this._baseUrlProvider.concatWith("ConnectionsManager/ApproveConnection"), req).toPromise();
    } catch (e) {
      if (e instanceof HttpErrorResponse) {
        var ex = NotYourConnectionError.parseFromRes(e);
        if (ex) {
          this._connectionApprovingFailedSubj.next(ex);
          throw ex;
        } else {
          throw e;
        }
      }
    }

    this._connectionApprovedSubj.next(null);
  }

  public async postFeedback(req: PostFeedbackReq): Promise<void> {
    try {
      await this._http.post(this._baseUrlProvider.concatWith("ConnectionsManager/PostFeedback"), req).toPromise();
    } catch (e) {
      if (e instanceof HttpErrorResponse) {
        var ex = NotYourConnectionError.parseFromRes(e);
        if (ex) {
          this._feedbackPostingFailed.next(ex);
          throw ex;
        } else {
          throw e;
        }
      }
    }

    this._feedbackPostedSubj.next(null);
  }
}
