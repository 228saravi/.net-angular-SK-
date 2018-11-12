import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { HttpClient, HttpParams } from '@angular/common/http';
import { SpecialitiesSelectionData } from '../sk-specialities-selections/sk-specialities-selections.component';
import { CitiesDirectory, City } from '../../_services/sk-domain-cities-directory.service';
import { forEach } from '@angular/router/src/utils/collection';
import { takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';

export class ExpertsFilterData {
  public cityId: string = null;
  public maxRatePerHour: number;
  public specialitiesSelectionData: SpecialitiesSelectionData;

  public constructor(specialitiesSelectionData: SpecialitiesSelectionData) {
    this.specialitiesSelectionData = specialitiesSelectionData;
  }

  public static fromParamMap(paramMap: ParamMap): ExpertsFilterData {
    var vacanciesFilterData = new ExpertsFilterData(SpecialitiesSelectionData.fromParamMap(paramMap));
    vacanciesFilterData.cityId = paramMap.get("cityId");
    vacanciesFilterData.maxRatePerHour = parseInt(paramMap.get("maxRatePerHour")) || null;
    return vacanciesFilterData;
  }

  public toHttpParams(): HttpParams {
    var params = new HttpParams({});

    if (this.cityId) {
      params = params.append("cityId", this.cityId);
    }

    if (this.maxRatePerHour) {
      params = params.append("maxRatePerHour", this.maxRatePerHour.toString());
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
  expertsFilterData: ExpertsFilterData;
}

@Component({
  selector: 'sk-experts-filter',
  templateUrl: './sk-experts-filter.component.html',
  styleUrls: ['./sk-experts-filter.component.scss']
})
export class SkExpertsFilterComponent implements OnInit, OnDestroy {
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
      this._vm.expertsFilterData = ExpertsFilterData.fromParamMap(params);
    });

    this._vm.allCities = (await this._citiesStore.getAll()).cities;
  }

  public async ngOnDestroy(): Promise<void> {
    this._disposedSubj.next(null);
    this._disposedSubj.complete();
  }

  public onSearchClicked(): void {
    var httpParams = this._vm.expertsFilterData.toHttpParams();
    var paramsObj = {};

    for (let key of httpParams.keys()) {
      paramsObj[key] = httpParams.getAll(key);
    }

    this._router.navigate([], { queryParams: paramsObj });
  }
}
