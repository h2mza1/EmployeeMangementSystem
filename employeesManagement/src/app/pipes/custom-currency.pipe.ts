import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'customCurrency'
})
export class CustomCurrencyPipe implements PipeTransform {

  transform(value: number, currencyCode: string = 'JOD', decimals: string = '1.0-0'): string {
    if (value == null || isNaN(value)) {
      return '';
    }

    const decimalPlaces = parseInt(decimals.split('-')[1], 10);

    const formattedNumber = value.toFixed(decimalPlaces);

    return `${formattedNumber} ${currencyCode}`;
  }

}
