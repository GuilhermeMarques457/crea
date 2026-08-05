import { signal } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, convertToParamMap, Router } from '@angular/router';
import { Observable, of, throwError } from 'rxjs';
import { afterEach, describe, expect, it, vi } from 'vitest';

import { AnexoService } from '../../../core/services/anexo.service';
import { AssinaturaService } from '../../../core/services/assinatura.service';
import { AuthService } from '../../../core/services/auth.service';
import { NotificacaoService } from '../../../core/services/notificacao.service';
import { ObraService } from '../../../core/services/obra.service';
import { RelatoVisitaService } from '../../../core/services/registro-diario.service';
import { TermoConclusaoService } from '../../../core/services/termo-conclusao.service';
import { ToastService } from '../../../core/services/toast.service';
import {
  AnexoDto,
  AssinaturaDto,
  LoginResponseDto,
  ObraDto,
  PosicaoObra,
  RelatoVisitaDto,
  StatusObra,
  TermoConclusaoDto,
  TipoAssinante,
  TipoEdificacao,
  TipoEntidadeAssinatura,
  TipoUsuario,
} from '../../../shared/models/api.models';
import { EntidadeAnexosDialogComponent } from '../../../shared/components/entidade-anexos-dialog/entidade-anexos-dialog.component';
import { SignaturePadDialogComponent } from '../../../shared/components/signature-pad-dialog/signature-pad-dialog.component';
import { SignatureViewDialogComponent } from '../../../shared/components/signature-view-dialog/signature-view-dialog.component';
import { ObraDetailComponent } from './obra-detail.component';

type DialogRefStub<T> = {
  afterClosed: () => Observable<T>;
};

interface SetupOptions {
  id?: string;
  queryParams?: Record<string, string>;
  user?: LoginResponseDto | null;
  obra?: ObraDto;
  obraError?: boolean;
  registros?: RelatoVisitaDto[];
  anexos?: AnexoDto[];
  termo?: TermoConclusaoDto | null;
  assinaturas?: AssinaturaDto[];
  statusError?: boolean;
  uploadResult?: AnexoDto;
  uploadError?: boolean;
  dialogResult?: File | null;
  signatureResult?: AssinaturaDto;
}

function makeUser(overrides: Partial<LoginResponseDto> = {}): LoginResponseDto {
  return {
    token: 'token',
    expiracao: '2026-12-31T23:59:59.000Z',
    usuarioId: 'user-1',
    nome: 'Ada',
    email: 'ada@example.com',
    tipoUsuario: TipoUsuario.ResponsavelTecnico,
    ...overrides,
  };
}

function makeObra(overrides: Partial<ObraDto> = {}): ObraDto {
  return {
    id: 'obra-1',
    localObra: 'Rua Central, 123',
    proprietarioId: 'prop-1',
    nomeProprietario: 'Maria Souza',
    telefoneProprietario: '(41) 99999-0000',
    numeroArt: 'ART-123',
    tipoEdificacao: TipoEdificacao.Residencial,
    direcaoTecnica: true,
    status: StatusObra.EmAndamento,
    dataInicio: '2026-01-10T12:00:00.000Z',
    profissionalId: 'prof-1',
    nomeProfissional: 'Eng. Ada',
    usuarioCriadorId: 'user-1',
    ativo: true,
    criadoEm: '2026-01-01T12:00:00.000Z',
    ...overrides,
  };
}

function makeAssinatura(overrides: Partial<AssinaturaDto> = {}): AssinaturaDto {
  return {
    id: 'assinatura-1',
    tipoEntidade: TipoEntidadeAssinatura.Obra,
    entidadeId: 'obra-1',
    tipoAssinante: TipoAssinante.Profissional,
    usuarioId: 'user-1',
    nomeUsuario: 'Ada',
    hashAssinatura: 'hash',
    dataAssinatura: '2026-02-10T12:00:00.000Z',
    urlImagemAssinatura: 'https://example.com/signature.png',
    imagemAssinatura: 'base64',
    ipAssinante: '127.0.0.1',
    userAgent: 'vitest',
    ...overrides,
  };
}

