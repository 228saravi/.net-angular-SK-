import { Component, OnInit } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr';

import { ConnectableObservable } from 'rxjs';
import { publishReplay, filter } from 'rxjs/operators';

import { Chat, Messages } from '../../_services/sk-domain-chat.service';
import { Security } from '../../_services/sk-security.service';



@Component({
  selector: 'sk-dialogs-page',
  templateUrl: './sk-dialogs-page.component.html',
  styleUrls: ['./sk-dialogs-page.component.scss']
})
export class SkDialogsPageComponent implements OnInit {
  private _security: Security;
  messages: Messages.Message[] = [];

  chat: Chat;

  constructor(security: Security, chat: Chat) {
    this._security = security;
    this.chat = chat;
  }

  public get showExpertDialogs(): boolean {
    return !!(this._security.currentUserData && this._security.currentUserData.expertProfile);
  }

  public get showCompanyDialogs(): boolean {
    return !!(this._security.currentUserData && this._security.currentUserData.company);
  }

  public async ngOnInit(): Promise<void> {
    
  }

}
