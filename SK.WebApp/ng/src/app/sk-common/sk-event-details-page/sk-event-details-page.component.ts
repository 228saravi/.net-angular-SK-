import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Location } from '@angular/common';
import { NgForm } from '@angular/forms';

import { Subject, BehaviorSubject, combineLatest, from } from 'rxjs';
import { map, filter, flatMap, tap, takeUntil } from 'rxjs/operators';

import { Security, CurrentUserDataRes } from '../../_services/sk-security.service';
import { EventDetailsProvider, Res, Event } from '../../_services/sk-domain-event-details-provider.service';
import { ConnectionsManager, AmountIsFullError, AlreadyConnectedError } from '../../_services/sk-connections-manager.service';
import { SegmentsDirectory, Res as SegmentsRes, Segment } from '../../_services/sk-domain-segments-directory.service';
import { CitiesDirectory, Res as CitiesRes, City } from '../../_services/sk-domain-cities-directory.service';
import { EventTypesDirectory, Res as EventTypesRes, EventType } from '../../_services/sk-domain-event-types-directory.service';
import { EventFormatsDirectory, Res as EventFormatsRes, EventFormat } from '../../_services/sk-domain-event-formats-directory.service';
import { EventDetailsUpdator, EventIsNotPublishedError } from '../../_services/sk-domain-event-details-updator.service';

class HeaderEditingVm {
  public name: string;
  public typeId: string;
  public formatId: string;

  public allTypes: EventType[];
  public allFormats: EventFormat[];

  public constructor(event: Event, typesRes: EventTypesRes, formatsRes: EventFormatsRes) {
    this.name = event.name;
    this.typeId = event.type ? event.type.id : null;
    this.formatId = event.format ? event.format.id : null;

    this.allTypes = typesRes.eventTypes;
    this.allFormats = formatsRes.eventFormats;
  }
}

class MainInfoEditingVm {
  public startTime: Date;
  public endTime: Date;
  public segmentId: string;
  public cityId: string;
  public address: string;
  public estimatedGuestsCount: number;
  public estimatedAverageCheck: number;
  public withtDelivery: boolean;
  public withAccomodation: boolean;

  public allSegments: Segment[] = [];
  public allCities: City[] = [];

  public constructor(event: Event, segmentsRes: SegmentsRes, citiesRes: CitiesRes) {
    this.startTime = event.startTime;
    this.endTime = event.endTime;
    this.segmentId = event.segment ? event.segment.id : null;
    this.cityId = event.city ? event.city.id : null;
    this.address = event.address;
    this.estimatedGuestsCount = event.estimatedGuestsCount;
    this.estimatedAverageCheck = event.estimatedAverageCheck;
    this.withtDelivery = event.withtDelivery;
    this.withAccomodation = event.withAccomodation;

    this.allSegments = segmentsRes.segments;
    this.allCities = citiesRes.cities;
  }
}

class AboutEventEditingVm {
  public html: string;

  public constructor(event: Event) {
    this.html = event.descriptionHtml;
  }
}

class Vm {
  private _now: Date = new Date(new Date().setSeconds(0));

  private _res: Res;
  private _currentUserData: CurrentUserDataRes;
  private _types: EventTypesRes;
  private _formats: EventFormatsRes;
  private _segments: SegmentsRes;
  private _citiesRes: CitiesRes;

  private _logoUploadUrl: string = null;

  private _headerEditing: HeaderEditingVm = null;
  private _mainInfoEditing: MainInfoEditingVm = null;
  private _aboutEventEditing: AboutEventEditingVm = null;

  public constructor(
    res: Res,
    currentUserData: CurrentUserDataRes,
    types: EventTypesRes,
    formats: EventFormatsRes,
    segments: SegmentsRes,
    citiesRes: CitiesRes,
  ) {
    this._res = res;
    this._currentUserData = currentUserData;
    this._types = types;
    this._formats = formats;
    this._segments = segments;
    this._citiesRes = citiesRes;

    this._logoUploadUrl = `/api/EventDetailsUpdator/UploadLogo?eventId=${res.event.id}`;
  }

  public get now(): Date {
    return this._now;
  }

  public get isMyEvent(): boolean {
    return this._currentUserData && this._currentUserData.company && this._currentUserData.company.id == this._res.event.company.id;
  }

