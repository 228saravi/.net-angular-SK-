import { Component, ViewChild, AfterViewInit, ElementRef, ViewEncapsulation, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router, ActivationStart } from '@angular/router';
import { Meta } from '@angular/platform-browser';

import { DxToastComponent } from 'devextreme-angular';
import { SkPublishCheckListPopupComponent, CheckData } from './sk-common/sk-publish-check-list-popup/sk-publish-check-list-popup.component';

import notify from 'devextreme/ui/notify';

import { Security, UserNotFoundError, EmailAlreadyUsedError } from './_services/sk-security.service';
import { ConnectionsManager, AmountIsFullError, AlreadyConnectedError, TooLateToCancelConnectionError } from './_services/sk-connections-manager.service';
import { ExpertProfileDetailsUpdator, ProfileIsNotPublishedError } from './_services/sk-domain-expert-profile-details-updator.service';
import { filter, map, takeUntil } from 'rxjs/operators';
import { SkExpertsSearchPageComponent } from './sk-common/sk-experts-search-page/sk-experts-search-page.component';
import { SkExpertProfileDetailsPageComponent } from './sk-common/sk-expert-profile-details-page/sk-expert-profile-details-page.component';
import { CompanyDetailsUpdator, CompanyIsNotPublishedError } from './_services/sk-domain-company-details-updator.service';
import { EventDetailsProvider } from './_services/sk-domain-event-details-provider.service';
import { EventDetailsUpdator, EventIsNotPublishedError } from './_services/sk-domain-event-details-updator.service';
import { VacancyDetailsUpdator, VacancyIsNotPublishedError } from './_services/sk-domain-vacancy-details-updator.service';
import { Subject } from 'rxjs';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class AppComponent implements AfterViewInit, OnDestroy {

  @ViewChild("toast") toast: DxToastComponent;
  @ViewChild("publishCheckListPopup") publishCheckListPopup: SkPublishCheckListPopupComponent;

  private _disposedSubj: Subject<void> = new Subject();

  title = 'app';

  data: any;

  private success(message: string): void {
    notify({ message: message, position: { my: 'bottom left', at: 'bottom left', of: window, offset: '9 -9' }, }, "success");
  }

  private error(message: string): void {
    notify({ message: message, position: { my: 'bottom left', at: 'bottom left', of: window, offset: '9 -9' }, }, "error");
  }

  private connectToSecurity(): void {
    this._security.signingInFailed$
      .pipe(takeUntil(this._disposedSubj))
      .subscribe(() => {
        this.error("Неудачная попытка входа!");
      });

    this._security.passwordResetInitiated$
      .pipe(takeUntil(this._disposedSubj))
      .subscribe(() => {
        this.success("Ссылка для смены пароля отправлена! Проверьте Вашу почту!");
      });

    this._security.passwordResetFinished$
      .pipe(takeUntil(this._disposedSubj))
      .subscribe(() => {
        this.success("Пароль изменён! Войдите в систему!");
      });

    this._security.initPasswordResetFailed
      .pipe(takeUntil(this._disposedSubj))
      .subscribe((e) => {
        if (e instanceof UserNotFoundError) {
          this.error("Пользователь не найден!");
        } else {
          throw e;
        }
      });

    this._security.expertRegistrationFailed$
      .pipe(takeUntil(this._disposedSubj))
      .subscribe((e) => {
        if (e instanceof EmailAlreadyUsedError) {
          this.error("Этот email уже используется!");
          return;
        }

        this.error("Не удалось зарегистрироваться!");
      });
  }

  private connectToExpertProfileDetailsUpdator(): void {
    this._expertProfileUpdator.published$
      .pipe(takeUntil(this._disposedSubj))
      .subscribe(() => {
        this.success("Профиль опубликован!");
      })

    this._expertProfileUpdator.publishFailed$
      .pipe(takeUntil(this._disposedSubj))
      .subscribe(e => {
        if (e instanceof ProfileIsNotPublishedError) {
          this.publishCheckListPopup.show(CheckData.fromProfileIsNotPreparedForPublishError(e));
        }
      });
  }

  private connectToCompanyDetailsUpdator(): void {
    this._companyDetailsUpdator.published$
      .pipe(takeUntil(this._disposedSubj))
      .subscribe(() => {
        this.success("Компания опубликована!");
      })

    this._companyDetailsUpdator.publishFailed$
      .pipe(takeUntil(this._disposedSubj))
      .subscribe((e) => {
        if (e instanceof CompanyIsNotPublishedError) {
          this.publishCheckListPopup.show(CheckData.fromCompanyIsNotPreparedForPublishError(e));
        }
      });
  }

  private connectToEventDetailsUpdator(): void {
    this._eventDetailsUpdator.published$
      .pipe(takeUntil(this._disposedSubj))
      .subscribe(() => {
        this.success("Мероприятие опубликовано!");
      });

    this._eventDetailsUpdator.publishFailed$
      .pipe(takeUntil(this._disposedSubj))
      .subscribe((e) => {
        if (e instanceof EventIsNotPublishedError) {
          this.publishCheckListPopup.show(CheckData.fromEventIsNotPreparedForPublishError(e));
          return;
        }

        throw e;
      });

    this._eventDetailsUpdator.unpublished$
      .pipe(takeUntil(this._disposedSubj))
      .subscribe(() => {
        this.success("Мероприятие снято с публикации!");
      });
  }

  private connectToVacancyDetailsUpdator(): void {
    this._vacancyDetailsUpdator.published$
      .pipe(takeUntil(this._disposedSubj))
      .subscribe(() => {
        this.success("Вакансия опубликована!");
      });

    this._vacancyDetailsUpdator.publishFailed$
      .pipe(takeUntil(this._disposedSubj))
      .subscribe((e) => {
        if (e instanceof VacancyIsNotPublishedError) {
          this.publishCheckListPopup.show(CheckData.fromVacancyIsNotPreparedForPublishError(e));
          return;
        }

        throw e;
      });

    this._vacancyDetailsUpdator.unpublished$
      .pipe(takeUntil(this._disposedSubj))
      .subscribe(() => {
        this.success("Вакансия снята с публикации!");
      });
  }

  private connectToConnectionsManager(): void {
    this._connectionsManager.connectionRegistered$
      .pipe(takeUntil(this._disposedSubj))
      .subscribe(() => {
        this.success("Отклик отправлен!");
      });

    this._connectionsManager.connectionRegistrationFailed$
      .pipe(takeUntil(this._disposedSubj))
      .subscribe((e) => {
        if (e instanceof AmountIsFullError) {
          this.error("Все места заняты!");
          return;
        }

        if (e instanceof AlreadyConnectedError) {
          this.error("Отклик уже зарегистрирован");
          return;
        }

        if (e instanceof CompanyIsNotPublishedError) {
          this.publishCheckListPopup.show(CheckData.fromCompanyIsNotPreparedForPublishError(e));
          return;
        }

        if (e instanceof ProfileIsNotPublishedError) {
          this.publishCheckListPopup.show(CheckData.fromProfileIsNotPreparedForPublishError(e));
          return;
        }

        throw e;
      });

    this._connectionsManager.connectionCanceled$
      .pipe(takeUntil(this._disposedSubj))
      .subscribe(() => {
        this.success("Отклик отозван!");
      });

    this._connectionsManager.connectionCancelingFailed$
      .pipe(takeUntil(this._disposedSubj))
      .subscribe((e) => {
        if (e instanceof TooLateToCancelConnectionError) {
          this.error("Слишком поздно, чтобы отзывать отклик!");
          return;
        }

        throw e;
      });

    this._connectionsManager.connectionApproved$
      .pipe(takeUntil(this._disposedSubj))
      .subscribe(() => {
        this.success("Отклик принят!");
      });

    this._connectionsManager.connectionApprovingFailed$
      .pipe(takeUntil(this._disposedSubj))
      .subscribe((e) => {
        throw e;
      });
  }

  public constructor(
    private _elementRef: ElementRef,
    private _security: Security,
    private _expertProfileUpdator: ExpertProfileDetailsUpdator,
    private _companyDetailsUpdator: CompanyDetailsUpdator,
    private _eventDetailsUpdator: EventDetailsUpdator,
    private _vacancyDetailsUpdator: VacancyDetailsUpdator,
    private _connectionsManager: ConnectionsManager,
  ) {
    this.connectToSecurity();
    this.connectToExpertProfileDetailsUpdator();
    this.connectToCompanyDetailsUpdator();
    this.connectToEventDetailsUpdator();
    this.connectToVacancyDetailsUpdator();
    this.connectToConnectionsManager();
  }

  public ngAfterViewInit(): void { }

  public ngOnDestroy(): void {
    this._disposedSubj.next(null);
    this._disposedSubj.complete();
  }
}
