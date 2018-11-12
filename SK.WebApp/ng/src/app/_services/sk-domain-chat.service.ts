import { Injectable, Inject, PLATFORM_ID, OnDestroy } from '@angular/core';
import { HttpClient, HttpParams, HttpErrorResponse } from '@angular/common/http';
import { isPlatformBrowser } from '@angular/common';
import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr';
import { Subject, Observable, BehaviorSubject } from 'rxjs';
import { map, takeUntil } from 'rxjs/operators';
import { ApiBaseUrlProvider } from './sk-api-base-url-provider.service';
import { ConnectionsManager } from './sk-connections-manager.service';
import { HttpTransferableError } from '../_classes/http-transferable-error';

export namespace ExpertConnections {
  export class Specialization {
    public id: string;
    public name: string;
  }
  export class Speciality {
    public id: string;
    public name: string;
    public specialization: Specialization;
  }
  export class Company {
    public id: number;
    public mame: string;
    public isOnline: boolean;
    public lastSeenTime: Date;
  }
  export class Event {
    public id: number;
    public name: string;
    public logoImageUrl: string;
    public company: Company;
  }
  export class Vacancy {
    public id: number;
    public speciality: Speciality;
    public event: Event;
  }
  export class LastMessage {
    public id: number;
    public body: string;
    public time: Date;
  }
  export class Feedback {
    public id: number;
    public rating: number;
    public commentHtml: string;
  }
  export class Connection {
    public id: number;
    public type: string;
    public status: string;
    public vacancy: Vacancy;
    public feedbackForExpert: Feedback;
    public feedbackForCompany: Feedback;
    public newIncomingMessagesCount: number;
    public lastMessage: LastMessage;
  }
  export class ExpertConnectionsRes {
    public connections: Connection[];
  }
}

export namespace CompanyConnections {
  export class Expert {
    public id: number;
    public name: string;
    public thumbnailImageUrl: string;
  }
  export class Vacancy {
    public id: number;
  }
  export class LastMessage {
    public id: number;
    public body: string;
    public time: Date;
  }
  export class Feedback {
    public id: number;
    public rating: number;
    public commentHtml: string;
  }
  export class Connection {
    public id: number;
    public type: string;
    public status: string;
    public expert: Expert;
    public vacancy: Vacancy;
    public feedbackForExpert: Feedback;
    public feedbackForCompany: Feedback;
    public newIncomingMessagesCount: number;
    public lastMessage: LastMessage;
  }
  export class CompanyConnectionsRes {
    public connections: Connection[];
  }
}

export namespace Messages {
  export class ChatMessageDirections {
    public static get expertToVacancy(): string { return "ExpertToVacancy"; }
    public static get vacancyToExpert(): string { return "VacancyToExpert"; }
  }

  export class Message {
    public id: number;
    public direction: string;
    public body: string;
    public sendTime: Date;
    public receiveTime: Date;
    public connectionId: number;
    public connectionType: string;
    public expertId: number;
    public expertName: string;
    public companyId: number;
    public companyName: string;
  }

  export class MessagesReq {
    public connectionId: number;
  }
  export class MessagesRes {
    public messages: Message[];
  }
}


export class SendMessageReq {
  public body: string;
  public connectionId: number;
}

export class MarkConnectionAsViewedReq {
  public connectionId: number;
}


export class NotYourMessagesError extends HttpTransferableError {
  public static parseFromRes(res: HttpErrorResponse): NotYourMessagesError | null {
    return HttpTransferableError.parseFromResInternal(res, "NOT_YOUR_MESSAGES", (extraData) => new NotYourMessagesError());
  }
}


export function createChatInitializer(chat: Chat) {
  return async () => {
    await chat.init();
  }
}

@Injectable({
  providedIn: 'root',
})
export class Chat implements OnDestroy {
  private _disposedSubj: Subject<void> = new Subject();

  private _platformId: Object;

  private _http: HttpClient;
  private _baseUrlProvider: ApiBaseUrlProvider;
  private _connectionsManager: ConnectionsManager;
  private _hubConnection: HubConnection;

  private _messaageReceivedSubj: Subject<Messages.Message> = new Subject<Messages.Message>();
  private _newIncomingMessagesChangedSubj: Subject<Messages.MessagesRes> = new BehaviorSubject<Messages.MessagesRes>(null);
  private _messagesDeliveredSubject: Subject<Messages.MessagesRes> = new Subject<Messages.MessagesRes>();



