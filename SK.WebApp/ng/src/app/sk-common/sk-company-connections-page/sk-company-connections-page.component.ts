import { Component, OnInit, ViewChild, AfterViewInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

import { Subject, BehaviorSubject, Observable, combineLatest, from, of, Subscription } from 'rxjs';
import { flatMap, filter, map, merge, withLatestFrom, share, shareReplay, tap, takeUntil } from 'rxjs/operators';

import { Security } from '../../_services/sk-security.service';
import { CompanyDetailsProvider, Res as CompanyRes, Company, Event } from '../../_services/sk-domain-company-details-provider.service';
import { VacancyDetailsProvider, Vacancy } from '../../_services/sk-domain-vacancy-details-provider.service';
import { CompanyDetailsUpdator, CompanyIsNotPublishedError } from '../../_services/sk-domain-company-details-updator.service';
import { EventDetailsUpdator, EventIsNotPublishedError } from '../../_services/sk-domain-event-details-updator.service';
import { ConnectionsManager, VacancyConnections, NotYourConnectionError, TooLateToCancelConnectionError } from '../../_services/sk-connections-manager.service';
import { ConnectionStatuses } from '../../_classes/sk-domain-connection-statuses';
import { ConnectionTypes } from '../../_classes/sk-domain-connection-types';
import { VacancyDetailsUpdator } from '../../_services/sk-domain-vacancy-details-updator.service';
import { SkTabsComponent } from '../../sk-ui-kit/sk-tabs/sk-tabs.component';
import { SkFeedbackPopupComponent } from '../sk-feedback-popup/sk-feedback-popup.component';

type EventFilterMode = "Active" | "Archive";
type ConnectionsFilterMode = "Connected" | "Incoming" | "Outgoing";

class FilterVm {

  private _disposedSubj: Subject<void> = new Subject();

  private _router: Router;
  private _activatedRoute: ActivatedRoute;

  private _companyDetailsProvider: CompanyDetailsProvider;
  private _companyDetailsUpdator: CompanyDetailsUpdator;
  private _eventDetailsUpdator: EventDetailsUpdator;
  private _vacancyDetailsUpdator: VacancyDetailsUpdator;

  private _companyId: number = null;

  private _res: CompanyRes;
  private _filterMode: EventFilterMode = null;
  private _filteredEvents: Event[] = [];

  private _selectedEventId: number;
  private _selectedVacancyId: number;

  private _vacancySelectedSubj: Subject<number> = new BehaviorSubject<number>(null);
  private _vacancySelectedShared$: Observable<number> = this._vacancySelectedSubj; //this._vacancySelectedSubj.pipe(shareReplay(1));

  private _now = new Date();

  public constructor(
    router: Router,
    activatedRoute: ActivatedRoute,
    companyDetailsProvider: CompanyDetailsProvider,
    companyDetailsUpdator: CompanyDetailsUpdator,
    eventDetailsUpdator: EventDetailsUpdator,
    vacancyDetailsUpdator: VacancyDetailsUpdator
  ) {
    this._router = router;
    this._activatedRoute = activatedRoute;
    this._companyDetailsProvider = companyDetailsProvider;
    this._companyDetailsUpdator = companyDetailsUpdator;
    this._eventDetailsUpdator = eventDetailsUpdator;
    this._vacancyDetailsUpdator = vacancyDetailsUpdator;
  }

  public get selectedEventId(): number {
    return this._selectedEventId;
  }

  public get selectedVacancyId(): number {
    return this._selectedVacancyId;
  }

  public get vacancySelected$(): Observable<number> {
    return this._vacancySelectedShared$;
  }

  public get company(): Company {
    if (!this._res) {
      return null;
    }

    return this._res.company;
  }

  public get filterMode(): EventFilterMode {
    return this._filterMode;
  }

  public get filteredEvents(): Event[] {
    return this._filteredEvents;
  }

  public async init(companyId: number): Promise<void> {
    this._companyId = companyId;
    await this.refresh();

    this._activatedRoute.queryParamMap
      .pipe(takeUntil(this._disposedSubj))
      .subscribe(queryParamMap => {
        var eventId = queryParamMap.get("eventId");
        this._selectedEventId = eventId ? parseInt(eventId) : null;

        var vacancyId = queryParamMap.get("vacancyId");
        this._selectedVacancyId = vacancyId ? parseInt(vacancyId) : null;

        this._vacancySelectedSubj.next(this._selectedVacancyId);
      });

    //this.selectVacancy(null);
  }

  public async dispose(): Promise<void> {
    this._disposedSubj.next(null);
    this._disposedSubj.complete();
  }

  public async refresh(): Promise<void> {
    if (this._companyId) {
      this._res = await this._companyDetailsProvider.get({ companyId: this._companyId, withVacancies: true });
      this.setFilter(this._filterMode);
    } else {
      this._res = null;
    }
  }

  public setFilter(mode: EventFilterMode): void {
    this._filterMode = mode;

    if (!this._res || !this._filterMode) {
      this._filteredEvents = [];
      return;
    }

    if (this._filterMode == "Active") {
      this._filteredEvents = this._res.company.events.filter(e => e.endTime == null || e.endTime >= this._now);
    }

    if (this._filterMode == "Archive") {
      this._filteredEvents = this._res.company.events.filter(e => e.endTime < this._now);
    }
  }

  public async addEvent(): Promise<void> {
    try {
      var res = await this._companyDetailsUpdator.registerEvent({ companyId: this._res.company.id });
      await this.init(this._res.company.id);
      await this._router.navigate(['/events', res.eventId]);
    } catch (e) {
      if (e instanceof CompanyIsNotPublishedError) {

      } else {
        throw e;
      }
    }
  }

  public async addVacancy(eventId: number): Promise<void> {
    try {
      var res = await this._eventDetailsUpdator.registerVacancy({ eventId: eventId });
      await this.init(this._res.company.id);
      await this._router.navigate(['/vacancies', res.vacancyId]);
    } catch (e) {
      if (e instanceof EventIsNotPublishedError) {

      } else {
        throw e;
      }
    }
  }

  public async editEvent(eventId: number): Promise<void> {
    await this._router.navigate(['/events', eventId]);
    await this.init(this._res.company.id);
  }

  public async deleteEvent(eventId: number): Promise<void> {
    await this._eventDetailsUpdator.delete({ eventId: eventId });
    await this.init(this._res.company.id);
  }

  public async editVacancy(vacancyId: number): Promise<void> {
    await this._router.navigate(['/vacancies', vacancyId]);
    await this.init(this._res.company.id);
  }

  public async deleteVacancy(vacancyId: number): Promise<void> {
    await this._vacancyDetailsUpdator.delete({ vacancyId: vacancyId });
    await this.init(this._res.company.id);
  }

  public selectEvent(eventId: number): void {
    var queryParams = { ...this._activatedRoute.snapshot.queryParams };

    var newQueryParams = {};
    newQueryParams["eventsTab"] = queryParams["eventsTab"];
    newQueryParams["eventId"] = eventId;

    this._router.navigate([], { queryParams: newQueryParams });
  }

  public selectVacancy(vacancyId: number): void {
    var queryParams = { ...this._activatedRoute.snapshot.queryParams };

    var newQueryParams = {};
    newQueryParams["eventsTab"] = queryParams["eventsTab"];
    newQueryParams["eventId"] = queryParams["eventId"];
    newQueryParams["vacancyId"] = vacancyId;
    newQueryParams["connectionsTab"] = 0;

    this._router.navigate([], { queryParams: newQueryParams });
  }
}

class ConnectionsVm {
  private _activatedRoute: ActivatedRoute;
  private _vacancyDetailsProvider: VacancyDetailsProvider;
  private _connectionsManager: ConnectionsManager;
  private _filterVm: FilterVm;
  private _vacancy: Vacancy;
  private _res: VacancyConnections.VacancyConnectionsRes;
  private _connectedConnections: VacancyConnections.Connection[] = [];
  private _incomingConnections: VacancyConnections.Connection[] = [];
  private _outgoingConnections: VacancyConnections.Connection[] = [];
  private _filterMode: ConnectionsFilterMode = null;
  private _filteredConnections: VacancyConnections.Connection[] = [];

  private _now: Date = new Date();

  private _updatedSubj: Subject<void> = new BehaviorSubject<void>(null);
  private _disposedSubj: Subject<void> = new Subject();

  public constructor(
    activatedRoute: ActivatedRoute,
    vacancyDetailsProvider: VacancyDetailsProvider,
    connectionsManager: ConnectionsManager,
    filterVm: FilterVm
  ) {
    this._activatedRoute = activatedRoute;
    this._vacancyDetailsProvider = vacancyDetailsProvider;
    this._connectionsManager = connectionsManager;
    this._filterVm = filterVm;
  }

  public get res(): VacancyConnections.VacancyConnectionsRes {
    return this._res;
  }

  public get vacancy(): Vacancy {
    return this._vacancy;
  }

  public get filterMode(): ConnectionsFilterMode {
    return this._filterMode;
  }

  public get connectedConnections(): VacancyConnections.Connection[] {
    return this._connectedConnections;
  }

  public get incomingConnections(): VacancyConnections.Connection[] {
    return this._incomingConnections;
  }

  public get outgoingConnections(): VacancyConnections.Connection[] {
    return this._outgoingConnections;
  }

  public get filteredConnections(): VacancyConnections.Connection[] {
    return this._filteredConnections;
  }

  public async init(): Promise<void> {

    var triggers = combineLatest(
      this._activatedRoute.queryParamMap.pipe(map(queryParamsMap => queryParamsMap.has("vacancyId") ? queryParamsMap.get("vacancyId") : null)),
      this._connectionsManager.connectionApproved$.pipe(merge(of(null))),
      this._connectionsManager.connectionCanceled$.pipe(merge(of(null))),
      this._connectionsManager.connectionRegistered$.pipe(merge(of(null))),
      this._connectionsManager.feedbackPosted$.pipe(merge(of(null))),
    ).pipe(
      map(([vacancyId]: any) => vacancyId)
    );

    triggers.pipe(
      flatMap((vacancyId: number) => combineLatest(
        vacancyId ? from(this._vacancyDetailsProvider.get({ vacancyId: vacancyId })) : of(null),
        vacancyId ? from(this._connectionsManager.getVacancyConnections({ vacancyId: vacancyId })) : of(null)
      )),
      takeUntil(this._disposedSubj)
    ).subscribe(([vac, conn]) => {
      this._vacancy = vac ? vac.foundVacancy : null;
      this._res = conn ? conn : null;

      if (this._res) {
        this._connectedConnections = this._res.connections
          .filter(c => c.status == ConnectionStatuses.connected);

        this._incomingConnections = this._res.connections
          .filter(c => c.status == ConnectionStatuses.initiated)
          .filter(c => c.type == ConnectionTypes.expertToVacancy);

        this._outgoingConnections = this._res.connections
          .filter(c => c.status == ConnectionStatuses.initiated)
          .filter(c => c.type == ConnectionTypes.vacancyToExpert);
      }

      this.setFilter(this._filterMode);;
    });

  }

  public async dispose(): Promise<void> {
    this._disposedSubj.next(null);
    this._disposedSubj.complete();
  }

  public setFilter(mode: ConnectionsFilterMode): void {
    this._filterMode = mode;

    if (!this._res || !this._filterMode) {
      this._filteredConnections = [];
      return;
    }

    if (this._filterMode == "Connected") {
      this._filteredConnections = this._connectedConnections;
    }

    if (this._filterMode == "Incoming") {
      this._filteredConnections = this._incomingConnections;
    }

    if (this._filterMode == "Outgoing") {
      this._filteredConnections = this._outgoingConnections;
    }
  }


  public isConnectionCancelable(connection: VacancyConnections.Connection): boolean {
    if (!this._vacancy.startTime) {
      return true;
    }

    if (connection.status == ConnectionStatuses.initiated) {
      return true;
    }

    var distanceToStart = this._vacancy.startTime.valueOf() - this._now.valueOf();

    return distanceToStart > 86400 /*one day in miliseconds*/ &&
      connection.status != ConnectionStatuses.canceled;
  }

  public isConnectionApprovable(connection: VacancyConnections.Connection): boolean {
    return connection.type == ConnectionTypes.expertToVacancy &&
      connection.status == ConnectionStatuses.initiated &&
      this._vacancy.startTime &&
      this._vacancy.startTime.valueOf() > this._now.valueOf();
  }

  public isConnectionFeedbackable(connection: VacancyConnections.Connection): boolean {
    return connection.status == ConnectionStatuses.connected &&
      this._vacancy.event.endTime &&
      this._now.valueOf() > this._vacancy.event.endTime.valueOf() &&
      connection.feedbackForExpert == null;
  }

  public doesConnectionAllowMessaging(connection: VacancyConnections.Connection): boolean {
    return connection.status == ConnectionStatuses.connected &&
      this._vacancy.event.endTime &&
      this._vacancy.event.endTime.valueOf() > this._now.valueOf();
  }


  public async approveConnection(id: number): Promise<void> {
    try {
      await this._connectionsManager.approveConnection({ connectionId: id });
      await this._filterVm.refresh();
    } catch (e) {
      throw e;
    }
  }

  public async cancelConnection(id: number): Promise<void> {
    try {
      await this._connectionsManager.cancelConnection({ connectionId: id });
      await this._filterVm.refresh();
    } catch (e) {
      if (e instanceof NotYourConnectionError || e instanceof TooLateToCancelConnectionError) {
      } else {
        throw e;
      }
    }
  }

}

class Vm {
  private _filterVm: FilterVm = null;
  private _connectionsVm: ConnectionsVm = null;

  public constructor(
    router: Router,
    activatedRoute: ActivatedRoute,
    companyDetailsProvider: CompanyDetailsProvider,
    vacancyDetaisProvider: VacancyDetailsProvider,
    companyDetailsUpdator: CompanyDetailsUpdator,
    eventDetailsUpdator: EventDetailsUpdator,
    vacancyDetailsUpdator: VacancyDetailsUpdator,
    connectionsManager: ConnectionsManager
  ) {
    this._filterVm = new FilterVm(router, activatedRoute, companyDetailsProvider, companyDetailsUpdator, eventDetailsUpdator, vacancyDetailsUpdator);
    this._connectionsVm = new ConnectionsVm(activatedRoute, vacancyDetaisProvider, connectionsManager, this._filterVm);
  }

  public get filterVm(): FilterVm {
    return this._filterVm;
  }

  public get connectionsVm(): ConnectionsVm {
    return this._connectionsVm;
  }

  public async init(companyId: number): Promise<void> {
    await this._filterVm.init(companyId);
    await this._connectionsVm.init();
  }
}

@Component({
  selector: 'sk-company-connections-page',
  templateUrl: './sk-company-connections-page.component.html',
  styleUrls: ['./sk-company-connections-page.component.scss']
})
export class SkCompanyConnectionsPageComponent implements OnInit, OnDestroy, AfterViewInit {

  ConnectionStatuses = ConnectionStatuses;

  @ViewChild("eventsTabs") eventsTabs: SkTabsComponent;
  @ViewChild("connectionsTabsComponent") connectionsTabsComponent: SkTabsComponent;
  @ViewChild("feedbackPopup") public feedbackPopup: SkFeedbackPopupComponent;

  private _disposedSubj: Subject<void> = new Subject();

  private _router: Router;
  private _activatedRoute: ActivatedRoute;
  private _security: Security;
  private _vm: Vm;

  private _queryParamsSub: Subscription = null;

  public constructor(
    router: Router,
    activatedRoute: ActivatedRoute,
    security: Security,
    companyDetailsProvider: CompanyDetailsProvider,
    vacancyDetailsProvider: VacancyDetailsProvider,
    companyDetailsUpdator: CompanyDetailsUpdator,
    eventDetailsUpdator: EventDetailsUpdator,
    vacancyDetailsUpdator: VacancyDetailsUpdator,
    connectionsManager: ConnectionsManager
  ) {
    this._router = router;
    this._activatedRoute = activatedRoute;
    this._security = security;
    this._vm = new Vm(router, activatedRoute, companyDetailsProvider, vacancyDetailsProvider, companyDetailsUpdator, eventDetailsUpdator, vacancyDetailsUpdator, connectionsManager);
  }

  public get vm(): Vm {
    return this._vm;
  }

  public ngOnInit(): void {

  }

  public ngOnDestroy(): void {
    this._disposedSubj.next(null);
    this._disposedSubj.complete();
  }

  public ngAfterViewInit(): void {

    this._security.currentUserData$
      .pipe(takeUntil(this._disposedSubj))
      .subscribe(async (currentUserData) => {
        if (currentUserData && currentUserData.company) {
          await this._vm.init(currentUserData.company.id);

          if (this._queryParamsSub) {
            this._queryParamsSub.unsubscribe();
          }

          this._queryParamsSub = this._activatedRoute.queryParamMap
            .pipe(takeUntil(this._disposedSubj))
            .subscribe(queryParamMap => {
              var eventsTab = queryParamMap.get("eventsTab");
              if (eventsTab) {
                this.eventsTabs.selectTab(parseInt(eventsTab));
              } else {
                this.eventsTabs.selectTab(0);
              }

              var connectionsTab = queryParamMap.get("connectionsTab");
              if (connectionsTab) {
                this.connectionsTabsComponent.selectTab(parseInt(connectionsTab));
              }

            });

        } else {
          await this._vm.init(null);
        }

      });
  }

  public updateEventsTabsIndexInUrl(index: number): void {
    var queryParams = { ...this._activatedRoute.snapshot.queryParams };

    var newQueryParams = {};
    newQueryParams["eventsTab"] = index;

    this._router.navigate([], { queryParams: newQueryParams });
  }

  public updateConnectionsTabsIndexInUrl(index: number): void {
    var queryParams = { ...this._activatedRoute.snapshot.queryParams };

    var newQueryParams = {};
    newQueryParams["eventsTab"] = queryParams["eventsTab"];
    newQueryParams["eventId"] = queryParams["eventId"];
    newQueryParams["vacancyId"] = queryParams["vacancyId"];
    queryParams["connectionsTab"] = index;

    this._router.navigate([], { queryParams: queryParams });
  }

  public message(connectionId: number): void {
    this._router.navigate(["/dialogs"], { queryParams: { connectionId: connectionId } });
  }

  public async postFeedback(id: number): Promise<void> {
    this.feedbackPopup.show("ForExpert", id);
  }
}
