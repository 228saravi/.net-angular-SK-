import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpErrorResponse } from '@angular/common/http';

import { Subject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { ApiBaseUrlProvider } from './sk-api-base-url-provider.service';
import { ApiDtoProcessor } from './sk-api-dto-processor.service';
import { HttpTransferableError } from '../_classes/http-transferable-error';

export class UpdateHeaderReq {
  public vacancyId: number;
  public specialityId: string;
  public specializationId: string;
  public amount: number;
  public ratePerHour: number;
}

export class UpdateMainInfoReq {
  public vacancyId: number;
  public startTime: Date;
  public workingHours: number;
  public experienceOptionId: string;

}

export class UpdateExtraInfoReq {
  public vacancyId: number;
  public languagesIds: string[];
  public documentsIds: string[];
  public skillsIds: string[];
}

export class UpdateAboutMeReq {
  public vacancyId: number;
  public aboutVacancyHtml: string;
}

export class PublishReq {
  public vacancyId: number;
}

export class VacancyCheckBeforePublish {
  public specialitySet: boolean;
  public specializationSet: boolean;
  public amountSet: boolean;
  public ratePerHourSet: boolean;
  public startTimeSet: boolean;
  public workingHoursSet: boolean;
  public experienceSet: boolean;
}

export class VacancyIsNotPublishedError extends HttpTransferableError {
  private _vacancyCheck: VacancyCheckBeforePublish;

  private constructor(vacancyCheck: VacancyCheckBeforePublish) {
    super();

    this._vacancyCheck = vacancyCheck;
  }

  public get vacancyCheck(): VacancyCheckBeforePublish {
    return this._vacancyCheck;
  }

  public static parseFromRes(res: HttpErrorResponse): VacancyIsNotPublishedError | null {
    return HttpTransferableError.parseFromResInternal(res, "VACANCY_IS_NOT_PUBLISHED", (extraData) => new VacancyIsNotPublishedError(extraData.profileCheck));
  }
}

@Injectable({
  providedIn: 'root'
})
export class VacancyDetailsUpdator {
  private _publishedSubj: Subject<void> = new Subject<void>();
  private _unpublishedSubj: Subject<void> = new Subject<void>();
  private _publishFailedSubj: Subject<Error> = new Subject<Error>();

  public constructor(private _http: HttpClient, private _baseUrlProvider: ApiBaseUrlProvider, private _apiDtoProcessor: ApiDtoProcessor) { }

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
      await this._http.post(this._baseUrlProvider.concatWith("VacancyDetailsUpdator/UpdateHeader"), req).toPromise();
    } catch (e) {
      if (e instanceof HttpErrorResponse) {
        var ex = ex || VacancyIsNotPublishedError.parseFromRes(e);
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
      await this._http.post(this._baseUrlProvider.concatWith("VacancyDetailsUpdator/UpdateMainInfo"), req).toPromise();
    } catch (e) {
      if (e instanceof HttpErrorResponse) {
        var ex = ex || VacancyIsNotPublishedError.parseFromRes(e);
        if (ex) {
          this._publishFailedSubj.next(ex);
          throw ex;
        }
        throw e;
      }
    }
  }

  public async updateExtraInfo(req: UpdateExtraInfoReq): Promise<void> {
    try {
      await this._http.post(this._baseUrlProvider.concatWith("VacancyDetailsUpdator/UpdateExtraInfo"), req).toPromise();
    } catch (e) {
      if (e instanceof HttpErrorResponse) {
        var ex = ex || VacancyIsNotPublishedError.parseFromRes(e);
        if (ex) {
          this._publishFailedSubj.next(ex);
          throw ex;
        }
        throw e;
      }
    }
  }

  public async updateAboutVacancy(req: UpdateAboutMeReq): Promise<void> {
    try {
      await this._http.post(this._baseUrlProvider.concatWith("VacancyDetailsUpdator/UpdateAboutVacancy"), req).toPromise();
    } catch (e) {
      if (e instanceof HttpErrorResponse) {
        var ex = ex || VacancyIsNotPublishedError.parseFromRes(e);
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
      await this._http.post(this._baseUrlProvider.concatWith("VacancyDetailsUpdator/Publish"), req).toPromise();
      this._publishedSubj.next(null);
    } catch (e) {
      if (e instanceof HttpErrorResponse) {
        var ex = ex || VacancyIsNotPublishedError.parseFromRes(e);
        if (ex) {
          this._publishFailedSubj.next(ex);
          throw ex;
        }
        throw e;
      }
    }
  }

  public async unpublish(req: PublishReq): Promise<void> {
    await this._http.post(this._baseUrlProvider.concatWith("VacancyDetailsUpdator/Unpublish"), req).toPromise();
    this._unpublishedSubj.next(null);
  }

  public async makePublic(req: PublishReq): Promise<void> {
    await this._http.post(this._baseUrlProvider.concatWith("VacancyDetailsUpdator/MakePublic"), req).toPromise();
  }

  public async makePrivate(req: PublishReq): Promise<void> {
    await this._http.post(this._baseUrlProvider.concatWith("VacancyDetailsUpdator/MakePrivate"), req).toPromise();
  }

  public async delete(req: PublishReq): Promise<void> {
    await this._http.post(this._baseUrlProvider.concatWith("VacancyDetailsUpdator/Delete"), req).toPromise();
  }
}
