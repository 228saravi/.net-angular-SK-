import { Component, OnInit, OnDestroy } from '@angular/core';
import { Location } from '@angular/common';

import { Subscription, Observable, merge, from, combineLatest, of, Subject } from 'rxjs';
import { flatMap, map, shareReplay, publishReplay, takeUntil } from 'rxjs/operators';

import { Security, CurrentUserDataRes } from '../../_services/sk-security.service';
import { Chat } from '../../_services/sk-domain-chat.service';
import { Router } from '@angular/router';
import { ConnectionsManager } from '../../_services/sk-connections-manager.service';
import { CompanyDetailsProvider } from '../../_services/sk-domain-company-details-provider.service';
import { ExpertProfileDetailsProvider } from '../../_services/sk-domain-expert-profile-details-provider.service';
import { ConnectionTypes } from '../../_classes/sk-domain-connection-types';
import { ConnectionStatuses } from '../../_classes/sk-domain-connection-statuses';

@Component({
  selector: 'sk-nav',
  templateUrl: './sk-nav.component.html',
  styleUrls: ['./sk-nav.component.scss']
})
export class SkNavComponent implements OnInit, OnDestroy {
  private _router: Router;

  private _security: Security;
  private _chat: Chat;
  private _connectionsManager: ConnectionsManager;
  private _companyDetailsProvider: CompanyDetailsProvider;

  private _newIncomingConnectionsCount$: Observable<number> = null;
  private _newIncomingConnectionsCount: number = null;

  private _disposedSubj: Subject<void> = new Subject();

  public constructor(
    router: Router,
    security: Security,
    chat: Chat,
    connectionsManager: ConnectionsManager,
    companyDetailsProvider: CompanyDetailsProvider,
  ) {
    this._router = router;
    this._security = security;
    this._chat = chat;
    this._connectionsManager = connectionsManager;
    this._companyDetailsProvider = companyDetailsProvider;
  }

  public get data(): CurrentUserDataRes {
    return this._security.currentUserData;
  }

  public get chat(): Chat {
    return this._chat;
  }

  public get newIncomingConnectionsCount$(): Observable<number> {
    return this._newIncomingConnectionsCount$;
  }

  public get newIncomingConnectionsCount(): number {
    return this._newIncomingConnectionsCount;
  }

  public get canSeeExperts(): boolean {
    return this._security.currentUserData &&
      !!this._security.currentUserData.company;
  }

  public get canSeeVacancies(): boolean {
    return this._security.currentUserData &&
      !!this._security.currentUserData.expertProfile;
  }

  public ngOnInit(): void {

    this._newIncomingConnectionsCount$ = combineLatest(
      this._security.currentUserData$,
      merge(
        this._connectionsManager.connectionApproved$,
        this._connectionsManager.connectionCanceled$,
        of(null)
      )
    ).pipe(
      flatMap(([currentUserData]) => {
        var now = new Date();

        if (!currentUserData) {
          return of(0);
        }

        if (currentUserData.company) {
          return from(this._companyDetailsProvider.get({ companyId: currentUserData.company.id, withVacancies: true })).pipe(
            map(res => res.company.events
              .filter(e => e.endTime > now).reduce(
                (sum, e) => sum + e.vacancies.reduce((sum, v) => sum + v.incomingConnectionsCount, 0),
                0
              )
            )
          );
        }

        if (currentUserData.expertProfile) {
          return from(this._connectionsManager.getExpertConnections({ expertProfileId: currentUserData.expertProfile.id })).pipe(
            map(res => res.connections
              .filter(c => c.type == ConnectionTypes.vacancyToExpert)
              .filter(c => c.status == ConnectionStatuses.initiated)
              .filter(c => !!c.vacancy.event.endTime || c.vacancy.event.endTime > now)
              .length
            )
          );
        }

        return of(0);
      })
    );

    this._newIncomingConnectionsCount$
      .pipe(takeUntil(this._disposedSubj))
      .subscribe(x => this._newIncomingConnectionsCount = x);

    this._chat.reloadNewIncomingMessages();
  }

  public ngOnDestroy(): void {
    this._disposedSubj.next(null);
    this._disposedSubj.complete();
  }

  public async onLogoClick(): Promise<void> {
    var u = this._security.currentUserData;

    if (u && u.company) {
      await this._router.navigate(['/experts']);
      return;
    } else {
      await this._router.navigate(['/vacancies']);
      return;
    }

  }

  public async signIn(): Promise<void> {
    await this._security.enshureSignedIn();

    if (window) {
      window.location.reload();
    }

  }

  public async signOut(): Promise<void> {
    await this._security.signOut();
    await this._router.navigateByUrl("/");
  }

}
