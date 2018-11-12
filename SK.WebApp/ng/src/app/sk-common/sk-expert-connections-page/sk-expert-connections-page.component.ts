import { Component, OnInit, ViewChild, AfterViewInit, OnDestroy } from '@angular/core';
import { DxPopoverComponent } from 'devextreme-angular';
import { ConnectionsManager, ExpertConnections, NotYourConnectionError, TooLateToCancelConnectionError } from '../../_services/sk-connections-manager.service';
import { Security } from '../../_services/sk-security.service';
import { ConnectionTypes } from '../../_classes/sk-domain-connection-types';
import { ConnectionStatuses } from '../../_classes/sk-domain-connection-statuses';
import { Subject, BehaviorSubject, Subscription } from 'rxjs';
import { SkFeedbackPopupComponent } from '../sk-feedback-popup/sk-feedback-popup.component';
import { ActivatedRoute, Router } from '@angular/router';
import { SkTabsComponent } from '../../sk-ui-kit/sk-tabs/sk-tabs.component';
import { takeUntil } from 'rxjs/operators';

type FilterMode = "Incoming" | "Outgoing" | "Connected" | "Archival";

class Vm {
  private _now: Date = new Date();

  private _filterMode: FilterMode = "Connected";
  private _res: ExpertConnections.ExpertConnectionsRes;

  private _connectedConnections: ExpertConnections.Connection[] = [];
  private _incomingConnections: ExpertConnections.Connection[] = [];
  private _outgoingConnections: ExpertConnections.Connection[] = [];
  private _archivalConnections: ExpertConnections.Connection[] = [];
  private _filteredConnections: ExpertConnections.Connection[] = [];

  public constructor(res: ExpertConnections.ExpertConnectionsRes) {
    this.setRes(res);
  }

  public get filterMode(): FilterMode {
    return this._filterMode;
  }

  public get res(): ExpertConnections.ExpertConnectionsRes {
    return this._res;
  }

  public get connectedConnections(): ExpertConnections.Connection[] {
    return this._connectedConnections;
  }

  public get incomingConnections(): ExpertConnections.Connection[] {
    return this._incomingConnections;
  }

  public get outgoingConnections(): ExpertConnections.Connection[] {
    return this._outgoingConnections;
  }

  public get archivalConnections(): ExpertConnections.Connection[] {
    return this._archivalConnections;
  }

  public get filteredConnections(): ExpertConnections.Connection[] {
    return this._filteredConnections;
  }

  public setFilter(mode: FilterMode): void {
    this._filterMode = mode;

    if (this._filterMode == "Connected") {
      this._filteredConnections = this._connectedConnections;
      return;
    }

    if (this._filterMode == "Incoming") {
      this._filteredConnections = this._incomingConnections;
      return;
    }

    if (this._filterMode == "Outgoing") {
      this._filteredConnections = this._outgoingConnections;
      return;
    }

    if (this._filterMode == "Archival") {
      this._filteredConnections = this._archivalConnections;
      return;
    }
  }

  public setRes(res: ExpertConnections.ExpertConnectionsRes): void {
    this._res = res;

    if (this._res && this._res.connections) {
      this._connectedConnections = this._res.connections
        .filter(c => c.status == ConnectionStatuses.connected)
        .filter(c => !c.vacancy.event.endTime || c.vacancy.event.endTime.valueOf() > this._now.valueOf());

      this._incomingConnections = this._res.connections
        .filter(c => c.status == ConnectionStatuses.initiated && c.type == ConnectionTypes.vacancyToExpert)
        .filter(c => !c.vacancy.event.endTime || c.vacancy.event.endTime.valueOf() > this._now.valueOf());

      this._outgoingConnections = this._res.connections
        .filter(c => c.status == ConnectionStatuses.initiated && c.type == ConnectionTypes.expertToVacancy)
        .filter(c => !c.vacancy.event.endTime || c.vacancy.event.endTime.valueOf() > this._now.valueOf());

      this._archivalConnections = this._res.connections
        .filter(c => c.status == ConnectionStatuses.connected)
        .filter(c => !c.vacancy.event.endTime || c.vacancy.event.endTime.valueOf() < this._now.valueOf());
    } else {
      this._connectedConnections = [];
      this._incomingConnections = [];
      this._outgoingConnections = [];
      this._archivalConnections = [];
    }

    this.setFilter(this._filterMode);
  }
}

@Component({
  selector: 'sk-expert-connections-page',
  templateUrl: './sk-expert-connections-page.component.html',
  styleUrls: ['./sk-expert-connections-page.component.scss']
})
export class SkExpertConnectionsPageComponent implements OnInit, OnDestroy, AfterViewInit {

  ConnectionTypes = ConnectionTypes;
  ConnectionStatuses = ConnectionStatuses;

  @ViewChild("connectionsTabsComponent") connectionsTabsComponent: SkTabsComponent;
  @ViewChild("feedbackPopup") public feedbackPopup: SkFeedbackPopupComponent;
  @ViewChild("popover") popover: DxPopoverComponent;

  private _router: Router;
  private _activatedRoute: ActivatedRoute;

  private _security: Security;
  private _connectionsManager: ConnectionsManager;

  private _updatedSubj: Subject<void> = new BehaviorSubject<void>(null);
  private _queryParamsSub: Subscription = null;

  private _vm: Vm = null;

  private _now: Date = new Date();

