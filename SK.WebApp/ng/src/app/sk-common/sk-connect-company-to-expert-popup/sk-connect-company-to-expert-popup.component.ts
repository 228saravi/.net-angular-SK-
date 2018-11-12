import { Component, OnInit, ViewChild } from '@angular/core';
import { DxPopupComponent } from 'devextreme-angular';

import { CompanyDetailsProvider, Res as CompanyRes, Company } from '../../_services/sk-domain-company-details-provider.service';
import { EventDetailsProvider, Res as EventRes, Event, Vacancy } from '../../_services/sk-domain-event-details-provider.service';
import { ConnectionsManager, AmountIsFullError, NotYourCompanyError, AlreadyConnectedError } from '../../_services/sk-connections-manager.service';
import { CompanyIsNotPublishedError } from '../../_services/sk-domain-company-details-updator.service';

class Vm {
  private _now: Date = new Date(new Date().setSeconds(0));

  private _companyDetailsProvider: CompanyDetailsProvider;
  private _eventDetailsProvider: EventDetailsProvider;
  private _connectionsManager: ConnectionsManager;

  private _expertProfileId: number;

  private _company: Company;
  private _event: Event;
  private _vacancies: Vacancy[] = [];

  //public preventClosingMarker: boolean = true;

  public constructor(companyDetailsProvider: CompanyDetailsProvider, eventDetailsProvider: EventDetailsProvider, connectionsManager: ConnectionsManager) {
    this._companyDetailsProvider = companyDetailsProvider;
    this._eventDetailsProvider = eventDetailsProvider;
    this._connectionsManager = connectionsManager;
  }

  public get company(): Company {
    return this._company;
  }

  public get event(): Event {
    return this._event;
  }

  public get vacancies(): Vacancy[] {
    return this._vacancies;
  }

  public async init(expertProfileId: number, companyId: number): Promise<void> {
    this._expertProfileId = expertProfileId;

    this._company = null;
    this._event = null;
    this._vacancies = null;

    var res = await this._companyDetailsProvider.get({ companyId: companyId, withVacancies: false });
    this._company = res.company;
    this._company.events = this._company.events
      .filter(e => e.isPublished)
      .filter(e => !e.startTime || e.startTime.valueOf() > this._now.valueOf());
  }

  public async selectEvent(eventId: number): Promise<void> {
    var res = await this._eventDetailsProvider.get({ eventId: eventId, withNotFullVacancies: true, withConnectionsForExpert: this._expertProfileId });
    this._event = res.event;
    this._vacancies = this._event.vacancies
      .filter(v => v.isPublished);
  }

  public async deselectEvent(): Promise<void> {
    this._event = null;
    this._vacancies = null;
  }

  public async selectVacancy(vacancyId: number): Promise<void> {
    try {
      await this._connectionsManager.registerVacancyToExpertConnection({
        vacancyId: vacancyId,
        expertProfileId: this._expertProfileId,
      });
    } catch (e) {
      if (e instanceof AmountIsFullError || e instanceof NotYourCompanyError || e instanceof AlreadyConnectedError || e instanceof CompanyIsNotPublishedError) {
      } else {
        throw e;
      }
    }
  }
}

@Component({
  selector: 'sk-connect-company-to-expert-popup',
  templateUrl: './sk-connect-company-to-expert-popup.component.html',
  styleUrls: ['./sk-connect-company-to-expert-popup.component.scss']
})
export class SkConnectCompanyToExpertPopupComponent implements OnInit {
  @ViewChild("popup") popup: DxPopupComponent;

  private _companyDetailsProvider: CompanyDetailsProvider;
  private _eventDetailsProvider: EventDetailsProvider;
  private _connectionsManager: ConnectionsManager;

  private _vm: Vm = null;

  public constructor(
    companyDetailsProvider: CompanyDetailsProvider,
    eventDetailsProvider: EventDetailsProvider,
    connectionsManager: ConnectionsManager
  ) {
    this._companyDetailsProvider = companyDetailsProvider;
    this._eventDetailsProvider = eventDetailsProvider;
    this._connectionsManager = connectionsManager;
  }

  public get vm(): Vm {
    return this._vm;
  }

  public ngOnInit(): void { }

  public async show(expertProfileId: number, companyId: number): Promise<void> {
    this._vm = new Vm(this._companyDetailsProvider, this._eventDetailsProvider, this._connectionsManager);
    await this._vm.init(expertProfileId, companyId);
    await (this.popup.instance.show() as Promise<void>);
  }

  public async hide(): Promise<void> {
    //this._vm.preventClosingMarker = false;
    await (this.popup.instance.hide() as Promise<void>);
  }

  public onHidding($event): void {
    //if (this._vm.preventClosingMarker) {
    //  $event.cancel = true;
    //}
  }

  public onHidden(): void {
    this._vm = null;
  }

}
