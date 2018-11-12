import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Location } from '@angular/common';
import { NgForm } from '@angular/forms';

import { Subject, BehaviorSubject, from, combineLatest } from 'rxjs';
import { map, filter, flatMap, takeUntil } from 'rxjs/operators';

import { Security, CurrentUserDataRes } from '../../_services/sk-security.service';
import { VacancyDetailsProvider, Res, Vacancy } from '../../_services/sk-domain-vacancy-details-provider.service';
import { VacancyDetailsUpdator, VacancyIsNotPublishedError } from '../../_services/sk-domain-vacancy-details-updator.service';
import { ConnectionsManager, AmountIsFullError, AlreadyConnectedError, NotYourConnectionError, TooLateToCancelConnectionError } from '../../_services/sk-connections-manager.service';

import { ConnectionStatuses } from '../../_classes/sk-domain-connection-statuses';
import { ConnectionTypes } from '../../_classes/sk-domain-connection-types';
import { SkillsMatrix, Speciality, Specialization, SkillsMatrixDirectory } from '../../_services/sk-domain-skills-matrix-directory.service';
import { LanguagesDirectory, Language, Res as LanguagesRes } from '../../_services/sk-domain-languages-directory.service';
import { ExpertDocumentsDirectory, Res as DocumentsRes, Document } from '../../_services/sk-domain-clothing-expert-documents-directory.service';
import { ExperienceOptionsDirectory, Res as ExperienceOptionsRes, ExperienceOption } from '../../_services/sk-domain-experience-options-directory.service';
import { ProfileIsNotPublishedError } from '../../_services/sk-domain-expert-profile-details-updator.service';
import { Meta, Title } from '@angular/platform-browser';

class LanguageSelectionVm {
  public id: string;
  public name: string;
  public selected: boolean;
}

class DocumentSelectionVm {
  public id: string;
  public name: string;
  public selected: boolean;
}

class SkillSelectionVm {
  public id: string;
  public name: string;
  public selected: boolean;
}

class SkillsGroupSelectionVm {
  public name: string;
  public skills: SkillSelectionVm[] = [];
}

class HeaderEditingVm {
  public selectedSpecialityId: string = null;
  public selectedSpecializationId: string = null;
  public ratePerHour: number = null;
  public amount: number = null;

  public skillsMatrix: SkillsMatrix = null;

  public constructor(vacancy: Vacancy, skillsMatrix: SkillsMatrix) {
    if (vacancy.speciality) {
      this.selectedSpecialityId = vacancy.speciality.id;

      if (vacancy.speciality.specialization) {
        this.selectedSpecializationId = vacancy.speciality.specialization.id;
      }
    }

    this.ratePerHour = vacancy.ratePerHour;
    this.amount = vacancy.amount;

    this.skillsMatrix = skillsMatrix;
  }

  public get specializationsForSelectedSpeciality(): Specialization[] {
    var speciality = this.skillsMatrix.specialities.find(s => s.id == this.selectedSpecialityId);

    if (!speciality) {
      return undefined; // Does not work with no undefined here.
    }

    return speciality.specializations;
  }
}

class MainInfoEditingVm {
  public startTime: Date;
  public workingHours: number;
  public experienceOptionId: string;

  public allExperienceOptions: ExperienceOption[] = [];

  public constructor(vac: Vacancy, allExperienceOptions: ExperienceOption[]) {
    this.startTime = vac.startTime;
    this.workingHours = vac.workingHours;
    this.experienceOptionId = vac.experience ? vac.experience.id : null;
    this.allExperienceOptions = allExperienceOptions;
  }
}

class ExtraInfoEditingVm {
  public languages: LanguageSelectionVm[] = [];
  public documents: DocumentSelectionVm[] = [];
  public skillsGroups: SkillsGroupSelectionVm[] = [];

  public constructor(vac: Vacancy, allLanguages: Language[], allDocuments: Document[], skillsMatrix: SkillsMatrix) {

    this.languages = allLanguages.map((l): LanguageSelectionVm => {
      return {
        id: l.id,
        name: l.name,
        selected: vac.languages.some(ll => ll.id == l.id)
      };
    });

    this.documents = allDocuments.map((d): DocumentSelectionVm => {
      return {
        id: d.id,
        name: d.name,
        selected: vac.documents.some(dd => dd.id == d.id)
      };
    })

    var speciality = vac.speciality
      ? skillsMatrix.specialities.find(s => s.id == vac.speciality.id)
      : null;

    if (speciality) {

      var skillsFromRes = vac.speciality.skillsGroups
        .map(sg1 => sg1.skills).reduce((skills, chunk) => [...skills, ...chunk], []);

      this.skillsGroups = speciality.skillsGroups.map((sg): SkillsGroupSelectionVm => {
        return {
          name: sg.name,
          skills: sg.skills.map((sk): SkillSelectionVm => {
            return {
              id: sk.id,
              name: sk.name,
              selected: vac.speciality.skillsGroups
                .map(sg1 => sg1.skills).reduce((skills, chunk) => [...skills, ...chunk], [])
                .some(sk1 => sk1.id == sk.id)
            }
          })
        };
      });
    }
  }
}

