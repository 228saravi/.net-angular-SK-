import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Security } from '../_services/sk-security.service';

import { Observable, from, of } from 'rxjs';
import { map, tap, take } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  private _router: Router;
  private _security: Security;

  public constructor(router: Router, security: Security) {
    this._router = router;
    this._security = security;
  }

  public canActivate(next: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {

    var res = from(this._security.enshureSignedIn()).pipe(take(1));
    return res;

    //return this._security.currentUserData$.pipe(
    //  map(userData => !!userData),
    //  tap(authenticated => {
    //    if (!authenticated) {
    //      this._router.navigate(['/login'], { queryParams: { backUrl: next['_routerState'].url /*Не нашёл лучшего способа получить URL*/ } });
    //    }
    //  })
    //);
  }
}
