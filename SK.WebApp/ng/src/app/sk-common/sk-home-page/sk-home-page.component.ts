import { Component, OnInit } from '@angular/core';
import { Security } from '../../_services/sk-security.service';
import { Router } from '@angular/router';

import { VacanciesFilterData } from '../sk-vacancies-filter/sk-vacancies-filter.component';
import { ExpertsFilterData } from '../sk-experts-filter/sk-experts-filter.component';
import { SpecialitiesSelectionData } from '../sk-specialities-selections/sk-specialities-selections.component';
import { locale } from 'devextreme/localization';

type Step = "selectType" | "selectSpeciality" | "selectCity";
type UserType = "expert" | "company";

class SelectTypeStepVm {
  public type: UserType = null;
}

class SelectSpecialityStepVm {
  public specialityId: string;
}

class SelectCityStepVm {
  public cityId: string;
}

class StepsVm {
  public step: Step = "selectType";

  public selectTypeStep: SelectTypeStepVm = new SelectTypeStepVm();
  public selectSpecialityStep: SelectSpecialityStepVm = new SelectSpecialityStepVm();
  public selectCityStep: SelectCityStepVm = new SelectCityStepVm();
}

@Component({
  selector: 'sk-home-page',
  templateUrl: './sk-home-page.component.html',
  styleUrls: ['./sk-home-page.component.scss']
})
export class SkHomePageComponent implements OnInit {
  private _router: Router;
  private _security: Security;

  private _vm: StepsVm = new StepsVm();

  public constructor(router: Router, security: Security) {
    this._router = router;
    this._security = security;
  }

  public get vm(): StepsVm {
    return this._vm;
  }

  public async ngOnInit(): Promise<void> {
    var u = this._security.currentUserData;

    if (!u) {
      return;
    }

    if (u.company) {
      await this._router.navigate(['/experts']);
      return;
    }

    if (u.expertProfile) {
      await this._router.navigate(['/vacancies']);
      return;
    }
  }

  public async onReadyClick(): Promise<void> {

    if (this._vm.selectTypeStep.type == "expert") {

      var vacFilterData = new VacanciesFilterData(new SpecialitiesSelectionData());
      vacFilterData.specialitiesSelectionData.selectedSpecialitiesIds = [this._vm.selectSpecialityStep.specialityId];
      vacFilterData.cityId = this._vm.selectCityStep.cityId;

      var httpParams = vacFilterData.toHttpParams();
      var paramsObj = {};

      for (let key of httpParams.keys()) {
        paramsObj[key] = httpParams.getAll(key);
      }

      this._router.navigate(["/vacancies"], { queryParams: paramsObj });

    } else {

      var signedIn = this._security.enshureSignedIn();
      this._security.signInPopup.vm.mode = 2;

      var res = await signedIn;

      if (!res) {
        return;
      }

      var expertsFilterData = new ExpertsFilterData(new SpecialitiesSelectionData());
      expertsFilterData.specialitiesSelectionData.selectedSpecialitiesIds = [this._vm.selectSpecialityStep.specialityId];
      expertsFilterData.cityId = this._vm.selectCityStep.cityId;

      var httpParams = expertsFilterData.toHttpParams();
      var paramsObj = {};

      for (let key of httpParams.keys()) {
        paramsObj[key] = httpParams.getAll(key);
      }

      this._router.navigate(["/experts"], { queryParams: paramsObj });

    }

  }

  public async onAuthorizeClicked(): Promise<void> {
    await this._security.enshureSignedIn();
    await this.ngOnInit();
  }

  public async onAboutUsClick(): Promise<void> {

    if (location) {
      location.href = "http://land.soulkitchen.family/";
    }

  }
}
