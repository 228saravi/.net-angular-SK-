import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

import { DxScrollViewComponent } from 'devextreme-angular';

import { Observable, ConnectableObservable, Subject, BehaviorSubject, Subscription, of } from 'rxjs';
import { publishReplay, flatMap, tap, map, delay, takeUntil } from 'rxjs/operators';

import { SkFeedbackPopupComponent } from '../sk-feedback-popup/sk-feedback-popup.component';

import { Security } from '../../_services/sk-security.service';
import { Chat, Messages, CompanyConnections, NotYourMessagesError } from '../../_services/sk-domain-chat.service';
import { ConnectionTypes } from '../../_classes/sk-domain-connection-types';
import { VacancyDetailsProvider, Vacancy as VacancyDetails } from '../../_services/sk-domain-vacancy-details-provider.service';
import { ExpertProfileDetailsProvider, ExpertProfileRes as ExpertDetails } from '../../_services/sk-domain-expert-profile-details-provider.service';
import { ConnectionStatuses } from '../../_classes/sk-domain-connection-statuses';
import { ConnectionsManager } from '../../_services/sk-connections-manager.service';

export type DetailsMode = "Expert" | "Vacancy" | "Event";

export class ConnectionsVm {
  private _now: Date = new Date();

  private _router: Router;
  private _activatedRoute: ActivatedRoute;

  private _chat: Chat;
  private _vacancyDetailsProvider: VacancyDetailsProvider;
  private _expertProfileDetailsProvider: ExpertProfileDetailsProvider;

  private _companyConnectionsRes: CompanyConnections.CompanyConnectionsRes = null;
  private _selectedConnection: CompanyConnections.Connection = null;
  private _expertDetails: ExpertDetails = null;
  private _vacancyDetails: VacancyDetails = null;

  private _connectionSelected$: Observable<CompanyConnections.Connection> = null;

  private _connectionSelectedSub: Subscription = null;
  private _messageReceivedSub: Subscription = null;

  private _detailsMode: DetailsMode = "Expert";

  public constructor(
    router: Router,
    activatedRoute: ActivatedRoute,
    chat: Chat,
    vacancyDetailsProvider: VacancyDetailsProvider,
    expertProfileDetailsProvider: ExpertProfileDetailsProvider,
  ) {
    this._router = router;
    this._activatedRoute = activatedRoute;
    this._chat = chat;
    this._vacancyDetailsProvider = vacancyDetailsProvider;
    this._expertProfileDetailsProvider = expertProfileDetailsProvider;
  }

  public get companyConnectionsRes(): CompanyConnections.CompanyConnectionsRes {
    return this._companyConnectionsRes;
  }

  public get selectedConnection(): CompanyConnections.Connection {
    return this._selectedConnection;
  }

  public get expertDetails(): ExpertDetails {
    return this._expertDetails;
  }

  public get vacancyDetails(): VacancyDetails {
    return this._vacancyDetails;
  }

  public get detailsMode(): DetailsMode {
    return this._detailsMode;
  }

  public set detailsMode(val: DetailsMode) {
    this._detailsMode = val;
  }

  public get connectionSelected$(): Observable<CompanyConnections.Connection> {
    return this._connectionSelected$;
  }

  public get isCurrentConnectionFeedbackable(): boolean {
    if (!this._selectedConnection || !this._vacancyDetails) {
      return false;
    }

    return this._selectedConnection.status == ConnectionStatuses.connected &&
      this.vacancyDetails.event.endTime &&
      this._now.valueOf() > this.vacancyDetails.event.endTime.valueOf() &&
      this._selectedConnection.feedbackForExpert == null;
  }

  public get isCurrentConnectionCancelable(): boolean {
    if (!this._selectedConnection || !this._vacancyDetails) {
      return false;
    }

    if (!this._vacancyDetails.startTime) {
      return true;
    }

    if (this._selectedConnection.status == ConnectionStatuses.initiated) {
      return true;
    }

    var distanceToStart = this._vacancyDetails.startTime.valueOf() - this._now.valueOf();

    return distanceToStart > 86400 /*one day in miliseconds*/ &&
      this._selectedConnection.status != ConnectionStatuses.canceled;
  }

  public get isCurrentEventOver(): boolean {
    if (!this._vacancyDetails) {
      return false;
    }

    return this._now.valueOf() > this._vacancyDetails.event.endTime.valueOf();
  }

