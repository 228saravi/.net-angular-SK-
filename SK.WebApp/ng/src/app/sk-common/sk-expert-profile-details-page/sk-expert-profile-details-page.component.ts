import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgForm } from '@angular/forms';

import { Observable, combineLatest, from, Subject, BehaviorSubject } from 'rxjs';
import { map, flatMap, tap, takeUntil } from 'rxjs/operators';

import { SkConnectCompanyToExpertPopupComponent } from '../sk-connect-company-to-expert-popup/sk-connect-company-to-expert-popup.component';

import { ExpertProfileDetailsProvider, Res, Specialization, ExpertProfileRes } from '../../_services/sk-domain-expert-profile-details-provider.service';
import { LanguagesDirectory, Language, Res as LanguagesRes } from '../../_services/sk-domain-languages-directory.service';
import { SkillsMatrixDirectory, SkillsMatrix } from '../../_services/sk-domain-skills-matrix-directory.service';
import { ExpertProfileDetailsUpdator, ProfileIsNotPublishedError } from '../../_services/sk-domain-expert-profile-details-updator.service';
import { ClothingSizesDirectory, ClothingSize } from '../../_services/sk-domain-clothing-sizes-directory.service';
import { ExpertDocumentsDirectory, Document } from '../../_services/sk-domain-clothing-expert-documents-directory.service';
import { ExperienceOptionsDirectory, Res as ExperienceOptionsRes, ExperienceOption } from '../../_services/sk-domain-experience-options-directory.service';
import { CitiesDirectory, Res as CitiesRes, City } from '../../_services/sk-domain-cities-directory.service';
import { Security, CurrentUserDataRes } from '../../_services/sk-security.service';

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
  public name: string;
  public ratePerHour: number;
  public selectedSpecialityId: string = null;
  public selectedSpecializationId: string = null;

  public skillsMatrix: SkillsMatrix = null;

  public constructor(profile: ExpertProfileRes, allCities: City[], skillsMatrix: SkillsMatrix) {
    this.name = profile.name;
    this.ratePerHour = profile.ratePerHour;

    if (profile.speciality) {
      this.selectedSpecialityId = profile.speciality.id;

      if (profile.speciality.specialization) {
        this.selectedSpecializationId = profile.speciality.specialization.id;
      }
    }

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
  public experienceOptionId: string;
  public allExperienceOptions: ExperienceOption[] = [];

  public selectedCityId: string;
  public allCities: City[] = [];

  public languages: LanguageSelectionVm[] = [];
  public skillsGroups: SkillsGroupSelectionVm[] = [];

  public constructor(
    profile: ExpertProfileRes,
    allCities: City[],
    allLanguages: Language[],
    skillsMatrix: SkillsMatrix,
    experienceOptions: ExperienceOption[]
  ) {
    this.experienceOptionId = profile.experience ? profile.experience.id : null;
    this.allExperienceOptions = experienceOptions;

    this.selectedCityId = profile.city != null ? profile.city.id : null;
    this.allCities = allCities;

    this.languages = allLanguages.map((l): LanguageSelectionVm => {
      return {
        id: l.id,
        name: l.name,
        selected: profile.languages.some(ll => ll.id == l.id)
      };
    });

    var speciality = skillsMatrix.specialities.find(s => s.id == profile.speciality.id);
    if (speciality) {

      var skillsFromRes = profile.speciality.skillsGroups
        .map(sg1 => sg1.skills).reduce((skills, chunk) => [...skills, ...chunk], []);

      this.skillsGroups = speciality.skillsGroups.map((sg): SkillsGroupSelectionVm => {
        return {
          name: sg.name,
          skills: sg.skills.map((sk): SkillSelectionVm => {
            return {
              id: sk.id,
              name: sk.name,
              selected: profile.speciality.skillsGroups
                .map(sg1 => sg1.skills).reduce((skills, chunk) => [...skills, ...chunk], [])
                .some(sk1 => sk1.id == sk.id)
            }
          })
        };
      });
    }
  }
}