function makeRegistro(overrides: Partial<RelatoVisitaDto> = {}): RelatoVisitaDto {
  return {
    id: 'registro-1',
    obraId: 'obra-1',
    numeroSequencial: 7,
    data: '2026-03-10T12:00:00.000Z',
    atividades: 'Concretagem da laje',
    equipePresente: 'Equipe A',
    servicosPreliminar: false,
    fundacao: true,
    alvenarias: false,
    superestrutura: true,
    cobertura: false,
    esquadriasInstalacoesEletricasHidraulicas: false,
    revestimentoForroParePiso: false,
    pintura: false,
    servicosComplementares: false,
    posicaoObra: PosicaoObra.EmAndamento,
    usuarioId: 'user-1',
    nomeUsuario: 'Ada',
    totalAssinaturas: 0,
    assinadoPeloProfissional: false,
    assinadoPeloProprietario: false,
    assinaturas: [],
    quantidadeAnexos: 2,
    ativo: true,
    criadoEm: '2026-03-10T12:00:00.000Z',
    ...overrides,
  };
}

function makeAnexo(overrides: Partial<AnexoDto> = {}): AnexoDto {
  return {
    id: 'anexo-1',
    nomeArquivoOriginal: 'planta.pdf',
    nomeArquivo: 'planta.pdf',
    tipoArquivo: 'application/pdf',
    tamanhoBytes: 2048,
    obraId: 'obra-1',
    usuarioId: 'user-1',
    nomeUsuario: 'Ada',
    urlDownload: 'https://example.com/planta.pdf',
    criadoEm: '2026-04-10T12:00:00.000Z',
    ...overrides,
  };
}

function makeTermo(overrides: Partial<TermoConclusaoDto> = {}): TermoConclusaoDto {
  return {
    id: 'termo-1',
    obraId: 'obra-1',
    numeroTermo: 1,
    dataConclusao: '2026-05-10T12:00:00.000Z',
    descricao: 'Obra concluida conforme projeto.',
    profissionalId: 'prof-1',
    nomeProfissional: 'Eng. Ada',
    numeroRegistro: 'CREA-123',
    criadoEm: '2026-05-11T12:00:00.000Z',
    assinadoPeloProfissional: false,
    assinadoPeloProprietario: false,
    concluido: false,
    assinaturas: [],
    ...overrides,
  };
}

