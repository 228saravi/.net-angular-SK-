import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { ActivatedRoute, ParamMap } from '@angular/router';

import { debounceTime, flatMap, tap, takeUntil } from 'rxjs/operators';

import { SkPublishBeforeConnectPopupComponent } from '../sk-publish-before-connect-popup/sk-publish-before-connect-popup.component';
import { SkConnectCompanyToExpertPopupComponent } from '../sk-connect-company-to-expert-popup/sk-connect-company-to-expert-popup.component';
import { ExpertsFilterData } from '../sk-experts-filter/sk-experts-filter.component';

import { Security } from '../../_services/sk-security.service';
import { ExpertsSearcher, Req, Res, SkillsGroup } from '../../_services/sk-domain-experts-searcher.service';
import { ConnectionsManager } from '../../_services/sk-connections-manager.service';
import { Subject } from 'rxjs';

@Component({
  selector: 'sk-experts-search-page',
  templateUrl: './sk-experts-search-page.component.html',
  styleUrls: ['./sk-experts-search-page.component.scss']
})
export class SkExpertsSearchPageComponent implements OnInit, OnDestroy {
  @ViewChild("connectCompanyToExpertPopup")
  public connectCompanyToExpertPopup: SkConnectCompanyToExpertPopupComponent;

  @ViewChild("publishBeforeConnectPopup")
  public publishBeforeConnectPopup: SkPublishBeforeConnectPopupComponent;

  private _activatedRoute: ActivatedRoute;

  private _security: Security;
  private _searcher: ExpertsSearcher;

  private _pageSize: number = 20;

  private _searchResult: Res;

  private _disposedSubj: Subject<void> = new Subject();

  private createReq(paramsMap: ParamMap): Req {
    var filterData = ExpertsFilterData.fromParamMap(paramsMap);
    var req = new Req(
      null,
      filterData.cityId,
      filterData.maxRatePerHour,
      filterData.specialitiesSelectionData.selectedSpecialitiesIds,
      filterData.specialitiesSelectionData.selectedSpecializationsIds,
      filterData.specialitiesSelectionData.selectedSkillsIds,
      0,
      this._pageSize,
    );
    return req;
  }

  public constructor(activatedRoute: ActivatedRoute, searcher: ExpertsSearcher, security: Security) {
    this._activatedRoute = activatedRoute;

    this._security = security;
    this._searcher = searcher;
  }

  public get searchResult(): Res {
    return this._searchResult;
  }

  public async ngOnInit(): Promise<void> {
    this._activatedRoute.queryParamMap
      .pipe(
        tap(() => { this._searchResult = null; }),
        flatMap(paramMap => this._searcher.search(this.createReq(paramMap))),
        takeUntil(this._disposedSubj)
      )
      .subscribe((res) => {
        this._searchResult = res;
      });
  }

  public async ngOnDestroy(): Promise<void> {
    this._disposedSubj.next(null);
    this._disposedSubj.complete();
  }

  public async loadMore(): Promise<void> {
    var req = this.createReq(this._activatedRoute.snapshot.queryParamMap);
    req.skip = this._searchResult.experts.length;
    req.take = this._pageSize;
    var newRes = await this._searcher.search(req);
    newRes.experts = [...this._searchResult.experts, ...newRes.experts];
    this._searchResult = newRes;
  }

  public async onConnectClick(expertProfileId: number): Promise<void> {
    this.connectCompanyToExpertPopup.show(expertProfileId, this._security.currentUserData.company.id);
  }

}
