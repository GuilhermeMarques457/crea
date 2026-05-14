/** Remove tudo que não for dígito. */
export function stripCpf(value: string | null | undefined): string {
  return String(value ?? '').replace(/\D/g, '');
}

/** Formata até 11 dígitos como 000.000.000-00 */
export function formatCpfMask(value: string | null | undefined): string {
  const d = stripCpf(value).slice(0, 11);
  if (!d) return '';
  if (d.length <= 3) return d;
  if (d.length <= 6) return `${d.slice(0, 3)}.${d.slice(3)}`;
  if (d.length <= 9) return `${d.slice(0, 3)}.${d.slice(3, 6)}.${d.slice(6)}`;
  return `${d.slice(0, 3)}.${d.slice(3, 6)}.${d.slice(6, 9)}-${d.slice(9)}`;
}

/** CPF com 11 dígitos e dígitos verificadores válidos (sequências repetidas inválidas). */
export function isValidCpfDigits(cpf: string): boolean {
  const c = stripCpf(cpf);
  if (c.length !== 11) return false;
  if (/^(\d)\1{10}$/.test(c)) return false;

  let sum = 0;
  for (let i = 0; i < 9; i++) sum += parseInt(c[i]!, 10) * (10 - i);
  let d1 = 11 - (sum % 11);
  if (d1 >= 10) d1 = 0;
  if (d1 !== parseInt(c[9]!, 10)) return false;

  sum = 0;
  for (let i = 0; i < 10; i++) sum += parseInt(c[i]!, 10) * (11 - i);
  let d2 = 11 - (sum % 11);
  if (d2 >= 10) d2 = 0;
  return d2 === parseInt(c[10]!, 10);
}