function setup(options: SetupOptions = {}) {
  vi.spyOn(console, 'log').mockImplementation(() => undefined);

  const obra = options.obra ?? makeObra({ id: options.id ?? 'obra-1' });
  const registros = options.registros ?? [makeRegistro({ obraId: obra.id })];
  const anexos = options.anexos ?? [makeAnexo({ obraId: obra.id })];
  const termo = options.termo === undefined ? makeTermo({ obraId: obra.id }) : options.termo;
  const assinaturas = options.assinaturas ?? [];
  const userSignal = signal<LoginResponseDto | null>(
    options.user === undefined ? makeUser() : options.user,
  );

  const obraService = {
    obter: vi
      .fn()
      .mockReturnValue(
        options.obraError ? throwError(() => new Error('obra not found')) : of(obra),
      ),
    atualizarStatus: vi
      .fn()
      .mockReturnValue(
        options.statusError ? throwError(() => new Error('status failed')) : of(undefined),
      ),
  };
  const registroService = {
    porObra: vi.fn().mockReturnValue(of(registros)),
  };
  const anexoService = {
    porObra: vi.fn().mockReturnValue(of(anexos)),
    upload: vi
      .fn()
      .mockReturnValue(
        options.uploadError
          ? throwError(() => new Error('upload failed'))
          : of(options.uploadResult ?? makeAnexo({ id: 'anexo-upload', obraId: obra.id })),
      ),
  };
  const termoService = {
    porObra: vi
      .fn()
      .mockReturnValue(termo ? of(termo) : throwError(() => new Error('termo not found'))),
  };
  const assinaturaService = {
    porEntidade: vi.fn().mockReturnValue(of(assinaturas)),
    assinar: vi.fn().mockReturnValue(of(options.signatureResult ?? makeAssinatura())),
  };
  const toast = {
    success: vi.fn(),
    error: vi.fn(),
  };
  const notificacaoService = {
    carregarMeusPendentes: vi.fn(),
  };
  const dialog = {
    open: vi.fn(
      (): DialogRefStub<File | null> => ({
        afterClosed: () => of(options.dialogResult ?? null),
      }),
    ),
  };
  const router = {
    navigate: vi.fn(),
  };
  const activatedRoute = {
    snapshot: {
      paramMap: convertToParamMap({ id: obra.id }),
      queryParamMap: convertToParamMap(options.queryParams ?? {}),
    },
  };

  TestBed.configureTestingModule({
    imports: [ObraDetailComponent],
    providers: [
      { provide: ActivatedRoute, useValue: activatedRoute },
      { provide: Router, useValue: router },
      { provide: ObraService, useValue: obraService },
      { provide: RelatoVisitaService, useValue: registroService },
      { provide: AnexoService, useValue: anexoService },
      { provide: TermoConclusaoService, useValue: termoService },
      { provide: AssinaturaService, useValue: assinaturaService },
      { provide: ToastService, useValue: toast },
      { provide: AuthService, useValue: { currentUser: userSignal.asReadonly() } },
      { provide: NotificacaoService, useValue: notificacaoService },
      { provide: MatDialog, useValue: dialog },
    ],
  }).overrideComponent(ObraDetailComponent, {
    set: { template: '' },
  });

  const fixture = TestBed.createComponent(ObraDetailComponent);
  const component = fixture.componentInstance;
  fixture.detectChanges();

  return {
    fixture,
    component,
    userSignal,
    obra,
    registros,
    anexos,
    termo,
    assinaturas,
    mocks: {
      obraService,
      registroService,
      anexoService,
      termoService,
      assinaturaService,
      toast,
      notificacaoService,
      dialog,
      router,
    },
  };
}

function mockClipboard(writeText = vi.fn().mockResolvedValue(undefined)) {
  Object.defineProperty(navigator, 'clipboard', {
    configurable: true,
    value: { writeText },
  });
  return writeText;
}

