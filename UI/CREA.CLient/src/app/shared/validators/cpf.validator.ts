import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';
import { isValidCpfDigits, stripCpf } from '../utils/cpf';

/**
 * Valida CPF brasileiro.
 * @param optional — se true, campo vazio é válido; se false, CPF completo e válido é obrigatório.
 */
export function cpfDigitsValidator(optional: boolean): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const digits = stripCpf(String(control.value ?? ''));
    if (digits.length === 0) return optional ? null : { cpfRequired: true };
    if (digits.length < 11) return { cpfIncomplete: true };
    if (!isValidCpfDigits(digits)) return { cpfInvalid: true };
    return null;
  };
}
