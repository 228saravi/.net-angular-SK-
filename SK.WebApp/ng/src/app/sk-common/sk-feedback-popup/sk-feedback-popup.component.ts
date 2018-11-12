import { Component, OnInit, ViewChild, Output, EventEmitter } from '@angular/core';
import { DxPopupComponent } from 'devextreme-angular';
import { AbstractControl } from '@angular/forms';
import { ConnectionsManager } from '../../_services/sk-connections-manager.service';

type Mode = "ForExpert" | "ForCompany";

class Vm {
  public mode: Mode;
  public connectionId: number;
  public rating: number = null;
  public comment: string = null;

  public constructor(mode: Mode, connectionId: number) {
    this.mode = mode;
    this.connectionId = connectionId;
  }
}

@Component({
  selector: 'sk-feedback-popup',
  templateUrl: './sk-feedback-popup.component.html',
  styleUrls: ['./sk-feedback-popup.component.scss']
})
export class SkFeedbackPopupComponent implements OnInit {
  @Output() public feedbackPosted: EventEmitter<void> = new EventEmitter<void>();
  @ViewChild("popup") public popup: DxPopupComponent;

  private _connectionsManager: ConnectionsManager;

  private _vm: Vm = null;

  public constructor(connectionsManager: ConnectionsManager) {
    this._connectionsManager = connectionsManager;
  }

  public get vm(): Vm {
    return this._vm;
  }

  public ngOnInit(): void {

  }

  public async show(mode: Mode, connectionId: number): Promise<void> {
    this._vm = new Vm(mode, connectionId);
    await(this.popup.instance.show() as Promise<void>);
  }

  public async hide(): Promise<void> {
    await (this.popup.instance.hide() as Promise<void>);
  }

  public onHidden(): void {
    this._vm = null;
  }

  public async onSubmit(): Promise<void> {
    await this.postFeedback();
    await this.hide();
  }

  public detectRatingErrorMessage(control: AbstractControl): string {
    if (control.pristine || !control.errors) {
      return null;
    }

    if (control.errors["required"]) {
      return "Обязательное поле";
    }

    return null;
  }

  public async postFeedback(): Promise<void> {
    await this._connectionsManager.postFeedback({
      connectionId: this._vm.connectionId,
      rating: this._vm.rating,
      commentHtml: this._vm.comment
    });

    this.feedbackPosted.emit(null);
  }
}
