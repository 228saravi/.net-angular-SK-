import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpErrorResponse } from '@angular/common/http';

import { map } from 'rxjs/operators';

import { ApiBaseUrlProvider } from './sk-api-base-url-provider.service';

import { HttpTransferableError } from '../_classes/http-transferable-error';
import { Subject, Observable } from 'rxjs';

export class UpdateHeaderReq {
  public eventId: number;
  public name: string;
  public typeId: string;
  public formatId: string;
}

export class UpdateMainInfoReq {
  public eventId: number;
  public startTime: Date;
  public endTime: Date;
  public segmentId: string;
  public cityId: string;
  public address: string;
  public estimatedGuestsCount: number;
  public estimatedAverageCheck: number;
  public withDelivery: boolean;
  public withAccomodation: boolean;
}

export class UpdateAboutEventReq {
  public eventId: number;
  public aboutEventHtml: string;
}

export class PublishReq {
  public eventId: number;
}

export class RegisterVacancyReq {
  public eventId: number;
}

export class RegisterVacancyRes {
  public vacancyId: number;
}

export class EventCheckBeforePublish {
  public nameSet: boolean;
  public logoSet: boolean;
  public typeSet: boolean;
  public formatSet: boolean;
  public startTimeSet: boolean;
  public endTimeSet: boolean;
  public segmentSet: boolean;
  public citySet: boolean;
  public addrerssSet: boolean;
  public isReadyForPublish: boolean;
}

export class EventIsNotPublishedError extends HttpTransferableError {
  private _eventCheck: EventCheckBeforePublish;

  private constructor(eventCheck: EventCheckBeforePublish) {
    super();

    this._eventCheck = eventCheck;
  }

  public get eventCheck(): EventCheckBeforePublish {
    return this._eventCheck;
  }

  public static parseFromRes(res: HttpErrorResponse): EventIsNotPublishedError | null {
    return HttpTransferableError.parseFromResInternal(res, "EVENT_IS_NOT_PUBLISHED", (extraData) => new EventIsNotPublishedError(extraData.eventCheck));
  }
}

@Injectable({
  providedIn: 'root'
})
export class EventDetailsUpdator {
  private _publishedSubj: Subject<void> = new Subject<void>();
  private _unpublishedSubj: Subject<void> = new Subject<void>();
  private _publishFailedSubj: Subject<Error> = new Subject<Error>();

  public constructor(private _http: HttpClient, private _baseUrlProvider: ApiBaseUrlProvider) { }

  public get published$(): Observable<void> {
    return this._publishedSubj;
  }

  public get unpublished$(): Observable<void> {
    return this._unpublishedSubj;
  }

  public get publishFailed$(): Observable<Error> {
    return this._publishFailedSubj;
  }

  public async updateHeader(req: UpdateHeaderReq): Promise<void> {
    try {
      await this._http.post(this._baseUrlProvider.concatWith("EventDetailsUpdator/UpdateHeader"), req).toPromise();
    } catch (e) {
      if (e instanceof HttpErrorResponse) {
        var ex = ex || EventIsNotPublishedError.parseFromRes(e);
        if (ex) {
          this._publishFailedSubj.next(ex);
          throw ex;
        }
        throw e;
      }
    }
  }

  public async updateMainInfo(req: UpdateMainInfoReq): Promise<void> {
    try {
      await this._http.post(this._baseUrlProvider.concatWith("EventDetailsUpdator/UpdateMainInfo"), req).toPromise();
    } catch (e) {
      if (e instanceof HttpErrorResponse) {
        var ex = ex || EventIsNotPublishedError.parseFromRes(e);
        if (ex) {
          this._publishFailedSubj.next(ex);
          throw ex;
        }
        throw e;
      }
    }
  }

  public async updateAboutEvent(req: UpdateAboutEventReq): Promise<void> {
    try {
      await this._http.post(this._baseUrlProvider.concatWith("EventDetailsUpdator/UpdateAboutEvent"), req).toPromise();
    } catch (e) {
      if (e instanceof HttpErrorResponse) {
        var ex = ex || EventIsNotPublishedError.parseFromRes(e);
        if (ex) {
          this._publishFailedSubj.next(ex);
          throw ex;
        }
        throw e;
      }
    }
  }

  public async publish(req: PublishReq): Promise<void> {
    try {
      await this._http.post(this._baseUrlProvider.concatWith("EventDetailsUpdator/Publish"), req).toPromise();
      this._publishedSubj.next(null);
    } catch (e) {
      if (e instanceof HttpErrorResponse) {
        var ex = ex || EventIsNotPublishedError.parseFromRes(e);
        if (ex) {
          this._publishFailedSubj.next(ex);
          throw ex;
        }
        throw e;
      }
    }
  }

  public async unpublish(req: PublishReq): Promise<void> {
    await this._http.post(this._baseUrlProvider.concatWith("EventDetailsUpdator/Unpublish"), req).toPromise();
    this._unpublishedSubj.next(null);
  }

  public async makePublic(req: PublishReq): Promise<void> {
    await this._http.post(this._baseUrlProvider.concatWith("EventDetailsUpdator/MakePublic"), req).toPromise();
    //this._unpublishedSubj.next(null);
  }

  public async makePrivate(req: PublishReq): Promise<void> {
    await this._http.post(this._baseUrlProvider.concatWith("EventDetailsUpdator/MakePrivate"), req).toPromise();
    //this._unpublishedSubj.next(null);
  }

  public async delete(req: PublishReq): Promise<void> {
    await this._http.post(this._baseUrlProvider.concatWith("EventDetailsUpdator/Delete"), req).toPromise();
    //this._unpublishedSubj.next(null);
  }

  public async registerVacancy(req: RegisterVacancyReq): Promise<RegisterVacancyRes> {
    try {
      return await this._http.post(this._baseUrlProvider.concatWith("EventDetailsUpdator/RegisterVacancy"), req)
        .pipe(map(res => res as RegisterVacancyRes))
        .toPromise();
    } catch (e) {
      if (e instanceof HttpErrorResponse) {
        var ex = ex || EventIsNotPublishedError.parseFromRes(e);
        if (ex) {
          this._publishFailedSubj.next(ex);
          throw ex;
        }
        throw e;
      }
    }
  }
}