class ExtraInfoEditingVm {
  public selectedClothingSizeId: string = null;
  public ducumentSelectionVms: DocumentSelectionVm[] = [];

  public allClothingSizes: ClothingSize[] = [];

  public constructor(profile: ExpertProfileRes, allClothingSizes: ClothingSize[], allDocuments: Document[]) {
    this.selectedClothingSizeId = profile.clothingSize ? profile.clothingSize.id : null;

    this.allClothingSizes = allClothingSizes;

    this.ducumentSelectionVms = allDocuments.map((d): DocumentSelectionVm => {
      return {
        id: d.id,
        name: d.name,
        selected: profile.documents.some(dd => dd.id == d.id),
      };
    });
  }
}

class AboutMeEditingVm {
  public html: string;

  public constructor(profile: ExpertProfileRes) {
    this.html = profile.aboutMeHtml;
  }
}

class Vm {
  private _res: Res = null;
  private _currentUserData: CurrentUserDataRes;
  private _skillsMatrix: SkillsMatrix = null;
  private _languages: Language[] = [];
  private _cities: City[] = [];
  private _clothingSizes: ClothingSize[] = [];
  private _documents: Document[] = [];
  private _experienceOptions: ExperienceOption[] = [];

  private _photoUploadUrl: string = null;

  private _headerEditing: HeaderEditingVm = null;
  private _mainInfoEditing: MainInfoEditingVm = null;
  private _extraInfoEditing: ExtraInfoEditingVm = null;
  private _aboutMeEditing: AboutMeEditingVm = null;

  public constructor(
    res: Res,
    currentUserData: CurrentUserDataRes,
    skillsMatrix: SkillsMatrix,
    languages: Language[],
    cities: City[],
    clothingSizes: ClothingSize[],
    documents: Document[],
    experienceOptions: ExperienceOption[],
  ) {
    this._res = res;
    this._currentUserData = currentUserData;
    this._skillsMatrix = skillsMatrix;
    this._languages = languages;
    this._cities = cities;
    this._clothingSizes = clothingSizes;
    this._documents = documents;
    this._experienceOptions = experienceOptions;

    this._photoUploadUrl = `/api/ExpertProfileDetailsUpdator/UploadPhoto?expertProfileId=${res.expertProfile.id}`;
  }

  public get isMyProfile(): boolean {
    return this._currentUserData && this._currentUserData.expertProfile && this._currentUserData.expertProfile.id == this._res.expertProfile.id;
  }

  public get res(): Res {
    return this._res;
  }

  public get profile(): ExpertProfileRes {
    return this._res.expertProfile;
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

  public get aboutMeEditing(): AboutMeEditingVm {
    return this._aboutMeEditing;
  }

  public get isSomethingEditing(): boolean {
    return !!(this._headerEditing || this._mainInfoEditing || this._extraInfoEditing || this._aboutMeEditing);
  }

  public get photoUploadUrl(): string {
    return this._photoUploadUrl;
  }

  public clearAllEditing(): void {
    this._headerEditing = null;
    this._mainInfoEditing = null;
    this._extraInfoEditing = null;
    this._aboutMeEditing = null;
  }

  public editHeader(): void {
    this.clearAllEditing();
    this._headerEditing = new HeaderEditingVm(this._res.expertProfile, this._cities, this._skillsMatrix);
  }

  public editMainInfo(): void {
    this.clearAllEditing();
    this._mainInfoEditing = new MainInfoEditingVm(this._res.expertProfile, this._cities, this._languages, this._skillsMatrix, this._experienceOptions);
  }

  public editExtraInfo(): void {
    this.clearAllEditing();
    this._extraInfoEditing = new ExtraInfoEditingVm(this._res.expertProfile, this._clothingSizes, this._documents);
  }

  public editAboutMe(): void {
    this.clearAllEditing();
    this._aboutMeEditing = new AboutMeEditingVm(this._res.expertProfile);
  }


}

@Component({
  selector: 'sk-expert-profile-details-page',
  templateUrl: './sk-expert-profile-details-page.component.html',
  styleUrls: ['./sk-expert-profile-details-page.component.scss']
})
export class SkExpertProfileDetailsPageComponent implements OnInit, OnDestroy {
  @ViewChild("connectCompanyToExpertPopup")
  public connectCompanyToExpertPopup: SkConnectCompanyToExpertPopupComponent;