  public async init(): Promise<void> {
    this._companyConnectionsRes = await this._chat.getActiveCompanyConnections();

    this._connectionSelected$ = this._activatedRoute.queryParamMap.pipe(
      map(queryParamMap => parseInt(queryParamMap.get("connectionId"))),
      map(connectionId => this._companyConnectionsRes.connections.find(c => c.id == connectionId) || null)
    );

    this._connectionSelectedSub = this._connectionSelected$.subscribe(async connection => {

      this._selectedConnection = connection;

      if (this._selectedConnection) {
        this._selectedConnection.newIncomingMessagesCount = 0;

        var [expertRes, vacancyRes] = await Promise.all([
          this._expertProfileDetailsProvider.get({ expertProfileId: this._selectedConnection.expert.id }),
          this._vacancyDetailsProvider.get({ vacancyId: this._selectedConnection.vacancy.id })
        ]);

        this._expertDetails = expertRes.expertProfile;
        this._vacancyDetails = vacancyRes.foundVacancy;
      }

    });


    var messageReceivedPublished$ = this._chat.messageReceived$.pipe(publishReplay(20)) as ConnectableObservable<Messages.Message>;
    messageReceivedPublished$.connect();

    this._messageReceivedSub = messageReceivedPublished$.subscribe(async (msg) => {

      var messageConnection = this._companyConnectionsRes.connections.find(c => c.id == msg.connectionId);
      if (messageConnection) {
        messageConnection.lastMessage = {
          id: msg.id,
          body: msg.body,
          time: new Date()
        };

        if (msg.direction == Messages.ChatMessageDirections.expertToVacancy) {
          messageConnection.newIncomingMessagesCount++;
        }

      }

      if (this._selectedConnection && msg.connectionId == this._selectedConnection.id) {
        await this._chat.markConnectionAsViewed({ connectionId: this._selectedConnection.id });
        this._selectedConnection.newIncomingMessagesCount = 0;
      }

    });
  }

  public async dispose(): Promise<void> {
    if (this._connectionSelectedSub) {
      this._connectionSelectedSub.unsubscribe();
    }

    if (this._messageReceivedSub) {
      this._messageReceivedSub.unsubscribe();
    }
  }

  public selectConnection(connectionId: number): void {
    this._router.navigate([], { queryParams: { connectionId: connectionId } });
  }
}

export class MessagesVm {

  private _activatedRoute: ActivatedRoute;

  private _chat: Chat;
  private _connectionsVm: ConnectionsVm;
  private _messagesRes: Messages.MessagesRes;

  private _connectionSelected$: Observable<number> = null;
  private _selectedConnectionId: number;

  private _messageReceivedSub: Subscription = null;
  private _messagesDeliveredSub: Subscription = null;

  private _messagesUpdatedSubj: BehaviorSubject<void> = new BehaviorSubject<void>(null);

  public constructor(activatedRoute: ActivatedRoute, chat: Chat, connectionsVm: ConnectionsVm) {
    this._activatedRoute = activatedRoute;
    this._chat = chat;
    this._connectionsVm = connectionsVm;
  }

  public get messagesRes(): Messages.MessagesRes {
    return this._messagesRes;
  }

  public get messagesUpdated$(): Observable<void> {
    return this._messagesUpdatedSubj;
  }

  public async init(): Promise<void> {
    this._connectionSelected$ = this._connectionsVm.connectionSelected$.pipe(map(c => c ? c.id : null));

    this._connectionSelected$.pipe(
      tap(connectionId => this._selectedConnectionId = connectionId),
      flatMap(connectionId => !!connectionId
        ? this._chat.getMessages({ connectionId: connectionId })
        : of(null)
      ),
    ).subscribe(async (messagesRes) => {
      this._messagesRes = messagesRes;

      if (this._selectedConnectionId) {
        await this._chat.markConnectionAsViewed({ connectionId: this._selectedConnectionId });
      }

      this._messagesUpdatedSubj.next(null);
    }, (e) => {
      var err = e as NotYourMessagesError;
      if (err) { }
      else {
        throw err;
      }
    });

    var messageReceivedPublished$ = this._chat.messageReceived$.pipe(publishReplay(20)) as ConnectableObservable<Messages.Message>;
    messageReceivedPublished$.connect();

    var messagesDeliveredPublished$ = this._chat.messagesDelivered$.pipe(
      publishReplay(20)
    ) as ConnectableObservable<Messages.MessagesRes>;
    messagesDeliveredPublished$.connect();

    this._messageReceivedSub = messageReceivedPublished$.subscribe(async (msg) => {
      if (
        this._messagesRes &&
        (msg.connectionId == this._selectedConnectionId) &&
        (this._messagesRes.messages.length == 0 || msg.id > this._messagesRes.messages[this._messagesRes.messages.length - 1].id)
      ) {
        this._messagesRes.messages.push(msg);
      }

      this._messagesUpdatedSubj.next(null);
    });

    this._messagesDeliveredSub = messagesDeliveredPublished$.subscribe(messagesRes => {
      if (this._messagesRes && messagesRes) {
        for (let i = 0; i < this._messagesRes.messages.length; i++) {
          for (let m of messagesRes.messages) {
            if (m.id == this._messagesRes.messages[i].id) {
              this._messagesRes.messages[i] = m;
            }
          }
        }
      }

      this._messagesUpdatedSubj.next(null);
    });

  }

  public async dispose(): Promise<void> {
    if (this._messageReceivedSub) {
      await this._messageReceivedSub.unsubscribe();
    }

    if (this._messagesDeliveredSub) {
      await this._messagesDeliveredSub.unsubscribe();
    }
  }
}

export class NewMessageVm {
  private _chat: Chat;

  private _connectionsVm: ConnectionsVm;

  private _connectionSelected$: Observable<number>;
  private _selectedConnectionId: number;