class AboutVacancyEditingVm {
  public html: string;

  public constructor(vacancy: Vacancy) {
    this.html = vacancy.aboutVacancyHtml;
  }
}

class Vm {
  private _now: Date = new Date(new Date().setSeconds(0));

  private _res: Res;
  private _currentUserData: CurrentUserDataRes;
  private _skillsMatrix: SkillsMatrix = null;
  private _allLanguages: Language[] = [];
  private _allDocuments: Document[] = [];
  private _allExperienceOptions: ExperienceOption[] = [];

  private _headerEditing: HeaderEditingVm = null;
  private _mainInfoEditing: MainInfoEditingVm = null;
  private _extraInfoEditing: ExtraInfoEditingVm = null;
  private _aboutVacancyEditing: AboutVacancyEditingVm = null;

  public constructor(res: Res, currentUserData: CurrentUserDataRes, skillsMatrix: SkillsMatrix, allLanguages: Language[], allDocuments: Document[], allExpOptions: ExperienceOption[]) {
    this._res = res;
    this._currentUserData = currentUserData;
    this._skillsMatrix = skillsMatrix;
    this._allLanguages = allLanguages;
    this._allDocuments = allDocuments;
    this._allExperienceOptions = allExpOptions;
  }

  public get now(): Date {
    return this._now;
  }

  public get vac(): Vacancy {
    return this._res.foundVacancy;
  }


  public get isCurrentUserAnExpert(): boolean {
    return this._currentUserData && this._currentUserData.expertProfile != null;
  }

  public get canCurrentUserConnecting(): boolean {
    return (!this._currentUserData || this.isCurrentUserAnExpert) &&
      (this.vac && !this.vac.connection && !this.isEventOver);
  }

  public get isMyVacancy(): boolean {
    return this._currentUserData && this._currentUserData.company && this._currentUserData.company.id == this._res.foundVacancy.event.company.id;
  }

  public get isEventOver(): boolean {
    return this._res.foundVacancy.event.endTime && this._now.valueOf() > this._res.foundVacancy.event.endTime.valueOf();
  }

  public get headerEditing(): HeaderEditingVm {
    return this._headerEditing;
  }

  public get mainInfoEditing(): MainInfoEditingVm {
    return this._mainInfoEditing;
  }

  public get extraInfoEditing(): ExtraInfoEditingVm {
    return this._extraInfoEditing;
  }

  public get aboutVacancyEditing(): AboutVacancyEditingVm {
    return this._aboutVacancyEditing;
  }

  public get isSomethingEditing(): boolean {
    return !!(this._headerEditing || this._mainInfoEditing || this._extraInfoEditing || this._aboutVacancyEditing);
  }

  public editHeader(): void {
    this.clearAllEditing();
    this._headerEditing = new HeaderEditingVm(this._res.foundVacancy, this._skillsMatrix);
  }

  public editMainInfo(): void {
    this.clearAllEditing();
    this._mainInfoEditing = new MainInfoEditingVm(this._res.foundVacancy, this._allExperienceOptions);
  }

  public editExtraInfo(): void {
    this.clearAllEditing();
    this._extraInfoEditing = new ExtraInfoEditingVm(this._res.foundVacancy, this._allLanguages, this._allDocuments, this._skillsMatrix);
  }

  public editAboutVacancy(): void {
    this.clearAllEditing();
    this._aboutVacancyEditing = new AboutVacancyEditingVm(this._res.foundVacancy);
  }

  public clearAllEditing(): void {
    this._headerEditing = null;
    this._mainInfoEditing = null;
    this._extraInfoEditing = null;
    this._aboutVacancyEditing = null;
  }
}

@Component({
  selector: 'sk-vacancy-details-page',
  templateUrl: './sk-vacancy-details-page.component.html',
  styleUrls: ['./sk-vacancy-details-page.component.scss']
})
export class SkVacancyDetailsPageComponent implements OnInit, OnDestroy {
  ConnectionTypes = ConnectionTypes; // Enum for template.
  ConnectionStatuses = ConnectionStatuses; // Enum for template.

