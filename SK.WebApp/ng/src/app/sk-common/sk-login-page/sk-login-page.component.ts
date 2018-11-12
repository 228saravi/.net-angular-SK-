import { Component, OnInit, ViewChild, AfterViewChecked, AfterViewInit, OnDestroy } from '@angular/core';
import { SkSignInPopupComponent } from '../sk-sign-in-popup/sk-sign-in-popup.component';
import { Security } from '../../_services/sk-security.service';
import { ActivatedRoute, Router } from '@angular/router';
import { filter, takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';

@Component({
  selector: 'sk-login-page',
  templateUrl: './sk-login-page.component.html',
  styleUrls: ['./sk-login-page.component.scss']
})
export class SkLoginPageComponent implements OnInit, AfterViewInit, OnDestroy {

  private _router: Router;
  private _activatedRoute: ActivatedRoute;
  private _security: Security;

  private _unsubscribe$ = new Subject();

  public constructor(router: Router, activatedRoute: ActivatedRoute, security: Security) {
    this._router = router;
    this._activatedRoute = activatedRoute;
    this._security = security;
  }

  public ngOnInit() {
    this._security.currentUserData$
      .pipe(
        filter(x => !!x),
        takeUntil(this._unsubscribe$)
      ).subscribe(currentUserData => {
        var backUrl = this._activatedRoute.snapshot.queryParamMap.get("backUrl") || "/";
        this._router.navigateByUrl(backUrl);
      });
  }

  public ngAfterViewInit(): void {
    this._security.enshureSignedIn();
  }

  public ngOnDestroy(): void {
    this._unsubscribe$.next();
    this._unsubscribe$.complete();
  }

}