describe('ObraDetailComponent', () => {
  afterEach(() => {
    TestBed.resetTestingModule();
    vi.restoreAllMocks();
  });

  it('loads the obra and related resources from the route id', () => {
    const { component, obra, registros, anexos, termo, mocks } = setup();

    expect(component.loading()).toBe(false);
    expect(component.obra()).toEqual(obra);
    expect(component.registros()).toEqual(registros);
    expect(component.anexos()).toEqual(anexos);
    expect(component.termo()).toEqual(termo);
    expect(component.assinaturasObra()).toEqual([]);
    expect(mocks.obraService.obter).toHaveBeenCalledWith('obra-1');
    expect(mocks.registroService.porObra).toHaveBeenCalledWith('obra-1');
    expect(mocks.anexoService.porObra).toHaveBeenCalledWith('obra-1');
    expect(mocks.termoService.porObra).toHaveBeenCalledWith('obra-1');
    expect(mocks.assinaturaService.porEntidade).toHaveBeenCalledWith(
      TipoEntidadeAssinatura.Obra,
      'obra-1',
    );
  });

  it('navigates back to obras when the obra cannot be loaded', () => {
    const { component, mocks } = setup({ obraError: true });

    expect(component.loading()).toBe(false);
    expect(mocks.router.navigate).toHaveBeenCalledWith(['/obras']);
    expect(mocks.registroService.porObra).not.toHaveBeenCalled();
    expect(mocks.anexoService.porObra).not.toHaveBeenCalled();
  });

  it('updates the obra status and notifies success', () => {
    const { component, mocks } = setup();

    component.mudarStatus(StatusObra.Concluida);

    expect(mocks.obraService.atualizarStatus).toHaveBeenCalledWith('obra-1', StatusObra.Concluida);
    expect(component.obra()?.status).toBe(StatusObra.Concluida);
    expect(mocks.toast.success).toHaveBeenCalledWith('Status atualizado.');
  });

  it('keeps the current status and notifies error when status update fails', () => {
    const { component, mocks } = setup({ statusError: true });

    component.mudarStatus(StatusObra.Cancelada);

    expect(component.obra()?.status).toBe(StatusObra.EmAndamento);
    expect(mocks.toast.error).toHaveBeenCalledWith('Erro ao atualizar status.');
  });

  it('evaluates obra signature permission by user role and existing signatures', () => {
    const { component, userSignal } = setup();

    expect(component.podeAssinarObra()).toBe(true);

    component.assinaturasObra.set([makeAssinatura({ tipoAssinante: TipoAssinante.Profissional })]);
    expect(component.podeAssinarObra()).toBe(false);

    userSignal.set(makeUser({ tipoUsuario: TipoUsuario.UsuarioCrea }));
    expect(component.podeAssinarObra()).toBe(true);

    component.assinaturasObra.set([makeAssinatura({ tipoAssinante: TipoAssinante.UsuarioCrea })]);
    expect(component.podeAssinarObra()).toBe(false);

    userSignal.set(makeUser({ tipoUsuario: TipoUsuario.Proprietario }));
    component.assinaturasObra.set([]);
    expect(component.podeAssinarObra()).toBe(false);
  });

  it('evaluates termo and registro signature permission from the current user role', () => {
    const { component, userSignal } = setup();
    const registro = makeRegistro();

    expect(component.podeAssinarTermo()).toBe(true);
    expect(component.podeAssinarRegistro(registro)).toBe(true);

    component.termo.set(makeTermo({ assinadoPeloProfissional: true }));
    expect(component.podeAssinarTermo()).toBe(false);

    userSignal.set(makeUser({ tipoUsuario: TipoUsuario.Proprietario }));
    component.termo.set(makeTermo({ assinadoPeloProprietario: false }));
    expect(component.podeAssinarTermo()).toBe(true);
    expect(component.podeAssinarRegistro({ ...registro, assinadoPeloProprietario: true })).toBe(
      false,
    );

    component.termo.set(makeTermo({ concluido: true }));
    expect(component.podeAssinarTermo()).toBe(false);
  });

  it('uploads a selected attachment and appends it to the list', () => {
    const uploaded = makeAnexo({ id: 'anexo-2', nomeArquivoOriginal: 'memorial.pdf' });
    const { component, mocks } = setup({ uploadResult: uploaded });
    const file = new File(['pdf'], 'memorial.pdf', { type: 'application/pdf' });

    component.uploadAnexo({ target: { files: [file] } } as unknown as Event);

    expect(mocks.anexoService.upload).toHaveBeenCalledWith(file, { obraId: 'obra-1' });
    expect(component.anexos()).toContainEqual(uploaded);
    expect(mocks.toast.success).toHaveBeenCalledWith('Arquivo enviado.');
  });

  it('notifies an upload error and ignores events without a file', () => {
    const { component, mocks } = setup({ uploadError: true });
    const file = new File(['pdf'], 'erro.pdf', { type: 'application/pdf' });

    component.uploadAnexo({ target: { files: [file] } } as unknown as Event);
    expect(mocks.toast.error).toHaveBeenCalledWith('Erro ao enviar arquivo.');

    mocks.anexoService.upload.mockClear();
    component.uploadAnexo({ target: { files: [] } } as unknown as Event);
    expect(mocks.anexoService.upload).not.toHaveBeenCalled();
  });

  it('registers a signature after the pad dialog returns a file', () => {
    const file = new File(['signature'], 'signature.png', { type: 'image/png' });
    const onSuccess = vi.fn();
    const { component, mocks } = setup({ dialogResult: file });

    component.assinarEntidade(TipoEntidadeAssinatura.Obra, 'obra-1', onSuccess);

    expect(mocks.dialog.open).toHaveBeenCalledWith(SignaturePadDialogComponent, {
      width: '560px',
      disableClose: true,
    });
    expect(mocks.assinaturaService.assinar).toHaveBeenCalledWith(
      expect.objectContaining({
        tipoEntidade: TipoEntidadeAssinatura.Obra,
        entidadeId: 'obra-1',
        imagemAssinatura: file,
      }),
    );
    expect(mocks.toast.success).toHaveBeenCalledWith('Assinatura registrada com sucesso!');
    expect(mocks.notificacaoService.carregarMeusPendentes).toHaveBeenCalled();
    expect(onSuccess).toHaveBeenCalled();
  });

  it('does not register a signature when the pad dialog is cancelled', () => {
    const { component, mocks } = setup({ dialogResult: null });

    component.assinarEntidade(TipoEntidadeAssinatura.Obra, 'obra-1');

    expect(mocks.assinaturaService.assinar).not.toHaveBeenCalled();
  });

  it('refreshes the obra signatures after signing the obra', () => {
    const file = new File(['signature'], 'signature.png', { type: 'image/png' });
    const signature = makeAssinatura({ id: 'assinatura-refresh' });
    const { component, mocks } = setup({ dialogResult: file });
    mocks.assinaturaService.porEntidade.mockReturnValue(of([signature]));

    component.assinarObra();

    expect(component.assinaturasObra()).toEqual([signature]);
  });

  it('auto-opens a registro signature request from query parameters when the user can sign it', () => {
    const file = new File(['signature'], 'signature.png', { type: 'image/png' });
    const { mocks } = setup({
      queryParams: { assinar: 'registro', r: 'registro-1' },
      dialogResult: file,
    });

    expect(mocks.assinaturaService.assinar).toHaveBeenCalledWith(
      expect.objectContaining({
        tipoEntidade: TipoEntidadeAssinatura.RelatoVisita,
        entidadeId: 'registro-1',
        imagemAssinatura: file,
      }),
    );
  });

  it('opens the signature viewer dialog with signer details', () => {
    const { component, mocks } = setup();
    const signature = makeAssinatura({ tipoAssinante: TipoAssinante.UsuarioCrea });

    component.verAssinatura(signature);

    expect(mocks.dialog.open).toHaveBeenCalledWith(SignatureViewDialogComponent, {
      width: '480px',
      data: {
        nomeUsuario: 'Ada',
        tipoAssinante: 'CREA',
        dataAssinatura: '2026-02-10T12:00:00.000Z',
        imagemAssinatura: 'https://example.com/signature.png',
      },
    });
  });

  it('copies assinatura links and reports clipboard success or failure', async () => {
    const { component, mocks } = setup();
    const writeText = mockClipboard();

    component.copiarLink('registro', 'registro-1');
    await Promise.resolve();

    expect(writeText).toHaveBeenCalledWith(
      `${window.location.origin}/obras/obra-1?assinar=registro&r=registro-1`,
    );
    expect(mocks.toast.success).toHaveBeenCalledWith('Link de assinatura copiado!');

    const failingWriteText = mockClipboard(vi.fn().mockRejectedValue(new Error('denied')));
    component.copiarLink('obra');
    await Promise.resolve();

    expect(failingWriteText).toHaveBeenCalledWith(
      `${window.location.origin}/obras/obra-1?assinar=obra`,
    );
    expect(mocks.toast.error).toHaveBeenCalledWith('N\u00e3o foi poss\u00edvel copiar o link.');
  });

  it('opens the registro attachments dialog with the selected registro', () => {
    const { component, mocks } = setup();
    const registro = makeRegistro({ id: 'registro-99', numeroSequencial: 99 });

    component.abrirAnexosRegistro(registro);

    expect(mocks.dialog.open).toHaveBeenCalledWith(EntidadeAnexosDialogComponent, {
      width: '520px',
      maxWidth: '95vw',
      data: expect.objectContaining({
        tipo: 'registro',
        entidadeId: 'registro-99',
        titulo: expect.stringContaining('Registro #99'),
      }),
    });
    expect(component.qtdAnexosRegistro({ ...registro, quantidadeAnexos: undefined })).toBe(0);
    expect(component.qtdAnexosRegistro(registro)).toBe(2);
  });
});