  private _location: Location;
  private _router: Router;
  private _activatedRoute: ActivatedRoute;
  private _title: Title;
  private _meta: Meta;

  private _security: Security;
  private _connectionsManager: ConnectionsManager;
  private _vacancyDetailsProvider: VacancyDetailsProvider;
  private _vacancyDetailsUpdator: VacancyDetailsUpdator;
  private _skillsMatrixDirectory: SkillsMatrixDirectory;
  private _languagesDirectory: LanguagesDirectory;
  private _documentsDirectory: ExpertDocumentsDirectory;
  private _experienceOptionsDirectory: ExperienceOptionsDirectory;

  private _updatedSubj: Subject<void> = new BehaviorSubject<void>(null);

  private _vm: Vm;

  private _disposedSubj: Subject<void> = new Subject();

  private calcTitle(vac: Vacancy): string {

    if (vac.speciality == null) {
      return "Новая вакансия";
    }

    var spec = vac.speciality.specialization ? vac.speciality.specialization.name : vac.speciality.name;
    return `Вакансия: ${spec}`;
  }

  public constructor(
    location: Location,
    router: Router,
    activatedRoute: ActivatedRoute,
    title: Title,
    meta: Meta,
    security: Security,
    connectionsManager: ConnectionsManager,
    vacancyDetailsProvider: VacancyDetailsProvider,
    vacancyDetailsUpdator: VacancyDetailsUpdator,
    skillsMatrixDirectory: SkillsMatrixDirectory,
    languagesDirectory: LanguagesDirectory,
    documentsDirectory: ExpertDocumentsDirectory,
    experienceOptionsDirectory: ExperienceOptionsDirectory,
  ) {
    this._location = location;
    this._router = router;
    this._activatedRoute = activatedRoute;
    this._title = title;
    this._meta = meta;
    this._security = security;
    this._connectionsManager = connectionsManager;
    this._vacancyDetailsProvider = vacancyDetailsProvider;
    this._vacancyDetailsUpdator = vacancyDetailsUpdator;
    this._skillsMatrixDirectory = skillsMatrixDirectory;
    this._languagesDirectory = languagesDirectory;
    this._documentsDirectory = documentsDirectory;
    this._experienceOptionsDirectory = experienceOptionsDirectory;
  }

  public get vm(): Vm {
    return this._vm;
  }
   
  public ngOnInit(): void {

    combineLatest([
      combineLatest(
        this._activatedRoute.paramMap.pipe(
          map(params => parseInt(params.get("id")))
        ),
        this._updatedSubj,
      ).pipe(
        flatMap(([id]) => this._vacancyDetailsProvider.get({ vacancyId: id }))
      ),
      this._security.currentUserData$,
      from(this._skillsMatrixDirectory.get()),
      from(this._languagesDirectory.getAll()),
      from(this._documentsDirectory.getAll()),
      from(this._experienceOptionsDirectory.getAll()),
    ])
      .pipe(takeUntil(this._disposedSubj))
      .subscribe(([res, currentUserData, skillsMatrix, languages, documents, expOptions, x]: [Res, CurrentUserDataRes, SkillsMatrix, LanguagesRes, DocumentsRes, ExperienceOptionsRes, any]) => {
      this._vm = new Vm(res, currentUserData, skillsMatrix, languages.languages, documents.documents, expOptions.experienceOptions);

      if (this._vm.vac) {
        if (!this._vm.vac.isPublic) {
          this._security.setNoIndex();
        } else {
          this._security.removeNoIndex();
        }

        this._title.setTitle(this.calcTitle(this._vm.vac));

        this._meta.addTag({ name: "og:title", content: this.calcTitle(this._vm.vac) });
        this._meta.addTag({ name: "og:image", content: this._vm.vac.event.logoImageUrl });

      } else {

        this._meta.removeTag("og:title");
        this._meta.removeTag("og:image");
      }

    });

  }

  public ngOnDestroy(): void {
    this._disposedSubj.next(null);
    this._disposedSubj.complete();

    this._title.setTitle("Soulkitchen.family");
    this._security.removeNoIndex();
  }

  public submit(form: NgForm): void {
    form.ngSubmit.emit(form ? form.value : null);
  }

  public async onHeaderSubmit($event, form: NgForm): Promise<void> {
    try {
      await this._vacancyDetailsUpdator.updateHeader({
        vacancyId: this._vm.vac.id,
        specialityId: this._vm.headerEditing.selectedSpecialityId,
        specializationId: this._vm.headerEditing.selectedSpecializationId,
        amount: this._vm.headerEditing.amount,
        ratePerHour: this._vm.headerEditing.ratePerHour,
      });
      this._vm.clearAllEditing();
      this._updatedSubj.next();
    } catch (e) {
      if (e instanceof VacancyIsNotPublishedError) {

      } else {
        throw e
      }
    }
  }