  private _disposedSubj: Subject<void> = new Subject();

  public constructor(router: Router, activatedRoute: ActivatedRoute, security: Security, connectionsManager: ConnectionsManager) {
    this._router = router;
    this._activatedRoute = activatedRoute;
    this._security = security;
    this._connectionsManager = connectionsManager;
  }

  public get vm(): Vm {
    return this._vm;
  }

  public async ngOnInit(): Promise<void> {

  }

  public async ngOnDestroy(): Promise<void> {
    if (this._queryParamsSub) {
      this._queryParamsSub.unsubscribe();
    }

    this._updatedSubj.complete();

    this._disposedSubj.next(null);
    this._disposedSubj.complete();
  }

  public ngAfterViewInit(): void {
    this._updatedSubj
      .pipe(takeUntil(this._disposedSubj))
      .subscribe(async () => {
        var res = await this._connectionsManager.getExpertConnections({ expertProfileId: this._security.currentUserData.expertProfile.id });
        if (!this._vm) {
          this._vm = new Vm(res);
        } else {
          this._vm.setRes(res);
        }

        if (this._queryParamsSub) {
          this._queryParamsSub.unsubscribe();
        }

        this._queryParamsSub = this._activatedRoute.queryParamMap
          .pipe(takeUntil(this._disposedSubj))
          .subscribe(queryParamMap => {
            var connectionsTab = queryParamMap.get("connectionsTab");
            if (connectionsTab) {
              this.connectionsTabsComponent.selectTab(parseInt(connectionsTab));
            } else {
              this.connectionsTabsComponent.selectTab(0);
            }
          });
      });
  }


  public isConnectionCancelable(connection: ExpertConnections.Connection): boolean {
    if (!connection.vacancy.startTime) {
      return true;
    }

    if (connection.status == ConnectionStatuses.initiated) {
      return true;
    }

    var distanceToStart = connection.vacancy.startTime.valueOf() - this._now.valueOf();

    return distanceToStart > 86400 /*one day in miliseconds*/ &&
      connection.status != ConnectionStatuses.canceled;
  }

  public isConnectionApprovable(connection: ExpertConnections.Connection): boolean {
    return connection.type == ConnectionTypes.vacancyToExpert && connection.status == ConnectionStatuses.initiated && connection.vacancy.startTime && connection.vacancy.startTime.valueOf() > this._now.valueOf();
  }

  public isConnectionFeedbackable(connection: ExpertConnections.Connection): boolean {
    return connection.status == ConnectionStatuses.connected &&
      connection.vacancy.event.endTime &&
      this._now.valueOf() > connection.vacancy.event.endTime.valueOf() &&
      connection.feedbackForCompany == null;
  }

  public doesConnectionAllowMessaging(connection: ExpertConnections.Connection): boolean {
    return connection.status == ConnectionStatuses.connected && connection.vacancy.event.endTime && connection.vacancy.event.endTime.valueOf() > this._now.valueOf();
  }



  public isConnectionApproved(connection: ExpertConnections.Connection): boolean {
    return connection.status == ConnectionStatuses.connected;
  }

  public isConnectionIncoming(connection: ExpertConnections.Connection): boolean {
    return true;
  }

  public isConnectionOutgoing(connection: ExpertConnections.Connection): boolean {
    return true;
  }

  public isConnectionOver(connection: ExpertConnections.Connection): boolean {
    return true;
  }


  public onShowAllSkillsGroupClicked($event: MouseEvent, group: ExpertConnections.SkillsGroup): void {
    var template = `
      <strong>${group.name}:</strong><br/><br/>
      ${group.skills.map(s => s.name).reduce((res, name) => res + `<div>${name}</div>`, "")}
    `;

    this.popover.instance.option("target", $event.target);
    this.popover.instance.option("width", "300");
    this.popover.instance.option("contentTemplate", template);

    this.popover.instance.show();
  }


  public async cancelConnection(id: number): Promise<void> {
    if (!(await this._security.enshureSignedIn())) {
      return Promise.resolve(null);
    }

    try {
      await this._connectionsManager.cancelConnection({ connectionId: id });
    } catch (e) {
      if (e instanceof NotYourConnectionError || e instanceof TooLateToCancelConnectionError) {
      } else {
        throw e;
      }
    }

    this._updatedSubj.next(null);
  }

  public async approveConnection(id: number): Promise<void> {
    if (!(await this._security.enshureSignedIn())) {
      return Promise.resolve(null);
    }

    try {
      await this._connectionsManager.approveConnection({ connectionId: id });
    } catch (e) {
      //if (e instanceof NotYourConnectionError || e instanceof TooLateToCancelConnectionError) {
      //} else {
      //  throw e;
      //}

      throw e;
    }

    this._updatedSubj.next(null);
  }

  public async postFeedback(id: number): Promise<void> {
    this.feedbackPopup.show("ForCompany", id);
  }

  public async onFeedbackPosted(): Promise<void> {
    this._updatedSubj.next(null);
  }

  public updateConnectionsTabsIndexInUrl(index: number): void {
    var queryParams = { ...this._activatedRoute.snapshot.queryParams };
    queryParams["connectionsTab"] = index;

    this._router.navigate([], { queryParams: queryParams });
  }

  public message(connectionId: number): void {
    this._router.navigate(["/dialogs"], { queryParams: { connectionId: connectionId } });
  }
}
