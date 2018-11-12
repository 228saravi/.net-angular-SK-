import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { DxPopupComponent } from 'devextreme-angular';
import { Security, RegisterExpertReq, UserNotFoundError, WrongEmailOrPasswordError, EmailAlreadyUsedError, RegisterCompanyReq } from '../../_services/sk-security.service';
import { AbstractControl } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';

enum Mode {
  signIn,
  registerExpert,
  registerCompany,
  initPasswordReset,
  finishPasswordReset,
}

class SignInVm {
  public email: string = null;
  public password: string = null;
}

class RegisterExpertVm {
  public email: string = null;
  public password: string = null;
  public name: string = null;
  public confirmed: boolean = false;
}

class RegisterCompanyVm {
  public email: string = null;
  public password: string = null;
  public name: string = null;
  public companyName: string = null;
  public confirmed: boolean = false;
}

class InitResetPasswordVm {
  public email: string = null;
}

class FinishResetPasswordVm {
  public email: string = null;
  public token: string = null;
  public newPassword: string = null;
}

class Vm {
  public mode: Mode = Mode.signIn;
  public signInVm: SignInVm = new SignInVm();
  public initPasswordResetVm: InitResetPasswordVm = new InitResetPasswordVm();
  public finishPasswordResetVm: FinishResetPasswordVm = new FinishResetPasswordVm();
  public registerExpertVm: RegisterExpertVm = new RegisterExpertVm();
  public registerCompanyVm: RegisterCompanyVm = new RegisterCompanyVm();
}

@Component({
  selector: 'sk-sign-in-popup',
  templateUrl: './sk-sign-in-popup.component.html',
  styleUrls: ['./sk-sign-in-popup.component.scss']
})
export class SkSignInPopupComponent implements OnInit, OnDestroy {
  public Mode = Mode;

  @ViewChild("popup") popup: DxPopupComponent;

  private _vm: Vm = null;
  private _enshureSignedInPromiseResolve: (isSignedIn: boolean) => void;

  private _disposedSubj: Subject<void> = new Subject();

  public constructor(private _activatedRoute: ActivatedRoute, private _security: Security) {
    _security.registerSignInPopup(this);
  }

  public get vm(): Vm {
    return this._vm;
  }

  public get popupHeight(): number {
    if (this._vm == null) {
      return 500;
    }

    switch (this._vm.mode) {
      case Mode.signIn:
        return 330;
      case Mode.registerExpert:
        return 385;
      case Mode.registerCompany:
        return 440;
      case Mode.initPasswordReset:
        return 210;
      case Mode.finishPasswordReset:
        return 210;
      default:
        return 400;
    }
  }

  public ngOnInit(): void {
    this._activatedRoute.queryParamMap
      .pipe(takeUntil(this._disposedSubj))
      .subscribe(params => {
        var email = params.get("email");
        var token = params.get("token");

        if (email && token) {
          this._vm = new Vm();
          this._vm.finishPasswordResetVm.email = email;
          this._vm.finishPasswordResetVm.token = token;
          this._vm.mode = Mode.finishPasswordReset;
        }
      });
  }

  public async ngOnDestroy(): Promise<void> {
    this._disposedSubj.next(null);
    this._disposedSubj.complete();
  }

  public async enshureSignedIn(): Promise<boolean> {
    this._vm = new Vm();

    var email = this._activatedRoute.snapshot.queryParamMap.get("email");
    var token = this._activatedRoute.snapshot.queryParamMap.get("token");

    if (email && token) {
      this._vm.finishPasswordResetVm.email = email;
      this._vm.finishPasswordResetVm.token = token;
      this._vm.mode = Mode.finishPasswordReset;
    }

    await (this.popup.instance.show() as Promise<void>);

    var p = new Promise<boolean>((resolve, reject) => {
      this._enshureSignedInPromiseResolve = resolve;
    });

    return p;
  }

  public async hide(): Promise<void> {
    await (this.popup.instance.hide() as Promise<void>);
  }

  public onHidding($event): void {
  }

  public onHidden(): void {
    this._vm = null;
  }

  public onCloseClick(): void {
    this.hide();

    if (this._enshureSignedInPromiseResolve) {
      this._enshureSignedInPromiseResolve(false);
    }
  }


  public async signIn(): Promise<void> {
    try {
      await this._security.signIn({
        email: this._vm.signInVm.email,
        password: this._vm.signInVm.password
      });

      await this.hide();

      if (this._enshureSignedInPromiseResolve) {
        this._enshureSignedInPromiseResolve(true);
      }

    } catch (e) {
      if (e instanceof WrongEmailOrPasswordError) {

      } else {
        await this.hide();
        throw e;
      }
    }
  }

  public async initPasswordReset(): Promise<void> {
    try {
      await this._security.initPasswordReset({ email: this._vm.initPasswordResetVm.email });
      this._vm.mode = Mode.signIn;
    } catch (e) {
      if (e instanceof UserNotFoundError) {

      } else {
        await this.hide();
        throw e;
      }

    }
  }

  public async finishPasswordReset(): Promise<void> {
    try {
      await this._security.finishPasswordReset({
        email: this._vm.finishPasswordResetVm.email,
        token: this._vm.finishPasswordResetVm.token,
        newPassword: this._vm.finishPasswordResetVm.newPassword
      });
      this._vm.mode = Mode.signIn;
    } catch (e) {
      await this.hide();
      throw e;
    }
  }

  public async registerExpert(): Promise<void> {
    try {
      await this._security.registerExpert({
        email: this._vm.registerExpertVm.email,
        password: this._vm.registerExpertVm.password,
        name: this._vm.registerExpertVm.name
      });

      //this._vm.mode = Mode.signIn;
      this._vm.signInVm.email = this._vm.registerExpertVm.email;
      this._vm.signInVm.password = this._vm.registerExpertVm.password;

      await this.signIn();
    } catch (e) {
      if (e instanceof EmailAlreadyUsedError) { }
      else {
        await this.hide();
        throw e;
      }
    }
  }

  public async registerCompany(): Promise<void> {
    try {
      await this._security.registerCompany({
        email: this._vm.registerCompanyVm.email,
        password: this._vm.registerCompanyVm.password,
        name: this._vm.registerCompanyVm.name,
        companyName: this._vm.registerCompanyVm.companyName,
      });

      //this._vm.mode = Mode.signIn;
      this._vm.signInVm.email = this._vm.registerCompanyVm.email;
      this._vm.signInVm.password = this._vm.registerCompanyVm.password;

      await this.signIn();
    } catch (e) {
      if (e instanceof EmailAlreadyUsedError) { }
      else {
        await this.hide();
        throw e;
      }
    }
  }


  public detectLoginErrorMessage(control: AbstractControl): string {
    if (control.pristine || !control.errors) {
      return null;
    }

    if (control.errors["required"]) {
      return "Обязательное поле";
    }

    if (control.errors["email"]) {
      return "Невалидный email";
    }

    return null;
  }

  public detectPasswordErrorMessage(control: AbstractControl): string {
    if (control.pristine || !control.errors) {
      return null;
    }

    if (control.errors["required"]) {
      return "Обязательное поле";
    }

    if (control.errors["minlength"]) {
      return "Недостаточная длина";
    }

    return null;
  }
}