  public get isEventOver(): boolean {
    return this._res.event.endTime && this._now.valueOf() > this._res.event.endTime.valueOf();
  }

  public get event(): Event {
    return this._res.event;
  }

  public get logoUploadUrl(): string {
    return this._logoUploadUrl;
  }

  public get headerEditing(): HeaderEditingVm {
    return this._headerEditing;
  }

  public get mainInfoEditing(): MainInfoEditingVm {
    return this._mainInfoEditing;
  }

  public get aboutEventEditing(): AboutEventEditingVm {
    return this._aboutEventEditing;
  }

  public get isSomethingEditing(): boolean {
    return !!(this._headerEditing || this._mainInfoEditing || this._aboutEventEditing);
  }

  public clearAllEditing(): void {
    this._headerEditing = null;
    this._mainInfoEditing = null;
    this._aboutEventEditing = null;
  }

  public editHeader(): void {
    this.clearAllEditing();
    this._headerEditing = new HeaderEditingVm(this._res.event, this._types, this._formats);
  }

  public editMainInfo(): void {
    this.clearAllEditing();
    this._mainInfoEditing = new MainInfoEditingVm(this._res.event, this._segments, this._citiesRes);
  }

  public editAboutEvent(): void {
    this.clearAllEditing();
    this._aboutEventEditing = new AboutEventEditingVm(this._res.event);
  }
}

@Component({
  selector: 'sk-event-details-page',
  templateUrl: './sk-event-details-page.component.html',
  styleUrls: ['./sk-event-details-page.component.scss']
})
export class SkEventDetailsPageComponent implements OnInit, OnDestroy {
  private _disposedSubj: Subject<void> = new Subject();

  private _location: Location;
  private _router: Router;
  private _activatedRoute: ActivatedRoute;

  private _security: Security;
  private _detailsProvider: EventDetailsProvider;
  private _detailsUpdator: EventDetailsUpdator;
  private _connectionsManager: ConnectionsManager;
  private _typesDirectory: EventTypesDirectory;
  private _formatsDirectory: EventFormatsDirectory;
  private _segmentsDirectory: SegmentsDirectory;
  private _citiesDirectory: CitiesDirectory;

  private _updatedSubj: Subject<void> = new BehaviorSubject<void>(null);

  private _vm: Vm;
  private _notFound: boolean = false;

  public constructor(
    location: Location,
    router: Router,
    activatedRoute: ActivatedRoute,
    security: Security,
    detailsProvider: EventDetailsProvider,
    detailsUpdator: EventDetailsUpdator,
    connectionsManager: ConnectionsManager,
    typesDirectory: EventTypesDirectory,
    formatsDirectory: EventFormatsDirectory,
    segmentsDirectory: SegmentsDirectory,
    citiesDirectory: CitiesDirectory
  ) {
    this._location = location;
    this._router = router;
    this._activatedRoute = activatedRoute;
    this._security = security;
    this._detailsProvider = detailsProvider;
    this._detailsUpdator = detailsUpdator;
    this._connectionsManager = connectionsManager;
    this._typesDirectory = typesDirectory;
    this._formatsDirectory = formatsDirectory;
    this._segmentsDirectory = segmentsDirectory;
    this._citiesDirectory = citiesDirectory;
  }

  public get vm(): Vm {
    return this._vm;
  }

  public get notFound(): boolean {
    return this._notFound;
  }

  public ngOnInit(): void {
    combineLatest([
      combineLatest(
        this._activatedRoute.paramMap.pipe(
          map(params => parseInt(params.get("id")))
        ),
        this._updatedSubj,
      ).pipe(
        tap(() => { this._notFound = false; }),
        flatMap(([id]) => this._detailsProvider.get({ eventId: id }))
      ),
      this._security.currentUserData$,
      from(this._typesDirectory.getAll()),
      from(this._formatsDirectory.getAll()),
      from(this._segmentsDirectory.getAll()),
      from(this._citiesDirectory.getAll()),
    ])
      .pipe(takeUntil(this._disposedSubj))
      .subscribe(([res, currentUserData, types, formats, segments, cities, x]: [Res, CurrentUserDataRes, EventTypesRes, EventFormatsRes, SegmentsRes, CitiesRes, any]) => {
        this._vm = new Vm(res, currentUserData, types, formats, segments, cities);

        if (this._vm.event) {
          if (this._vm.event.isPublic) {
            this._security.setNoIndex();
          } else {
            this._security.removeNoIndex();
          }
        }

      }, (err) => {
        this._notFound = true;
      });
  }

