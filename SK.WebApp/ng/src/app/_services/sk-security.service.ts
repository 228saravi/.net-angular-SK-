import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpErrorResponse } from '@angular/common/http';
import { Meta } from '@angular/platform-browser';

import { SkSignInPopupComponent } from '../sk-common/sk-sign-in-popup/sk-sign-in-popup.component';

import { Observable, Subscriber, BehaviorSubject, merge, of, Subject } from 'rxjs';
import { map, switchMap, catchError, flatMap, take, tap, withLatestFrom, publish, publishReplay, refCount, share, shareReplay } from 'rxjs/operators';

import { ApiBaseUrlProvider } from './sk-api-base-url-provider.service';
import { HttpTransferableError } from '../_classes/http-transferable-error';
import { CompanyDetailsUpdator } from './sk-domain-company-details-updator.service';
import { ExpertProfileDetailsProvider } from './sk-domain-expert-profile-details-provider.service';
import { ExpertProfileDetailsUpdator } from './sk-domain-expert-profile-details-updator.service';

export class RegisterExpertReq {
  public email: string;
  public password: string;
  public name: string;
}

export class RegisterCompanyReq {
  public email: string;
  public password: string;
  public name: string;
  public companyName: string;
}

export class InitPasswordResetReq {
  public email: string;
}

export class FinishPasswordResetReq {
  public email: string;
  public token: string;
  public newPassword: string;
}

export class SignInReq {
  public email: string;
  public password: string;
}

export class ExpertProfileRes {
  public id: number;
  public isPublished: boolean;
  public thumbnailImageUrl: string;
}

export class CompanyRes {
  public id: number;
  public name: string;
  public isPublished: boolean;
  public thumbnailImageUrl: string;
}

export class CurrentUserDataRes {
  public email: string;
  public displayName: string;
  public expertProfile: ExpertProfileRes;
  public company: CompanyRes;
}

export class EmailAlreadyUsedError extends HttpTransferableError {
  public static parseFromRes(res: HttpErrorResponse): EmailAlreadyUsedError | null {
    return HttpTransferableError.parseFromResInternal(res, "EMAIL_ALREADY_USED", (extraData) => new EmailAlreadyUsedError());
  }
}

export class WrongEmailOrPasswordError extends HttpTransferableError {
  public static parseFromRes(res: HttpErrorResponse): WrongEmailOrPasswordError | null {
    return HttpTransferableError.parseFromResInternal(res, "WRONG_EMAIL_OR_PASSWORD", () => new WrongEmailOrPasswordError());
  }
}

export class UserNotFoundError extends HttpTransferableError {
  public static parseFromRes(res: HttpErrorResponse): UserNotFoundError | null {
    return HttpTransferableError.parseFromResInternal(res, "USER_NOT_FOUND", () => new UserNotFoundError());
  }
}

export function createSecurityInitializer(sec: Security) {
  return async () => {
    await sec.init();
  }
}

@Injectable({
  providedIn: 'root'
})
export class Security {
  private _signInPopup: SkSignInPopupComponent = null;
  private _currentUserData: CurrentUserDataRes = null;
  private _currentUserData$: Observable<CurrentUserDataRes> = null;
  private _currentUserDataShared$: Observable<CurrentUserDataRes> = null;

  private _userSignedInSubj: BehaviorSubject<void> = new BehaviorSubject<void>(null);
  private _signingInFailedSubj: Subject<Error> = new Subject<Error>();
  private _passwordResetInitiated: Subject<void> = new Subject<void>();
  private _passwordResetFinished: Subject<void> = new Subject<void>();
  private _initPasswordResetFailedSubj: Subject<Error> = new Subject<Error>();
  private _expertRegistrationFailedSubj: Subject<Error> = new Subject<Error>();

  public constructor(
    private _http: HttpClient,
    private _meta: Meta,
    private _baseUrlProvider: ApiBaseUrlProvider,
    private _companyDetailsUpdator: CompanyDetailsUpdator,
    private _expertProfileDetailsUpdator: ExpertProfileDetailsUpdator
  ) {

    this._currentUserData$ = merge(
      this._userSignedInSubj,
      this._companyDetailsUpdator.updated$,
      this._companyDetailsUpdator.published$,
      this._expertProfileDetailsUpdator.updated$,
      this._expertProfileDetailsUpdator.published$
    ).pipe(
      map(() => { }),
      flatMap(() => this._http.get(this._baseUrlProvider.concatWith("Account/GetCurrentUserData"))
        .pipe(
          map(res => res as CurrentUserDataRes),
          catchError(() => of(null)),
          tap(data => {
            this._currentUserData = data;

            var rnd = Math.random();

            if (this._currentUserData && this._currentUserData.expertProfile && this._currentUserData.expertProfile.thumbnailImageUrl) {
              this._currentUserData.expertProfile.thumbnailImageUrl += `?rnd=${rnd}`;
            }

            if (this.currentUserData && this._currentUserData.company && this._currentUserData.company.thumbnailImageUrl) {
              this._currentUserData.company.thumbnailImageUrl += `?rnd=${rnd}`;
            }

          })
        )
      ));

    this._currentUserDataShared$ = this._currentUserData$.pipe(shareReplay(1));
  }

