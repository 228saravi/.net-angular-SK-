import { Pipe, PipeTransform } from '@angular/core';
import * as moment from 'moment';

@Pipe({
  name: 'time'
})
export class SkTimePipe implements PipeTransform {

  transform(value: any, args?: any): any {
    try {
      if (!value) {
        return "";
      }

      if (typeof value == "string") {
        value = new Date(value);
      }

      var m = moment(value);
      return m.locale("ru").format("DD MMM HH:mm");
    } catch {
      return "";
    }
  }
}
