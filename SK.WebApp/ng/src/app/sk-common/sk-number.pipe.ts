import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'number'
})
export class SkNumberPipe implements PipeTransform {

  transform(value: number, fractionDigits: number = 0): any {
    if (value == null) {
      return "";
    }

    if (Intl) {
      return Intl.NumberFormat("en-US", { useGrouping: true, minimumIntegerDigits: 1, minimumFractionDigits: fractionDigits, maximumFractionDigits: fractionDigits }).format(value);
    } else {
      return value.toString();
    }

    
  }

}
