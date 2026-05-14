import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';
import { isValidPhoneBrDigits, stripPhoneBr } from '../utils/phone-br';

/**
 * Telefone BR (10 = fixo, 11 = celular com 9 na primeira posição local).
 * @param optional — vazio é válido quando true.
 */
export function phoneBrDigitsValidator(optional: boolean): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const digits = stripPhoneBr(String(control.value ?? ''));
    if (digits.length === 0) return optional ? null : { phoneRequired: true };
    if (digits.length < 10) return { phoneIncomplete: true };
    if (!isValidPhoneBrDigits(digits)) return { phoneInvalid: true };
    return null;
  };
}