  private _disposedSubj: Subject<void> = new Subject();

  private createMessageHtml(messageText: string): string {
    if (messageText == null) {
      return messageText;
    }

    return messageText.replace(/(?:\r\n|\r|\n)/g, '<br />');
  }

  public constructor(chat: Chat, connectionsVm: ConnectionsVm) {
    this._chat = chat;
    this._connectionsVm = connectionsVm;
  }

  public message: string = null;
  public sendToAll: boolean = false;

  public get isReady(): boolean {
    return !!this._selectedConnectionId;
  }

  public async init(): Promise<void> {
    this._connectionSelected$ = this._connectionsVm.connectionSelected$.pipe(
      map(c => c ? c.id : null),
      takeUntil(this._disposedSubj)
    );

    this._connectionSelected$.subscribe(connectionId => {
      this._selectedConnectionId = connectionId;
    });
  }

  public async dispose(): Promise<void> {
    this._disposedSubj.next(null);
    this._disposedSubj.complete();
  }

  public async send(): Promise<void> {
    if (this.isReady && this.message && this.message.length > 0) {
      if (this.sendToAll) {
        await this._chat.sendMessageToAll({ body: this.createMessageHtml(this.message), connectionId: this._selectedConnectionId });
      } else {
        await this._chat.sendMessage({ body: this.createMessageHtml(this.message), connectionId: this._selectedConnectionId });
      }

      this.message = null;
      this.sendToAll = false;
    }
  }
}

@Component({
  selector: 'sk-company-dialogs',
  templateUrl: './sk-company-dialogs.component.html',
  styleUrls: ['./sk-company-dialogs.component.scss']
})
export class SkCompanyDialogsComponent implements OnInit, OnDestroy {
  @ViewChild("messagesScrollView") messagesScrollView: DxScrollViewComponent;
  @ViewChild("feedbackPopup") public feedbackPopup: SkFeedbackPopupComponent;

  ConnectionTypes = ConnectionTypes;
  ChatMessageDirections = Messages.ChatMessageDirections;

  private _router: Router;
  private _activatedRoute: ActivatedRoute;
  private _security: Security;
  private _chat: Chat;
  private _vacancyDetailsProvider: VacancyDetailsProvider;
  private _expertProfileDetailsProvider: ExpertProfileDetailsProvider;
  private _connectionsManager: ConnectionsManager;

  private _connectionsVm: ConnectionsVm = null;
  private _messagesVm: MessagesVm = null;
  private _newMessageVm: NewMessageVm = null;

  private _messageReceivedSub: Subscription = null;

  private async reload(): Promise<void> {
    await this._router.navigate(["/dialogs"]);
    await this.ngOnDestroy();
    await this.ngOnInit();
  }

  constructor(router: Router,
    activatedRoute: ActivatedRoute,
    security: Security,
    chat: Chat,
    vacancyDetailsProvider: VacancyDetailsProvider,
    expertProfileDetailsProvider: ExpertProfileDetailsProvider,
    connectionsManager: ConnectionsManager,
  ) {
    this._router = router;
    this._activatedRoute = activatedRoute;
    this._security = security;
    this._chat = chat;
    this._vacancyDetailsProvider = vacancyDetailsProvider;
    this._expertProfileDetailsProvider = expertProfileDetailsProvider;
    this._connectionsManager = connectionsManager;
  }

  public async ngOnInit(): Promise<void> {
    this._connectionsVm = new ConnectionsVm(this._router, this._activatedRoute, this._chat, this._vacancyDetailsProvider, this._expertProfileDetailsProvider);
    this._messagesVm = new MessagesVm(this._activatedRoute, this._chat, this._connectionsVm);
    this._newMessageVm = new NewMessageVm(this._chat, this._connectionsVm);

    await this._connectionsVm.init();
    await this._messagesVm.init();
    await this._newMessageVm.init();

    this._messagesVm.messagesUpdated$.subscribe(() => {
      this.messagesScrollView.instance.scrollTo(this.messagesScrollView.instance.scrollHeight() + 5000);
    });
  }

  public async ngOnDestroy(): Promise<void> {
    await this._connectionsVm.dispose();
    await this._messagesVm.dispose();
    await this._newMessageVm.dispose();

    this._connectionsVm = null;
    this._messagesVm = null;
    this._newMessageVm = null;

    if (this._messageReceivedSub) {
      this._messageReceivedSub.unsubscribe();
    }
  }

  public get connectionsVm(): ConnectionsVm {
    return this._connectionsVm;
  }

  public get messagesVm(): MessagesVm {
    return this._messagesVm;
  }

  public get newMessageVm(): NewMessageVm {
    return this._newMessageVm;
  }

  public async postFeedback(): Promise<void> {
    this.feedbackPopup.show("ForExpert", this._connectionsVm.selectedConnection.id);
  }

  public async cancelConnection(): Promise<void> {
    await this._connectionsManager.cancelConnection({ connectionId: this._connectionsVm.selectedConnection.id });
    await this.reload();
  }

  public async onFeedbackPosted(): Promise<void> {
    await this.reload();
  }
}
