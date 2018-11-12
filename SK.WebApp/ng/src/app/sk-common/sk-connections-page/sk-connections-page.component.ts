import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Security } from '../../_services/sk-security.service';

@Component({
  selector: 'sk-connections-page',
  templateUrl: './sk-connections-page.component.html',
  styleUrls: ['./sk-connections-page.component.scss']
})
export class SkConnectionsPageComponent {
  private _security: Security;

  public constructor(activatedRoute: ActivatedRoute, security: Security) {
    this._security = security;
  }

  public get showExpertConnections(): boolean {
    return !!(this._security.currentUserData && this._security.currentUserData.expertProfile);
  }

  public get showCompanyConnections(): boolean {
    return !!(this._security.currentUserData && this._security.currentUserData.company);
  }
}