  private _activatedRoute: ActivatedRoute;

  private _security: Security;

  private _expertProfileDetailsProvider: ExpertProfileDetailsProvider;
  private _expertProfileDetailsUpdator: ExpertProfileDetailsUpdator;

  private _skillsMatrixDirectory: SkillsMatrixDirectory;
  private _languagesDirectory: LanguagesDirectory;
  private _citiesDirectory: CitiesDirectory;
  private _clothingSizesDirectory: ClothingSizesDirectory;
  private _expertDocumentsDirectory: ExpertDocumentsDirectory;
  private _experienceOptionsDirectory: ExperienceOptionsDirectory;

  private _updatedSubj: Subject<void> = new BehaviorSubject<void>(null);

  private _vm: Vm = null;
  private _notFound: boolean = false;

  private _disposedSubj: Subject<void> = new Subject();

  public constructor(
    activatedRoute: ActivatedRoute,
    security: Security,
    expertProfileDetailsProvider: ExpertProfileDetailsProvider,
    expertProfileDetailsUpdator: ExpertProfileDetailsUpdator,
    skillsMatrixDirectory: SkillsMatrixDirectory,
    languagesDirectory: LanguagesDirectory,
    citiesDirectory: CitiesDirectory,
    clothingSizesDirectory: ClothingSizesDirectory,
    expertDocumentsDirectory: ExpertDocumentsDirectory,
    experienceOptionsDirectory: ExperienceOptionsDirectory
  ) {
    this._activatedRoute = activatedRoute;

    this._security = security;

    this._expertProfileDetailsProvider = expertProfileDetailsProvider;
    this._expertProfileDetailsUpdator = expertProfileDetailsUpdator;

    this._skillsMatrixDirectory = skillsMatrixDirectory;
    this._languagesDirectory = languagesDirectory;
    this._citiesDirectory = citiesDirectory;
    this._clothingSizesDirectory = clothingSizesDirectory;
    this._expertDocumentsDirectory = expertDocumentsDirectory;
    this._experienceOptionsDirectory = experienceOptionsDirectory;
  }

  public get vm(): Vm {
    return this._vm;
  }

  public get notFound(): boolean {
    return this._notFound;
  }

  public get canCurrentUserConnect(): boolean {
    return !!this._security.currentUserData && !!this._security.currentUserData.company;
  }

  public async ngOnInit(): Promise<void> {

    combineLatest([
      combineLatest(
        this._activatedRoute.paramMap.pipe(
          tap(() => { this._notFound = false }),
          map(params => parseInt(params.get("id"))),
        ),
        this._updatedSubj,
      ).pipe(
        flatMap(([id]) => this._expertProfileDetailsProvider.get({ expertProfileId: id })),
      ),
      this._security.currentUserData$,
      from(this._skillsMatrixDirectory.get()),
      from(this._languagesDirectory.getAll()),
      from(this._citiesDirectory.getAll()),
      from(this._clothingSizesDirectory.getAll()),
      from(this._expertDocumentsDirectory.getAll()),
      from(this._experienceOptionsDirectory.getAll()),
    ])
      .pipe(takeUntil(this._disposedSubj))
      .subscribe(([res, currentUserData, skillsMatrix, languages, cities, clothingSizes, documents, expOptions, x]: [Res, CurrentUserDataRes, SkillsMatrix, LanguagesRes, CitiesRes, any, any, ExperienceOptionsRes, any]) => {
      this._vm = new Vm(res, currentUserData, skillsMatrix, languages.languages, cities.cities, clothingSizes.clothingSizes, documents.documents, expOptions.experienceOptions);
    }, (err) => {
      this._notFound = true;
    });
  }

