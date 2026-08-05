export enum TipoUsuario {
  Operacional = 1,
  ResponsavelTecnico = 2,
  Admin = 3,
  UsuarioCrea = 4,
  Proprietario = 5,
}

export enum TipoEntidadeAssinatura {
  Obra = 1,
  RelatoVisita = 2,
  TermoConclusao = 3,
}

export enum TipoAssinante {
  Profissional = 1,
  UsuarioCrea = 2,
  Proprietario = 3,
}

export enum StatusObra {
  EmAndamento = 1,
  Concluida = 2,
  Pausada = 3,
  Cancelada = 4,
}
export enum TipoEdificacao {
  Residencial = 1,
  Comercial = 2,
  Industrial = 3,
}
export enum AtividadeTecnica {
  Execucao = 1,
  Fiscalizacao = 2,
  Projeto = 3,
}
export enum PosicaoObra {
  DeAcordoComProjeto = 1,
  EmDesacordoComProjeto = 2,
  EmAndamento = 3,
  Paralisada = 4,
}

export const TIPO_EDIFICACAO_LABELS: Record<TipoEdificacao, string> = {
  [TipoEdificacao.Residencial]: 'Residencial',
  [TipoEdificacao.Comercial]: 'Comercial',
  [TipoEdificacao.Industrial]: 'Industrial',
};
export const ATIVIDADE_TECNICA_LABELS: Record<AtividadeTecnica, string> = {
  [AtividadeTecnica.Execucao]: 'Execução',
  [AtividadeTecnica.Fiscalizacao]: 'Fiscalização',
  [AtividadeTecnica.Projeto]: 'Projeto',
};
export const POSICAO_OBRA_LABELS: Record<PosicaoObra, string> = {
  [PosicaoObra.DeAcordoComProjeto]: 'De acordo com o projeto',
  [PosicaoObra.EmDesacordoComProjeto]: 'Em desacordo com o projeto',
  [PosicaoObra.EmAndamento]: 'Em andamento',
  [PosicaoObra.Paralisada]: 'Paralisada',
};

export const STATUS_OBRA_LABELS: Record<StatusObra, string> = {
  [StatusObra.EmAndamento]: 'Em Andamento',
  [StatusObra.Pausada]: 'Pausada',
  [StatusObra.Concluida]: 'Concluída',
  [StatusObra.Cancelada]: 'Cancelada',
};

export const TIPO_USUARIO_LABELS: Record<TipoUsuario, string> = {
  [TipoUsuario.Admin]: 'Administrador',
  [TipoUsuario.ResponsavelTecnico]: 'Responsável Técnico',
  [TipoUsuario.Operacional]: 'Operacional',
  [TipoUsuario.UsuarioCrea]: 'Usuário CREA',
  [TipoUsuario.Proprietario]: 'Proprietário',
};

// Auth
export interface LoginDto {
  email: string;
  senha: string;
}
export interface LoginResponseDto {
  token: string;
  expiracao: string;
  usuarioId: string;
  nome: string;
  email: string;
  tipoUsuario: TipoUsuario;
}
export interface CreateUsuarioDto {
  nome: string;
  email: string;
  senha: string;
  tipoUsuario: TipoUsuario;
}
export interface UpdateUsuarioDto {
  nome: string;
  email: string;
  tipoUsuario: TipoUsuario;
  ativo: boolean;
}
export interface UsuarioDto {
  id: string;
  nome: string;
  email: string;
  tipoUsuario: TipoUsuario;
  ativo: boolean;
  criadoEm: string;
}

// Obras
export interface ObraDto {
  id: string;
  localObra: string;
  proprietarioId: string;
  nomeProprietario: string;
  telefoneProprietario?: string;
  empresa?: string;
  numeroCaderneta?: string;
  numeroArt: string;
  numeroRT?: string;
  tipoEdificacao?: TipoEdificacao;
  atividadeTecnica?: AtividadeTecnica;
  direcaoTecnica: boolean;
  status: StatusObra;
  dataInicio: string;
  areaConstruir?: number;
  areaRegularizar?: number;
  areaAmpliar?: number;
  areaReformar?: number;
  areaTotalEdificada?: number;
  valorRecibo?: number;
  profissionalId: string;
  nomeProfissional: string;
  usuarioCriadorId: string;
  ativo: boolean;
  criadoEm: string;
}
export interface CreateObraDto {
  localObra?: string;
  proprietarioId: string;
  empresa?: string;
  numeroCaderneta?: string;
  numeroArt: string;
  numeroRT?: string;
  tipoEdificacao?: TipoEdificacao;
  atividadeTecnica?: AtividadeTecnica;
  direcaoTecnica: boolean;
  dataInicio: string;
  areaConstruir?: number;
  areaRegularizar?: number;
  areaAmpliar?: number;
  areaReformar?: number;
  areaTotalEdificada?: number;
  valorRecibo?: number;
  profissionalId: string;
}

// Profissionais
export interface ProfissionalDto {
  id: string;
  nome: string;
  cpf: string;
  numeroRegistro: string;
  tipoRegistro: string;
  empresa?: string;
  especialidade: string;
  email: string;
  telefone?: string;
  usuarioId?: string;
  ativo: boolean;
  criadoEm: string;
}
export interface CreateProfissionalDto {
  nome: string;
  cpf: string;
  numeroRegistro: string;
  tipoRegistro: string;
  empresa?: string;
  especialidade: string;
  email: string;
  telefone?: string;
  usuarioId?: string;
}

// Proprietários
export interface ProprietarioDto {
  id: string;
  nome: string;
  cpf: string;
  email: string;
  telefone: string;
  usuarioId?: string;
  ativo: boolean;
  criadoEm: string;
}
export interface CreateProprietarioDto {
  nome: string;
  cpf?: string;
  email?: string;
  telefone?: string;
  usuarioId?: string;
}

