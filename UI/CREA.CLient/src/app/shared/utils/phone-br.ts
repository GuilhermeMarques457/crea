/** Remove tudo que não for dígito (máx. 11 = DDD + celular). */
export function stripPhoneBr(value: string | null | undefined): string {
  return String(value ?? '')
    .replace(/\D/g, '')
    .slice(0, 11);
}

/**
 * Formata como (XX) XXXX-XXXX (10 dígitos) ou (XX) XXXXX-XXXX (11 dígitos).
 * Até 6 dígitos: (DD) parcial; 7–10: fixo; 11: celular.
 */
export function formatPhoneBrMask(value: string | null | undefined): string {
  const d = stripPhoneBr(value);
  if (!d) return '';

  const ddd = d.slice(0, 2);
  if (d.length <= 2) return `(${ddd}`;

  const loc = d.slice(2);
  if (d.length <= 6) return `(${ddd}) ${loc}`;

  if (d.length <= 10) {
    const a = loc.slice(0, 4);
    const b = loc.slice(4);
    return b ? `(${ddd}) ${a}-${b}` : `(${ddd}) ${a}`;
  }

  const a = loc.slice(0, 5);
  const b = loc.slice(5);
  return `(${ddd}) ${a}-${b}`;
}

/** Telefone BR: 10 dígitos (fixo) ou 11 (celular com 9 após DDD). DDD entre 11 e 99. */
export function isValidPhoneBrDigits(digits: string): boolean {
  const d = stripPhoneBr(digits);
  if (d.length !== 10 && d.length !== 11) return false;

  const ddd = parseInt(d.slice(0, 2), 10);
  if (ddd < 11 || ddd > 99) return false;

  if (d.length === 11) return d[2] === '9';
  return d[2] !== '9';
}