  public async onMainInfoSubmit($event, form: NgForm): Promise<void> {
    try {
      if (form.invalid) {
        return;
      }
      await this._vacancyDetailsUpdator.updateMainInfo({
        vacancyId: this._vm.vac.id,
        startTime: this._vm.mainInfoEditing.startTime,
        workingHours: this._vm.mainInfoEditing.workingHours,
        experienceOptionId: this._vm.mainInfoEditing.experienceOptionId,
      });
      this._vm.clearAllEditing();
      this._updatedSubj.next();
    } catch (e) {
      if (e instanceof VacancyIsNotPublishedError) {

      } else {
        throw e
      }
    }
  }

  public async onExtraInfoSubmit($event, form: NgForm): Promise<void> {
    try {
      await this._vacancyDetailsUpdator.updateExtraInfo({
        vacancyId: this._vm.vac.id,
        languagesIds: this._vm.extraInfoEditing.languages.filter(l => l.selected).map(l => l.id),
        documentsIds: this._vm.extraInfoEditing.documents.filter(d => d.selected).map(d => d.id),
        skillsIds: this._vm.extraInfoEditing.skillsGroups.map(s => s.skills).reduce((arr, chunk) => [...arr, ...chunk], []).filter(s => s.selected).map(s => s.id)
      });
      this._vm.clearAllEditing();
      this._updatedSubj.next();
    } catch (e) {
      if (e instanceof VacancyIsNotPublishedError) {

      } else {
        throw e
      }
    }
  }

  public async onAboutVacancySubmit($event, form: NgForm): Promise<void> {
    try {
      await this._vacancyDetailsUpdator.updateAboutVacancy({
        vacancyId: this._vm.vac.id,
        aboutVacancyHtml: this._vm.aboutVacancyEditing.html,
      });
      this._vm.clearAllEditing();
      this._updatedSubj.next();
    } catch (e) {
      if (e instanceof VacancyIsNotPublishedError) {

      } else {
        throw e;
      }
    }
  }

  public async onPublishClick(): Promise<void> {
    try {
      await this._vacancyDetailsUpdator.publish({ vacancyId: this._vm.vac.id });
      this._vm.clearAllEditing();
      this._updatedSubj.next();
    } catch (e) {
      if (e instanceof VacancyIsNotPublishedError) {

      } else {
        throw e
      }
    }
  }

  public async onUnpublishClick(): Promise<void> {
    await this._vacancyDetailsUpdator.unpublish({ vacancyId: this._vm.vac.id });
    this._vm.clearAllEditing();
    this._updatedSubj.next();
  }

  public async onIsPublicChanged(value: boolean): Promise<void> {
    if (value != this._vm.vac.isPublic) {
      if (value) {
        await this._vacancyDetailsUpdator.makePublic({ vacancyId: this._vm.vac.id });
      } else {
        await this._vacancyDetailsUpdator.makePrivate({ vacancyId: this._vm.vac.id });
      }

      this._vm.clearAllEditing();
      this._updatedSubj.next();
    }
  }

  public async onDeleteClick(): Promise<void> {
    await this._vacancyDetailsUpdator.delete({ vacancyId: this._vm.vac.id });
    this._location.back();
  }

  public async connect(): Promise<void> {
    if (!(await this._security.enshureSignedIn())) {
      return Promise.resolve(null);
    }

    try {
      await this._connectionsManager.registerExpertToVacancyConnection({
        expertProfileId: this._security.currentUserData.expertProfile.id,
        vacancyId: this._vm.vac.id,
      });
    } catch (e) {
      if (e instanceof AmountIsFullError || e instanceof AlreadyConnectedError || e instanceof ProfileIsNotPublishedError) {
      }
      else {
        throw e;
      }
    }

    this._updatedSubj.next(null);
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

  public async approveConnection(): Promise<void> {
    if (!(await this._security.enshureSignedIn())) {
      return Promise.resolve(null);
    }

    try {
      await this._connectionsManager.approveConnection({ connectionId: this._vm.vac.connection.id });
    } catch (e) {
      //if (e instanceof NotYourConnectionError || e instanceof TooLateToCancelConnectionError) {
      //} else {
      //  throw e;
      //}

      throw e;
    }

    this._updatedSubj.next(null);
  }

  public message(connectionId: number): void {
    this._router.navigate(["/dialogs"], { queryParams: { connectionId: connectionId } });
  }
}