  public ngOnDestroy(): void {
    this._security.removeNoIndex();

    this._disposedSubj.next(null);
    this._disposedSubj.complete();
  }

  public async connect(vacancyId: number): Promise<void> {
    if (!(await this._security.enshureSignedIn())) {
      return Promise.resolve(null);
    }

    try {
      await this._connectionsManager.registerExpertToVacancyConnection({
        expertProfileId: this._security.currentUserData.expertProfile.id,
        vacancyId: vacancyId,
      });
    } catch (e) {
      if (e instanceof AmountIsFullError || e instanceof AlreadyConnectedError) {
      }
      else {
        throw e;
      }
    }

    this._updatedSubj.next(null);
  }

  public submit(form: NgForm): void {
    form.ngSubmit.emit(form ? form.value : null);
  }

  public async onHeaderSubmit($event, form: NgForm): Promise<void> {
    try {
      await this._detailsUpdator.updateHeader({
        eventId: this._vm.event.id,
        name: this._vm.headerEditing.name,
        typeId: this._vm.headerEditing.typeId,
        formatId: this._vm.headerEditing.formatId,
      });
      this._vm.clearAllEditing();
      this._updatedSubj.next();
    } catch (e) {
      if (e instanceof EventIsNotPublishedError) {

      } else {
        throw e;
      }
    }
  }

  public async onMainInfoSubmit($event, form: NgForm): Promise<void> {
    if (form.invalid) {
      return;
    }

    await this._detailsUpdator.updateMainInfo({
      eventId: this._vm.event.id,
      startTime: this._vm.mainInfoEditing.startTime,
      endTime: this._vm.mainInfoEditing.endTime,
      segmentId: this._vm.mainInfoEditing.segmentId,
      cityId: this._vm.mainInfoEditing.cityId,
      address: this._vm.mainInfoEditing.address,
      estimatedGuestsCount: this._vm.mainInfoEditing.estimatedGuestsCount,
      estimatedAverageCheck: this._vm.mainInfoEditing.estimatedAverageCheck,
      withDelivery: this._vm.mainInfoEditing.withtDelivery,
      withAccomodation: this._vm.mainInfoEditing.withAccomodation,
    });

    this._vm.clearAllEditing();
    this._updatedSubj.next();
  }

  public async onAboutEventSubmit($event, form: NgForm): Promise<void> {
    await this._detailsUpdator.updateAboutEvent({
      eventId: this._vm.event.id,
      aboutEventHtml: this._vm.aboutEventEditing.html
    });

    this._vm.clearAllEditing();
    this._updatedSubj.next();
  }

  public async onUploaded($event): Promise<void> {
    this._updatedSubj.next();
  }

  public async onPublishClick(): Promise<void> {
    try {
      await this._detailsUpdator.publish({ eventId: this._vm.event.id });
      this._updatedSubj.next();
    } catch (e) {
      if (e instanceof EventIsNotPublishedError) {

      } else {
        throw e;
      }
    }
  }

  public async onUnpublishClick(): Promise<void> {
    try {
      await this._detailsUpdator.unpublish({ eventId: this._vm.event.id });
      this._updatedSubj.next();
    } catch (e) {
      if (e instanceof EventIsNotPublishedError) {

      } else {
        throw e;
      }
    }
  }

  public async onIsPublicChanged(value: boolean): Promise<void> {
    if (value != this._vm.event.isPublic) {
      if (value) {
        await this._detailsUpdator.makePublic({ eventId: this._vm.event.id });
      } else {
        await this._detailsUpdator.makePrivate({ eventId: this._vm.event.id });
      }

      this._vm.clearAllEditing();
      this._updatedSubj.next();
    }
  }

  public async onDeleteClick(): Promise<void> {
    await this._detailsUpdator.delete({ eventId: this._vm.event.id });
    this._location.back();
  }

  public async onRegisterVacancyClick(): Promise<void> {
    try {
      var res = await this._detailsUpdator.registerVacancy({ eventId: this._vm.event.id });
      this._updatedSubj.next();
      await this._router.navigate(['/vacancies', res.vacancyId]);
    } catch (e) {
      if (e instanceof EventIsNotPublishedError) {

      } else {
        throw e;
      }
    }
  }
}
