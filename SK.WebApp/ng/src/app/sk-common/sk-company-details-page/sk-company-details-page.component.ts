import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgForm } from '@angular/forms';

import { Subject, BehaviorSubject, combineLatest, from } from 'rxjs';
import { map, flatMap, tap, takeUntil } from 'rxjs/operators';

import { CompanyDetailsProvider, Res, Company, Event } from '../../_services/sk-domain-company-details-provider.service';
import { CompanyDetailsUpdator, CompanyIsNotPublishedError } from '../../_services/sk-domain-company-details-updator.service';
import { CitiesDirectory, Res as CitiesRes, City } from '../../_services/sk-domain-cities-directory.service';
import { CurrentUserDataRes, Security } from '../../_services/sk-security.service';

class HeaderEditingVm {
  public companyId: number;
  public name: string;

  public selectedCityId: string;
  public allCities: City[] = [];

  public constructor(company: Company, allCities: City[]) {
    this.companyId = company.id;
    this.name = company.name;
    this.selectedCityId = company.city != null ? company.city.id : null;
    this.allCities = allCities;
  }
}

class AboutCompanyEditingVm {
  public html: string;

  public constructor(company: Company) {
    this.html = company.aboutCompanyHtml;
  }
}

class Vm {
  private _now: Date = new Date(new Date().setSeconds(0));

  private _res: Res;
  private _currentUserData: CurrentUserDataRes;
  private _allCities: City[] = [];

  private _logoUploadUrl: string = null;

  private _headerEditing: HeaderEditingVm = null;
  private _aboutCompanyEditing: AboutCompanyEditingVm = null;

  public constructor(res: Res, currentUserData: CurrentUserDataRes, allCities: City[]) {
    this._res = res;
    this._currentUserData = currentUserData;
    this._allCities = allCities;

    this._logoUploadUrl = `/api/CompanyDetailsUpdator/UploadLogo?companyId=${res.company.id}`;
  }

  public get isMyCompany(): boolean {
    return this._currentUserData && this._currentUserData.company && this._currentUserData.company.id == this._res.company.id;
  }

  public get company(): Company {
    return this._res.company;
  }

  public get logoUploadUrl(): string {
    return this._logoUploadUrl;
  }

  public get headerEditing(): HeaderEditingVm {
    return this._headerEditing;
  }

  public get aboutCompanyEditing(): AboutCompanyEditingVm {
    return this._aboutCompanyEditing;
  }

  public get isSomethingEditing(): boolean {
    return !!(this._headerEditing || this._aboutCompanyEditing);
  }

  public clearAllEditing(): void {
    this._headerEditing = null;
    this._aboutCompanyEditing = null;
  }

  public editHeader(): void {
    this.clearAllEditing();
    this._headerEditing = new HeaderEditingVm(this._res.company, this._allCities);
  }

  public editAboutCompany(): void {
    this.clearAllEditing();
    this._aboutCompanyEditing = new AboutCompanyEditingVm(this._res.company);
  }

  public isEventOver(event: Event): boolean {
    return event.endTime && this._now.valueOf() > event.endTime.valueOf();
  }
}

@Component({
  selector: 'sk-company-details-page',
  templateUrl: './sk-company-details-page.component.html',
  styleUrls: ['./sk-company-details-page.component.scss']
})
export class SkCompanyDetailsPageComponent implements OnInit, OnDestroy {
  private _router: Router;
  private _activatedRoute: ActivatedRoute;
  private _security: Security;
  private _detailsProvider: CompanyDetailsProvider;
  private _detailsUpdator: CompanyDetailsUpdator;
  private _citiesDirectory: CitiesDirectory;

  private _updatedSubj: Subject<void> = new BehaviorSubject<void>(null);

  private _vm: Vm;
  private _notFound: boolean = false;

  private _disposedSubj: Subject<void> = new Subject();

  public constructor(
    router: Router,
    activatedRoute: ActivatedRoute,
    security: Security,
    detailsProvider: CompanyDetailsProvider,
    detailsUpdator: CompanyDetailsUpdator,
    citiesDirectory: CitiesDirectory
  ) {
    this._router = router;
    this._activatedRoute = activatedRoute;
    this._security = security;
    this._detailsProvider = detailsProvider;
    this._detailsUpdator = detailsUpdator;
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
          map(params => parseInt(params.get("id"))),
        ),
        this._updatedSubj,
      ).pipe(
        tap(() => { this._notFound = false; }),
        flatMap(([id]) => this._detailsProvider.get({ companyId: id, withVacancies: false })),
      ),
      this._security.currentUserData$,
      from(this._citiesDirectory.getAll()),
    ])
      .pipe(takeUntil(this._disposedSubj))
      .subscribe(([res, currentUserData, cities, x]: [Res, CurrentUserDataRes, CitiesRes, any]) => {
        if (res.company) {
          this._vm = new Vm(res, currentUserData, cities.cities);
        } else {
          this._notFound = true;
        }
      }, (err) => {
        this._notFound = true;
      });
  }

  public ngOnDestroy(): void {
    this._disposedSubj.next(null);
    this._disposedSubj.complete();
  }

  public submit(form: NgForm): void {
    form.ngSubmit.emit(form ? form.value : null);
  }

  public async onHeaderSubmit($event, form: NgForm): Promise<void> {
    try {
      await this._detailsUpdator.updateHeader({
        companyId: this._vm.headerEditing.companyId,
        name: this._vm.headerEditing.name,
        cityId: this._vm.headerEditing.selectedCityId
      });
      this._vm.clearAllEditing();
      this._updatedSubj.next();
    } catch (e) {
      if (e instanceof CompanyIsNotPublishedError) {

      } else {
        throw e;
      }
    }

  }

  public async onAboutCompanySubmit($event, form: NgForm): Promise<void> {
    try {
      await this._detailsUpdator.updateAboutCompany({
        companyId: this._vm.company.id,
        aboutCompanyHtml: this._vm.aboutCompanyEditing.html
      });
      this._vm.clearAllEditing();
      this._updatedSubj.next(null);
    } catch (e) {
      if (e instanceof CompanyIsNotPublishedError) {

      } else {
        throw e;
      }
    }

  }

  public async onUploaded($event): Promise<void> {
    // Notify security service about changes.
    {
      var subj = (this._security as any)._userSignedInSubj as Subject<void>;
      subj.next();
    }

    this._updatedSubj.next();
  }

  public async onPublishClick(): Promise<void> {
    try {
      await this._detailsUpdator.publish({ companyId: this._vm.company.id });
    } catch (e) {
      if (e instanceof CompanyIsNotPublishedError) {

      } else {
        throw e;
      }
    }

    this._updatedSubj.next();
  }

  public async onRegisterEventClick(): Promise<void> {
    try {
      var res = await this._detailsUpdator.registerEvent({ companyId: this._vm.company.id });
      this._updatedSubj.next(null);
      await this._router.navigate(['/events', res.eventId]);
    } catch (e) {
      if (e instanceof CompanyIsNotPublishedError) {

      } else {
        throw e;
      }
    }
  }

}
