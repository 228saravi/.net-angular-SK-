import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';

import { DxPopoverComponent } from 'devextreme-angular';

import { Observable, Subject, BehaviorSubject, from, combineLatest } from 'rxjs';
import { map, flatMap, debounce, debounceTime, tap, takeUntil, throttleTime, auditTime } from 'rxjs/operators';

import { VacanciesFilterData } from '../sk-vacancies-filter/sk-vacancies-filter.component';

import { ConnectionStatuses } from '../../_classes/sk-domain-connection-statuses';
import { VacanciesSearcher, Req, Vacancy, Res, SkillsGroup } from '../../_services/sk-domain-vacancies-searcher.service';
import { Security, CurrentUserDataRes } from '../../_services/sk-security.service';
import { ConnectionsManager, AmountIsFullError, AlreadyConnectedError } from '../../_services/sk-connections-manager.service';
import { SkPublishBeforeConnectPopupComponent } from '../sk-publish-before-connect-popup/sk-publish-before-connect-popup.component';
import { ProfileIsNotPublishedError } from '../../_services/sk-domain-expert-profile-details-updator.service';



class Vm {
  private _res: Res;
  private _currentUserData: CurrentUserDataRes;

  public constructor(res: Res, currentUserData: CurrentUserDataRes) {
    this._res = res;
    this._currentUserData = currentUserData;
  }

  public get res(): Res {
    return this._res;
  }

  public get canCurrentUserConnecting(): boolean {
    return !this._currentUserData || this._currentUserData.expertProfile != null && this._currentUserData.expertProfile.id != null;
  }

  public resetRes(): void {
    this._res = null;
  }

  public updateRes(res: Res): void {
    this._res = res;
  }
}

@Component({
  selector: 'sk-vacancies-search-page',
  templateUrl: './sk-vacancies-search-page.component.html',
  styleUrls: ['./sk-vacancies-search-page.component.scss']
})
export class SkVacanciesSearchPageComponent implements OnInit, OnDestroy {
  ConnectionStatuses = ConnectionStatuses; // Enum for template.

  @ViewChild("popover")
  public popover: DxPopoverComponent;

  @ViewChild("publishBeforeConnectPopup")
  public publishBeforeConnectPopup: SkPublishBeforeConnectPopupComponent;

  private _router: Router;
  private _activatedRoute: ActivatedRoute;

  private _security: Security;
  private _vacanciesSearcher: VacanciesSearcher;
  private _connectionsManager: ConnectionsManager;

  private _pageSize: number = 20;

  private _updatedSubj: Subject<void> = new BehaviorSubject<void>(null);

  private _vm: Vm = null;

  private _disposedSubj: Subject<void> = new Subject();

  private createReq(paramsMap: ParamMap): Req {
    var vacanciesFilterData = VacanciesFilterData.fromParamMap(paramsMap);
    var req = new Req(
      null,
      vacanciesFilterData.cityId,
      vacanciesFilterData.minRatePerHour,
      vacanciesFilterData.specialitiesSelectionData.selectedSpecialitiesIds,
      vacanciesFilterData.specialitiesSelectionData.selectedSpecializationsIds,
      vacanciesFilterData.specialitiesSelectionData.selectedSkillsIds,
      this._pageSize,
    );
    return req;
  }

  public constructor(router: Router, activatedRoute: ActivatedRoute, searcher: VacanciesSearcher, connectionsManager: ConnectionsManager, security: Security) {
    this._router = router;
    this._activatedRoute = activatedRoute;

    this._security = security;
    this._vacanciesSearcher = searcher;
    this._connectionsManager = connectionsManager;
  }

  public get vm(): Vm {
    return this._vm;
  }

  public async ngOnInit(): Promise<void> {

    combineLatest([
      combineLatest([
        this._activatedRoute.queryParamMap.pipe(
          tap(() => { this._vm = null; }),
          map(paramMap => this.createReq(paramMap))
        ),
        this._updatedSubj,
      ]).pipe(flatMap(([req]: [Req, any]) => this._vacanciesSearcher.search(req))),
      from(this._security.currentUserData$)
    ])
      .pipe(takeUntil(this._disposedSubj))
      .subscribe(([res, currentUserData, x]: [Res, CurrentUserDataRes, any]) => {
        this._vm = new Vm(res, currentUserData);
      });

  }

  public async ngOnDestroy(): Promise<void> {
    this._disposedSubj.next(null);
    this._disposedSubj.complete();
  }

  public async loadMore(): Promise<void> {
    var req = this.createReq(this._activatedRoute.snapshot.queryParamMap);
    req.skip = this._vm.res.foundVacancies.length;
    req.take = this._pageSize;
    var newRes = await this._vacanciesSearcher.search(req);
    newRes.foundVacancies = [...this._vm.res.foundVacancies, ...newRes.foundVacancies];
    this._vm.updateRes(newRes);
  }

  public async connect(vacancyId: number): Promise<void> {
    if (!(await this._security.enshureSignedIn())) {
      return Promise.resolve(null);
    }

    //if (this._security.currentUserData && this._security.currentUserData.expertProfile && !this._security.currentUserData.expertProfile.isPublished) {
    //  await this.publishBeforeConnectPopup.show();
    //  return;
    //}

    try {
      await this._connectionsManager.registerExpertToVacancyConnection({
        expertProfileId: this._security.currentUserData.expertProfile.id,
        vacancyId: vacancyId
      });
    } catch (e) {
      if (e instanceof AmountIsFullError || e instanceof AlreadyConnectedError || e instanceof ProfileIsNotPublishedError) {
      }
      else {
        throw e;
      }
    }

    this._updatedSubj.next();
  }

  public message(connectionId: number): void {
    this._router.navigate(["/dialogs"], { queryParams: { connectionId: connectionId } });
  }
}