// Relato de Visita
export interface RelatoVisitaDto {
  id: string;
  obraId: string;
  numeroSequencial: number;
  data: string;
  atividades: string;
  equipePresente: string;
  condicaoClimatica?: string;
  observacoes?: string;
  servicosPreliminar: boolean;
  fundacao: boolean;
  alvenarias: boolean;
  superestrutura: boolean;
  cobertura: boolean;
  esquadriasInstalacoesEletricasHidraulicas: boolean;
  revestimentoForroParePiso: boolean;
  pintura: boolean;
  servicosComplementares: boolean;
  posicaoObra?: PosicaoObra;
  decisoesTecnicas?: string;
  usuarioId: string;
  nomeUsuario: string;
  totalAssinaturas: number;
  assinadoPeloProfissional: boolean;
  assinadoPeloProprietario: boolean;
  assinaturas: AssinaturaDto[];
  quantidadeAnexos?: number;
  ativo: boolean;
  criadoEm: string;
}
export interface CreateRelatoVisitaDto {
  obraId: string;
  data: string;
  atividades: string;
  equipePresente: string;
  condicaoClimatica?: string;
  observacoes?: string;
  servicosPreliminar: boolean;
  fundacao: boolean;
  alvenarias: boolean;
  superestrutura: boolean;
  cobertura: boolean;
  esquadriasInstalacoesEletricasHidraulicas: boolean;
  revestimentoForroParePiso: boolean;
  pintura: boolean;
  servicosComplementares: boolean;
  posicaoObra?: PosicaoObra;
  decisoesTecnicas?: string;
}

// Assinaturas
export interface AssinaturaDto {
  id: string;
  tipoEntidade: TipoEntidadeAssinatura;
  entidadeId: string;
  tipoAssinante: TipoAssinante;
  usuarioId: string;
  nomeUsuario: string;
  hashAssinatura: string;
  dataAssinatura: string;
  urlImagemAssinatura: string;
  imagemAssinatura: string;
  ipAssinante: string;
  userAgent: string;
  navegador?: string;
  sistemaOperacional?: string;
  dispositivo?: string;
}

export interface CreateAssinaturaDto {
  tipoEntidade: TipoEntidadeAssinatura;
  entidadeId: string;
  imagemAssinatura: File;
  navegador?: string;
  sistemaOperacional?: string;
  dispositivo?: string;
}

export interface PendenteAssinaturaDto {
  tipoEntidade: TipoEntidadeAssinatura;
  entidadeId: string;
  obraId: string;
  tipoAssinante: TipoAssinante;
  titulo: string;
  subtitulo?: string;
  criadoEm: string;
}

export interface MinhaAssinaturaDto {
  tipoEntidade: TipoEntidadeAssinatura;
  entidadeId: string;
  obraId: string;
  tipoAssinante: TipoAssinante;
  titulo: string;
  subtitulo?: string;
  dataAssinatura: string;
  totalmenteAssinado: boolean;
}

// Anexos
export interface AnexoDto {
  id: string;
  nomeArquivoOriginal: string;
  nomeArquivo: string;
  tipoArquivo: string;
  tamanhoBytes: number;
  obraId?: string;
  registroDiarioId?: string;
  usuarioId: string;
  nomeUsuario: string;
  urlDownload: string;
  criadoEm: string;
}

// Termos de Conclusão
export interface TermoConclusaoDto {
  id: string;
  obraId: string;
  numeroTermo: number;
  dataConclusao: string;
  descricao: string;
  observacoes?: string;
  declaracaoTexto?: string;
  localDeclaracao?: string;
  dataDeclaracao?: string;
  profissionalId: string;
  nomeProfissional: string;
  numeroRegistro: string;
  criadoEm: string;
  assinadoPeloProfissional: boolean;
  assinadoPeloProprietario: boolean;
  concluido: boolean;
  assinaturas: AssinaturaDto[];
}
export interface CreateTermoConclusaoDto {
  obraId: string;
  numeroTermo?: number;
  dataConclusao: string;
  descricao: string;
  observacoes?: string;
  declaracaoTexto?: string;
  localDeclaracao?: string;
  dataDeclaracao?: string;
}

// Relatório
export interface RelatorioObraDto {
  obraId: string;
  localObra: string;
  proprietario: string;
  telefoneProprietario?: string;
  empresa?: string;
  numeroCaderneta?: string;
  numeroArt: string;
  numeroRT?: string;
  tipoEdificacao?: TipoEdificacao;
  atividadeTecnica?: AtividadeTecnica;
  direcaoTecnica: boolean;
  status: StatusObra;
  dataInicio: string;
  nomeProfissional: string;
  numeroRegistroProfissional: string;
  totalRelatoVisita: number;
  totalAnexos: number;
  possuiTermoConclusao: boolean;
  dataConclusao?: string;
  assinadoPeloProfissional: boolean;
  assinadoPeloProprietario: boolean;
  assinadoPeloCrea: boolean;
  termoConcluido: boolean;
  termoNumero?: number;
  termoDescricao?: string;
  termoObservacoes?: string;
  termoDeclaracaoTexto?: string;
  assinaturasObra: AssinaturaDto[];
  assinaturasTermo: AssinaturaDto[];
  relatoVisita: RelatoVisitaDto[];
  geradoEm: string;
}

// Paginação
export interface PagedResult<T> {
  items: T[];
  totalItems: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

// Auditoria
export interface LogAuditoriaDto {
  id: string;
  usuarioId?: string;
  nomeUsuario: string;
  acao: string;
  entidade: string;
  entidadeId?: string;
  dadosAntigos?: string;
  dadosNovos?: string;
  enderecoIp?: string;
  dataAcao: string;
}
