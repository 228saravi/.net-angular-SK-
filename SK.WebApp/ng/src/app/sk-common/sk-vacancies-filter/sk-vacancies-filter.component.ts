import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { HttpParams } from '@angular/common/http';
import { SpecialitiesSelectionData } from '../sk-specialities-selections/sk-specialities-selections.component';
import { CitiesDirectory, City } from '../../_services/sk-domain-cities-directory.service';
import { takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';

export class VacanciesFilterData {
  public cityId: string = null;
  public minRatePerHour: number;
  public specialitiesSelectionData: SpecialitiesSelectionData;

  public constructor(specialitiesSelectionData: SpecialitiesSelectionData) {
    this.specialitiesSelectionData = specialitiesSelectionData;
  }

  public static fromParamMap(paramMap: ParamMap): VacanciesFilterData {
    var vacanciesFilterData = new VacanciesFilterData(SpecialitiesSelectionData.fromParamMap(paramMap));
    vacanciesFilterData.cityId = paramMap.get("cityId");
    vacanciesFilterData.minRatePerHour = parseInt(paramMap.get("minRatePerHour")) || null;
    return vacanciesFilterData;
  }

  public toHttpParams(): HttpParams {
    var params = new HttpParams();

    if (this.cityId) {
      params = params.append("cityId", this.cityId);
    }

    if (this.minRatePerHour) {
      params = params.append("minRatePerHour", this.minRatePerHour.toString());
    }

    var specialitiesParams = this.specialitiesSelectionData.toHttpParams();
    for (let key of specialitiesParams.keys()) {
      for (let val of specialitiesParams.getAll(key)) {
        params = params.append(key, val);
      }
    }

    return params;
  }
}

class Vm {
  allCities: City[] = [];
  vacanciesFilterData: VacanciesFilterData;
}

@Component({
  selector: 'sk-vacancies-filter',
  templateUrl: './sk-vacancies-filter.component.html',
  styleUrls: ['./sk-vacancies-filter.component.scss']
})
export class SkVacanciesFilterComponent implements OnInit, OnDestroy {
  private _router: Router;
  private _activatedRoute: ActivatedRoute;

  private _citiesStore: CitiesDirectory;

  private _vm: Vm;

  private _disposedSubj: Subject<void> = new Subject();

  public constructor(router: Router, activatedRoute: ActivatedRoute, citiesStore: CitiesDirectory) {
    this._router = router;
    this._activatedRoute = activatedRoute;
    this._citiesStore = citiesStore;
  }

  public get vm(): Vm {
    return this._vm;
  }

  public async ngOnInit(): Promise<void> {
    this._vm = new Vm();

    this._activatedRoute.queryParamMap
      .pipe(takeUntil(this._disposedSubj))
      .subscribe(params => {
        this._vm.vacanciesFilterData = VacanciesFilterData.fromParamMap(params);
      });

    this._vm.allCities = (await this._citiesStore.getAll()).cities;
  }

  public async ngOnDestroy(): Promise<void> {
    this._disposedSubj.next(null);
    this._disposedSubj.complete();
  }

  public onSearchClicked(): void {
    var httpParams = this._vm.vacanciesFilterData.toHttpParams();
    var paramsObj = {};

    for (let key of httpParams.keys()) {
      paramsObj[key] = httpParams.getAll(key);
    }

    this._router.navigate([], { queryParams: paramsObj });
  }
}
