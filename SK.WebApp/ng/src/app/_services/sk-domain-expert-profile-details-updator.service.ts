import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpErrorResponse } from '@angular/common/http';

import { map } from 'rxjs/operators';

import { ApiBaseUrlProvider } from './sk-api-base-url-provider.service';

import { HttpTransferableError } from '../_classes/http-transferable-error';
import { Subject, Observable } from 'rxjs';

export class UpdateHeaderReq {
  public expertProfileId: number;
  public name: string;
  public ratePerHour: number;
  public specialityId: string;
  public specializationId: string;
}

export class UpdateMainInfoReq {
  public expertProfileId: number;
  public cityId: string;
  public experienceOptionId: string;
  public languagesIds: string[];
  public skillsIds: string[];
}

export class UpdateExtraInfoReq {
  public expertProfileId: number;
  public clothingSizeId: string;
  public expertDocumentsIds: string[];
}

export class UpdateAboutMeReq {
  public expertProfileId: number;
  public aboutMeHtml: string;
}

export class PublishReq {
  public expertProfileId: number;
}

export class ProfileCheckBeforePublish {
  public nameSet: boolean;
  public photoSet: boolean;
  public citySet: boolean;
  public ratePerHourSet: boolean;
  public specialitySet: boolean;
  public specializationSet: boolean;
  public experienceSet: boolean;
  public isReadyForPublish: boolean;
}

export class ProfileIsNotPublishedError extends HttpTransferableError {
  private _profileCheck: ProfileCheckBeforePublish;

  private constructor(profileCheck: ProfileCheckBeforePublish) {
    super();

    this._profileCheck = profileCheck;
  }

  public get profileCheck(): ProfileCheckBeforePublish {
    return this._profileCheck;
  }

  public static parseFromRes(res: HttpErrorResponse): ProfileIsNotPublishedError | null {
    return HttpTransferableError.parseFromResInternal(res, "PROFILE_IS_NOT_PUBLISHED", (extraData) => new ProfileIsNotPublishedError(extraData.profileCheck));
  }
}

@Injectable({
  providedIn: 'root'
})
export class ExpertProfileDetailsUpdator {
  private _updatedSubj: Subject<void> = new Subject<void>();
  private _updatedFailedSubj: Subject<Error> = new Subject<Error>();

  private _publishFailedSubj: Subject<Error> = new Subject<Error>();
  private _publishedSubj: Subject<void> = new Subject<void>();

  public constructor(private _http: HttpClient, private _baseUrlProvider: ApiBaseUrlProvider) { }

  public get updated$(): Observable<void> {
    return this._updatedSubj;
  }

  public get updatedFailed$(): Observable<Error> {
    return this._updatedFailedSubj;
  }

  public get published$(): Observable<void> {
    return this._publishedSubj;
  }

  public get publishFailed$(): Observable<Error> {
    return this._publishFailedSubj;
  }

  public async updateHeader(req: UpdateHeaderReq): Promise<void> {
    try {
      await this._http.post(this._baseUrlProvider.concatWith("ExpertProfileDetailsUpdator/UpdateHeader"), req).toPromise();
      this._updatedSubj.next(null);
    } catch (e) {
      if (e instanceof HttpErrorResponse) {
        var ex = ex || ProfileIsNotPublishedError.parseFromRes(e);
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
      await this._http.post(this._baseUrlProvider.concatWith("ExpertProfileDetailsUpdator/UpdateMainInfo"), req).toPromise();
      this._updatedSubj.next(null);
    } catch (e) {
      if (e instanceof HttpErrorResponse) {
        var ex = ex || ProfileIsNotPublishedError.parseFromRes(e);
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
      await this._http.post(this._baseUrlProvider.concatWith("ExpertProfileDetailsUpdator/UpdateExtraInfo"), req).toPromise();
      this._updatedSubj.next(null);
    } catch (e) {
      if (e instanceof HttpErrorResponse) {
        var ex = ex || ProfileIsNotPublishedError.parseFromRes(e);
        if (ex) {
          this._publishFailedSubj.next(ex);
          throw ex;
        }
        throw e;
      }
    }
  }

  public async updateAboutMe(req: UpdateAboutMeReq): Promise<void> {
    try {
      await this._http.post(this._baseUrlProvider.concatWith("ExpertProfileDetailsUpdator/UpdateAboutMe"), req).toPromise();
      this._updatedSubj.next(null);
    } catch (e) {
      if (e instanceof HttpErrorResponse) {
        var ex = ex || ProfileIsNotPublishedError.parseFromRes(e);
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
      await this._http.post(this._baseUrlProvider.concatWith("ExpertProfileDetailsUpdator/Publish"), req).toPromise();
      this._publishedSubj.next(null);
    } catch (e) {
      if (e instanceof HttpErrorResponse) {
        var ex = ex || ProfileIsNotPublishedError.parseFromRes(e);
        if (ex) {
          this._publishFailedSubj.next(ex);
          throw ex;
        }
        throw e;
      }
    }
  }

  public async unpublish(req: PublishReq): Promise<void> {
    try {
      await this._http.post(this._baseUrlProvider.concatWith("ExpertProfileDetailsUpdator/Unpublish"), req).toPromise();
      this._updatedSubj.next(null);
    } catch (e) {
    }
  }
}