  public constructor(@Inject(PLATFORM_ID) platformId: Object,
    http: HttpClient,
    baseUrlProvider: ApiBaseUrlProvider,
    connectionsManager: ConnectionsManager,
  ) {
    this._platformId = platformId;
    this._http = http;
    this._baseUrlProvider = baseUrlProvider;
    this._connectionsManager = connectionsManager;
  }

  public get messageReceived$(): Observable<Messages.Message> {
    return this._messaageReceivedSubj;
  }

  public get newIncomingMessages$(): Observable<Messages.MessagesRes> {
    return this._newIncomingMessagesChangedSubj;
  }

  public get messagesDelivered$(): Observable<Messages.MessagesRes> {
    return this._messagesDeliveredSubject;
  }

  public async reloadNewIncomingMessages(): Promise<void> {
    var res = await this._http.get(this._baseUrlProvider.concatWith("Chat/GetNewIncomingMessages")).pipe(map(res => res as Messages.MessagesRes)).toPromise();
    this._newIncomingMessagesChangedSubj.next(res);
  }

  public async init(): Promise<void> {
    if (isPlatformBrowser(this._platformId)) {
      var builder = new HubConnectionBuilder();
      this._hubConnection = builder.withUrl("/chatHub").build();

      this._hubConnection.on("MessageReceived", async (msg) => {
        await this.reloadNewIncomingMessages();
        this._messaageReceivedSubj.next(msg);
      });

      this._hubConnection.on("MessagesDelivered", async (messagesRes: Messages.MessagesRes) => {
        await this.reloadNewIncomingMessages();
        this._messagesDeliveredSubject.next(messagesRes);
      });

      this._connectionsManager.connectionCanceled$
        .pipe(takeUntil(this._disposedSubj))
        .subscribe(() => {
          this.reloadNewIncomingMessages();
        });

      this._hubConnection.start();

      this.reloadNewIncomingMessages();
    }
  }

  public async ngOnDestroy(): Promise<void> {
    this._disposedSubj.next(null);
    this._disposedSubj.complete();
  }

  public async sendMessage(req: SendMessageReq): Promise<void> {
    await this._hubConnection.send("SendMessage", req.connectionId, req.body);
  }

  public async sendMessageToAll(req: SendMessageReq): Promise<void> {
    await this._hubConnection.send("SendMessageToAll", req.connectionId, req.body);
  }

  public async markConnectionAsViewed(req: MarkConnectionAsViewedReq): Promise<void> {
    await this._hubConnection.send("MarkConnectionAsViewed", req.connectionId);
  }

  public async getActiveExpertConnections(): Promise<ExpertConnections.ExpertConnectionsRes> {
    var httpParams = new HttpParams();
    var res = await this._http.get(this._baseUrlProvider.concatWith("Chat/GetActiveExpertConnections"), { params: httpParams })
      .pipe(map(res => res as ExpertConnections.ExpertConnectionsRes))
      .toPromise();
    return res;
  }

  public async getActiveCompanyConnections(): Promise<CompanyConnections.CompanyConnectionsRes> {
    var httpParams = new HttpParams();
    var res = await this._http.get(this._baseUrlProvider.concatWith("Chat/GetActiveCompanyConnections"), { params: httpParams })
      .pipe(map(res => res as CompanyConnections.CompanyConnectionsRes))
      .toPromise();
    return res;
  }

  public async getMessages(req: Messages.MessagesReq): Promise<Messages.MessagesRes> {
    try {
      var httpParams = new HttpParams().set("connectionId", req.connectionId.toString());
      var res = await this._http.get(this._baseUrlProvider.concatWith("Chat/GetMessaages"), { params: httpParams })
        .pipe(map(res => res as Messages.MessagesRes))
        .toPromise();
      return res;
    } catch (e) {
      if (e instanceof HttpErrorResponse) {
        var err = NotYourMessagesError.parseFromRes(e);
        if (err) {
          throw err;
        } else {
          throw e;
        }
      }
    }
  }

  public async getNewIncomingMessages(): Promise<Messages.MessagesRes> {
    return await this._http.get(this._baseUrlProvider.concatWith("Chat/GetNewIncomingMessages"))
      .pipe(map(res => res as Messages.MessagesRes))
      .toPromise();
  }
}
