import { Pipe, PipeTransform } from '@angular/core';
import * as moment from 'moment';

export type Format = "Normal" | "Short";

@Pipe({
  name: 'date'
})
export class SkDatePipe implements PipeTransform {

  transform(value: any, format: Format = "Normal"): any {
    try {
      if (!value) {
        return "";
      }

      if (typeof value == "string") {
        value = new Date(value);
      }

      var m = moment(value);

      if (format == "Short") {
        return m.locale("ru").format("L");
      } else {
        return m.locale("ru").format("LL");
      }

      
    } catch {
      return "";
    }
  }

}
