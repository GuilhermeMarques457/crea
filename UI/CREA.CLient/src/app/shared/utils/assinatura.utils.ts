import {
  AssinaturaDto,
  TipoAssinante,
  TipoEntidadeAssinatura,
  TipoUsuario,
} from '../models/api.models';

export const TIPO_ASSINANTE_LABELS: Record<TipoAssinante, string> = {
  [TipoAssinante.Profissional]: 'Profissional',
  [TipoAssinante.UsuarioCrea]: 'CREA',
  [TipoAssinante.Proprietario]: 'Proprietário',
};

export const TIPO_ENTIDADE_LABELS: Record<TipoEntidadeAssinatura, string> = {
  [TipoEntidadeAssinatura.Obra]: 'Obra',
  [TipoEntidadeAssinatura.RelatoVisita]: 'Relato de visita',
  [TipoEntidadeAssinatura.TermoConclusao]: 'Termo de conclusão',
};

export function labelTipoAssinante(tipo: TipoAssinante | string | number): string {
  const n = Number(tipo);
  return TIPO_ASSINANTE_LABELS[n as TipoAssinante] ?? String(tipo);
}

export function tipoAssinanteDoUsuario(tipoUsuario: TipoUsuario): TipoAssinante | null {
  switch (tipoUsuario) {
    case TipoUsuario.ResponsavelTecnico:
      return TipoAssinante.Profissional;
    case TipoUsuario.UsuarioCrea:
      return TipoAssinante.UsuarioCrea;
    case TipoUsuario.Proprietario:
      return TipoAssinante.Proprietario;
    default:
      return null;
  }
}

export function usuarioPodeAssinarEntidade(
  tipoUsuario: TipoUsuario,
  tipoEntidade: TipoEntidadeAssinatura,
): boolean {
  switch (tipoUsuario) {
    case TipoUsuario.ResponsavelTecnico:
      return (
        tipoEntidade === TipoEntidadeAssinatura.Obra ||
        tipoEntidade === TipoEntidadeAssinatura.RelatoVisita ||
        tipoEntidade === TipoEntidadeAssinatura.TermoConclusao
      );
    case TipoUsuario.UsuarioCrea:
      return tipoEntidade === TipoEntidadeAssinatura.Obra;
    case TipoUsuario.Proprietario:
      return (
        tipoEntidade === TipoEntidadeAssinatura.RelatoVisita ||
        tipoEntidade === TipoEntidadeAssinatura.TermoConclusao
      );
    default:
      return false;
  }
}

export function possuiAssinatura(
  assinaturas: AssinaturaDto[],
  tipo: TipoAssinante,
): boolean {
  return assinaturas.some((a) => a.tipoAssinante === tipo);
}
