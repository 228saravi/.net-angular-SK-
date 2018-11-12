import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpErrorResponse } from '@angular/common/http';

import { map } from 'rxjs/operators';

import { ApiBaseUrlProvider } from './sk-api-base-url-provider.service';

import { HttpTransferableError } from '../_classes/http-transferable-error';
import { Subject, Observable } from 'rxjs';

export class UpdateHeaderReq {
  public companyId: number;
  public name: string;
  public cityId: string;
}

export class UpdateAboutCompanyReq {
  public companyId: number;
  public aboutCompanyHtml: string;
}

export class PublishReq {
  public companyId: number;
}

export class RegisterEventReq {
  public companyId: number;
}

export class RegisterEventRes {
  public eventId: number;
}

export class CompanyCheckBeforePublish {
  public nameSet: boolean;
  public logoSet: boolean;
  public citySet: boolean;
  public isReadyForPublish: boolean;
}

export class CompanyIsNotPublishedError extends HttpTransferableError {
  private _companyCheck: CompanyCheckBeforePublish;

  private constructor(profileCheck: CompanyCheckBeforePublish) {
    super();

    this._companyCheck = profileCheck;
  }

  public get companyCheck(): CompanyCheckBeforePublish {
    return this._companyCheck;
  }

  public static parseFromRes(res: HttpErrorResponse): CompanyIsNotPublishedError | null {
    return HttpTransferableError.parseFromResInternal(res, "COMPANY_IS_NOT_PUBLISHED", (extraData) => new CompanyIsNotPublishedError(extraData.companyCheck));
  }
}

@Injectable({
  providedIn: 'root'
})
export class CompanyDetailsUpdator {
  private _updatedSubj: Subject<void> = new Subject<void>();
  private _updatedFailedSubj: Subject<Error> = new Subject<Error>();

  private _publishedSubj: Subject<void> = new Subject<void>();
  private _publishFailedSubj: Subject<Error> = new Subject<Error>();

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
      await this._http.post(this._baseUrlProvider.concatWith("CompanyDetailsUpdator/UpdateHeader"), req).toPromise();
      this._updatedSubj.next(null);
    } catch (e) {
      if (e instanceof HttpErrorResponse) {
        var ex = ex || CompanyIsNotPublishedError.parseFromRes(e);
        if (ex) {
          this._publishFailedSubj.next(ex);
          throw ex;
        }
        throw e;
      }
    }
  }

  public async updateAboutCompany(req: UpdateAboutCompanyReq): Promise<void> {
    try {
      await this._http.post(this._baseUrlProvider.concatWith("CompanyDetailsUpdator/UpdateAboutCompany"), req).toPromise();
      this._updatedSubj.next(null);
    } catch (e) {
      if (e instanceof HttpErrorResponse) {
        var ex = ex || CompanyIsNotPublishedError.parseFromRes(e);
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
      await this._http.post(this._baseUrlProvider.concatWith("CompanyDetailsUpdator/Publish"), req).toPromise();
      this._publishedSubj.next(null);
    } catch (e) {
      if (e instanceof HttpErrorResponse) {
        var ex = ex || CompanyIsNotPublishedError.parseFromRes(e);
        if (ex) {
          this._publishFailedSubj.next(ex);
          throw ex;
        }
        throw e;
      }
    }
  }

  public async registerEvent(req: RegisterEventReq): Promise<RegisterEventRes> {
    try {
      return await this._http.post(this._baseUrlProvider.concatWith("CompanyDetailsUpdator/RegisterEvent"), req)
        .pipe(map(res => res as RegisterEventRes))
        .toPromise();
    } catch (e) {
      if (e instanceof HttpErrorResponse) {
        var ex = ex || CompanyIsNotPublishedError.parseFromRes(e);
        if (ex) {
          this._publishFailedSubj.next(ex);
          throw ex;
        }
        throw e;
      }
    }
  }
}
