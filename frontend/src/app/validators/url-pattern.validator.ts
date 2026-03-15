import { Directive } from '@angular/core';
import { AbstractControl, NG_VALIDATORS, ValidationErrors, Validator } from '@angular/forms';

@Directive({
  selector: '[appUrlPattern]',
  standalone: false,
  providers: [
    {
      provide: NG_VALIDATORS,
      useExisting: UrlPatternValidatorDirective,
      multi: true
    }
  ]
})
export class UrlPatternValidatorDirective implements Validator {

  validate(control: AbstractControl): ValidationErrors | null {

    if (!control.value) return null;

    const pattern = /^https?:\/\/.+$/;

    const valid = pattern.test(control.value);

    return valid ? null : { invalidUrl: true };
  }

}