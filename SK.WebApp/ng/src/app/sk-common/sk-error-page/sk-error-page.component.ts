import { Component, OnInit, ErrorHandler } from '@angular/core';
//import { ErrorHandler } from '../../_services/sk-error-handler.service';

@Component({
  selector: 'sk-error-page',
  templateUrl: './sk-error-page.component.html',
  styleUrls: ['./sk-error-page.component.scss']
})
export class SkErrorPageComponent implements OnInit {

  public err: string = null;

  public constructor(private _errorHandler: ErrorHandler) {

  }

  public ngOnInit(): void {
    var error = (this._errorHandler as any).getErrorFromStore();
    this.err = error ? error.message : "No Error";
  }

}