  public ngOnDestroy(): void {
    this._disposedSubj.next(null);
    this._disposedSubj.complete();

    this._security.removeNoIndex();
  }

  public submit(form: NgForm): void {
    form.ngSubmit.emit(form ? form.value : null);
  }

  public async onHeaderSubmit($event, form: NgForm): Promise<void> {
    try {
      await this._expertProfileDetailsUpdator.updateHeader({
        expertProfileId: this._vm.res.expertProfile.id,
        name: this._vm.headerEditing.name,
        ratePerHour: this._vm.headerEditing.ratePerHour,
        specialityId: this._vm.headerEditing.selectedSpecialityId,
        specializationId: this._vm.headerEditing.selectedSpecializationId,
      });
      this._vm.clearAllEditing();
      this._updatedSubj.next();
    } catch (e) {
      if (e instanceof ProfileIsNotPublishedError) {

      } else {
        throw e;
      }
    }
  }

  public async onMainInfoSubmit($event, form: NgForm): Promise<void> {
    try {
      await this._expertProfileDetailsUpdator.updateMainInfo({
        expertProfileId: this._vm.res.expertProfile.id,
        cityId: this._vm.mainInfoEditing.selectedCityId,
        experienceOptionId: this._vm.mainInfoEditing.experienceOptionId,
        languagesIds: this._vm.mainInfoEditing.languages.filter(l => l.selected).map(l => l.id),
        skillsIds: this._vm.mainInfoEditing.skillsGroups.map(s => s.skills).reduce((arr, chunk) => [...arr, ...chunk], []).filter(s => s.selected).map(s => s.id)
      });
      this._vm.clearAllEditing();
      this._updatedSubj.next();
    } catch (e) {
      if (e instanceof ProfileIsNotPublishedError) {

      } else {
        throw e;
      }
    }
  }

  public async onExtraInfoSubmit($event, form: NgForm): Promise<void> {
    try {
      await this._expertProfileDetailsUpdator.updateExtraInfo({
        expertProfileId: this._vm.res.expertProfile.id,
        clothingSizeId: this._vm.extraInfoEditing.selectedClothingSizeId,
        expertDocumentsIds: this._vm.extraInfoEditing.ducumentSelectionVms.filter(d => d.selected).map(d => d.id),
      });
      this._vm.clearAllEditing();
      this._updatedSubj.next();
    } catch (e) {
      if (e instanceof ProfileIsNotPublishedError) {

      } else {
        throw e;
      }
    }
  }

  public async onAboutMeSubmit($event, form: NgForm): Promise<void> {
    try {
      await this._expertProfileDetailsUpdator.updateAboutMe({ expertProfileId: this._vm.res.expertProfile.id, aboutMeHtml: this._vm.aboutMeEditing.html });
      this._vm.clearAllEditing();
      this._updatedSubj.next();
    } catch (e) {
      if (e instanceof ProfileIsNotPublishedError) {

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
      await this._expertProfileDetailsUpdator.publish({
        expertProfileId: this._vm.res.expertProfile.id
      });
    } catch (e) {
      if (e instanceof ProfileIsNotPublishedError) {

      } else {
        throw e;
      }
    }

    this._updatedSubj.next();
  }

  public async onUnpublishClick(): Promise<void> {
    try {
      await this._expertProfileDetailsUpdator.unpublish({
        expertProfileId: this._vm.res.expertProfile.id
      });
    } catch (e) {
    }

    this._updatedSubj.next();
  }

  public async onConnectClick(expertProfileId: number): Promise<void> {
    this.connectCompanyToExpertPopup.show(expertProfileId, this._security.currentUserData.company.id);
  }
}