  public get signInPopup(): SkSignInPopupComponent {
    return this._signInPopup;
  }

  public get currentUserData(): CurrentUserDataRes {
    return this._currentUserData;
  }

  public get currentUserData$(): Observable<CurrentUserDataRes> {
    return this._currentUserDataShared$;
  }

  public get signingInFailed$(): Observable<Error> {
    return this._signingInFailedSubj;
  }

  public get passwordResetInitiated$(): Observable<void> {
    return this._passwordResetInitiated;
  }

  public get passwordResetFinished$(): Observable<void> {
    return this._passwordResetFinished;
  }

  public get initPasswordResetFailed(): Observable<Error> {
    return this._initPasswordResetFailedSubj;
  }

  public get expertRegistrationFailed$(): Observable<Error> {
    return this._expertRegistrationFailedSubj;
  }

  public async init(): Promise<void> {
    await this.currentUserData$.pipe(take(1)).toPromise();
  }

  public registerSignInPopup(signInPopup: SkSignInPopupComponent): void {
    this._signInPopup = signInPopup;
  }

  public setNoIndex(): void {
    this._meta.addTag({ name: "robots", content: "noindex,nofollow" });
  }

  public removeNoIndex(): void {
    this._meta.removeTag("content='noindex,nofollow'");
  }

  public async registerExpert(req: RegisterExpertReq): Promise<void> {
    try {
      await this._http.post(this._baseUrlProvider.concatWith("Account/RegisterExpert"), req).toPromise();
    } catch (e) {
      if (e instanceof HttpErrorResponse) {
        var ex = EmailAlreadyUsedError.parseFromRes(e);
        if (ex) {
          this._expertRegistrationFailedSubj.next(ex);
          throw ex;
        }
      }
      throw e;
    }
  }

  public async registerCompany(req: RegisterCompanyReq): Promise<void> {
    try {
      await this._http.post(this._baseUrlProvider.concatWith("Account/RegisterCompany"), req).toPromise();
    } catch (e) {
      if (e instanceof HttpErrorResponse) {
        var ex = EmailAlreadyUsedError.parseFromRes(e);
        if (ex) {
          this._expertRegistrationFailedSubj.next(ex);
          throw ex;
        }
      }
      throw e;
    }
  }

  public async initPasswordReset(req: InitPasswordResetReq): Promise<void> {
    try {
      await this._http.post(this._baseUrlProvider.concatWith("Account/InitPasswordReset"), req).toPromise();
      this._passwordResetInitiated.next();
    } catch (error) {
      if (error instanceof HttpErrorResponse) {
        var e = e || UserNotFoundError.parseFromRes(error);
        if (e != null) {
          this._initPasswordResetFailedSubj.next(e);
          throw e;
        }
      }

      throw error;
    }
  }

  public async finishPasswordReset(req: FinishPasswordResetReq): Promise<void> {
    try {
      await this._http.post(this._baseUrlProvider.concatWith("Account/FinishPasswordReset"), req).pipe(
        map(res => res),
        catchError(e => of(null))
      ).toPromise();

      this._passwordResetFinished.next();
    } catch (error) {
      throw error;
    }
  }

  public async signIn(req: SignInReq): Promise<void> {
    try {

      await this._http.post(this._baseUrlProvider.concatWith("Account/SignIn"), req).toPromise();

      this._userSignedInSubj.next(null);
      await this._currentUserData$.pipe(take(1)).toPromise();

      //var currentUserData = await this._currentUserData$.pipe(take(1)).toPromise();

      //if (!currentUserData) {
      //  var a = currentUserData;
      //}

    } catch (e) {
      if (e instanceof HttpErrorResponse) {
        var ex = WrongEmailOrPasswordError.parseFromRes(e);
        if (ex != null) {
          this._signingInFailedSubj.next(ex);
          throw ex;
        }
      }
      throw e;
    }
  }

  public async enshureSignedIn(): Promise<boolean> {
    if (this._currentUserData) {
      return Promise.resolve(true);
    } else if (this._signInPopup) {
      return this._signInPopup.enshureSignedIn();
    } else {
      return Promise.resolve(false);
    }
  }

  public async signOut(): Promise<void> {
    await this._http.post(this._baseUrlProvider.concatWith("Account/SignOut"), {}).toPromise();

    this._userSignedInSubj.next(null);
    await this._currentUserData$.pipe(take(1)).toPromise();

    //if (window) {
    //  window.location.reload(true);
    //}
  }
}
