using CREA.Application.DTOs.Ocorrencias;
using CREA.Application.DTOs.RegistrosDiarios;
using CREA.Application.DTOs.Relatorios;
using CREA.Application.DTOs.TermosConclusao;
using CREA.Domain.Enums;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CREA.API.Services;

public static class RelatorioObraPdfComposer
{
    static RelatorioObraPdfComposer() =>
        QuestPDF.Settings.License = LicenseType.Community;

    public static byte[] Generate(RelatorioObraDto r) =>
        Document.Create(d => d.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(36);
            page.DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Black));
            page.Header().Element(c => DrawHeader(c, r));
            page.Content().Element(c => DrawBody(c, r));
            page.Footer().AlignCenter().DefaultTextStyle(x => x.FontSize(8).FontColor(Colors.Grey.Darken1)).Text(t =>
            {
                t.CurrentPageNumber();
                t.Span(" / ");
                t.TotalPages();
            });
        })).GeneratePdf();

    private static void DrawHeader(IContainer container, RelatorioObraDto r)
    {
        container.Background(Colors.Blue.Darken4).Padding(16).Column(col =>
        {
            col.Item().Text("Relatório completo da obra").FontSize(9).FontColor(Colors.Grey.Lighten3);
            col.Item().Text(r.NomeObra).Bold().FontSize(16).FontColor(Colors.White);
            col.Item().Text($"{r.Endereco} — {r.Cidade}/{r.Estado}").FontSize(9).FontColor(Colors.Grey.Lighten3);
        });
    }

    private static void DrawBody(IContainer container, RelatorioObraDto r)
    {
        container.PaddingVertical(12).Column(col =>
        {
            col.Spacing(10);
            col.Item().Text($"Gerado em: {r.GeradoEm:dd/MM/yyyy HH:mm} (UTC)").FontSize(8).FontColor(Colors.Grey.Darken1);

            col.Item().Text("Resumo").SemiBold().FontSize(12);
            col.Item().Row(row =>
            {
                row.Spacing(12);
                row.RelativeItem().Element(c => DrawMetric(c, "Registros diários", r.TotalRegistrosDiarios.ToString()));
                row.RelativeItem().Element(c => DrawMetric(c, "Ocorrências", r.TotalOcorrencias.ToString()));
                row.RelativeItem().Element(c => DrawMetric(c, "Anexos (arquivos)", r.TotalAnexos.ToString()));
                row.RelativeItem().Element(c => DrawMetric(c, "Status", StatusObraPt(r.Status)));
            });

            col.Item().Text("Dados da obra").SemiBold().FontSize(12);
            col.Item().Element(c => KeyValueBlock(c, r));

            col.Item().Text("Responsável técnico").SemiBold().FontSize(12);
            col.Item().Column(block =>
            {
                block.Spacing(4);
                block.Item().Element(c => DrawKeyRow(c,"Nome", r.NomeProfissionalResponsavel));
                block.Item().Element(c => DrawKeyRow(c,"Registro", r.NumeroRegistroProfissional));
            });

            if (r.PossuiTermoConclusao)
            {
                col.Item().Text("Termo de conclusão").SemiBold().FontSize(12);
                col.Item().Column(block =>
                {
                    block.Spacing(4);
                    block.Item().Element(c => DrawKeyRow(c,"Situação", r.TermoConcluido ? "Concluído" : "Pendente de assinatura(s)"));
                    block.Item().Element(c => DrawKeyRow(c,"Responsável assinou", r.AssinadoPeloResponsavel ? "Sim" : "Não"));
                    block.Item().Element(c => DrawKeyRow(c,"Administrador assinou", r.AssinadoPeloAdmin ? "Sim" : "Não"));
                    if (r.DataConclusao.HasValue)
                        block.Item().Element(c => DrawKeyRow(c,"Data conclusão", r.DataConclusao.Value.ToString("dd/MM/yyyy")));
                    if (r.TermoNumero.HasValue)
                        block.Item().Element(c => DrawKeyRow(c,"Nº termo", r.TermoNumero.Value.ToString()));
                    if (!string.IsNullOrWhiteSpace(r.TermoDescricao))
                        block.Item().Element(c => DrawKeyRowMultiline(c,"Descrição", r.TermoDescricao!));
                    if (!string.IsNullOrWhiteSpace(r.TermoObservacoes))
                        block.Item().Element(c => DrawKeyRowMultiline(c,"Observações (termo)", r.TermoObservacoes!));
                    if (!string.IsNullOrWhiteSpace(r.TermoLocalObra))
                        block.Item().Element(c => DrawKeyRow(c,"Local (termo)", r.TermoLocalObra));
                    if (!string.IsNullOrWhiteSpace(r.TermoDeclaracaoTexto))
                        block.Item().Element(c => DrawKeyRowMultiline(c,"Declaração", r.TermoDeclaracaoTexto!));
                    if (!string.IsNullOrWhiteSpace(r.TermoAssinaturaProprietario))
                    {
                        block.Item().Element(c => DrawKeyRow(c,"Assinatura proprietário (texto)", r.TermoAssinaturaProprietario!));
                        if (r.TermoDataAssinaturaProprietario.HasValue)
                            block.Item().Element(c => DrawKeyRow(c,"Data assinatura proprietário", r.TermoDataAssinaturaProprietario.Value.ToString("dd/MM/yyyy HH:mm")));
                    }
                });

                col.Item().Text("Assinaturas do termo").SemiBold().FontSize(11);
                if (r.Assinaturas.Count == 0)
                    col.Item().Text("Nenhuma assinatura digital registrada.").Italic().FontColor(Colors.Grey.Darken1);
                else
                {
                    foreach (var a in r.Assinaturas)
                        col.Item().Element(c => AssinaturaTermoItem(c, a));
                }
            }

            col.Item().Text("Registros diários").SemiBold().FontSize(12);
            var regs = r.RegistrosDiarios.ToList();
            if (regs.Count == 0)
                col.Item().Text("Nenhum registro diário.").Italic().FontColor(Colors.Grey.Darken1);
            else
            {
                foreach (var reg in regs.OrderBy(x => x.NumeroSequencial))
                    col.Item().Element(c => RegistroDiarioBlock(c, reg));
            }

            col.Item().Text("Ocorrências").SemiBold().FontSize(12);
            var ocs = r.Ocorrencias.ToList();
            if (ocs.Count == 0)
                col.Item().Text("Nenhuma ocorrência.").Italic().FontColor(Colors.Grey.Darken1);
            else
            {
                foreach (var oc in ocs.OrderByDescending(x => x.DataOcorrencia))
                    col.Item().Element(c => OcorrenciaBlock(c, oc));
            }
        });
    }

    private static void DrawMetric(IContainer cell, string label, string value) =>
        cell.Background(Colors.Grey.Lighten4).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
            .Column(c =>
            {
                c.Item().Text(value).Bold().FontSize(14);
                c.Item().Text(label).FontSize(8).FontColor(Colors.Grey.Darken1);
            });

    private static void KeyValueBlock(IContainer container, RelatorioObraDto r)
    {
        container.Column(block =>
        {
            block.Spacing(4);
            block.Item().Element(c => DrawKeyRow(c,"Proprietário", r.Proprietario));
            if (!string.IsNullOrWhiteSpace(r.TelefoneProprietario))
                block.Item().Element(c => DrawKeyRow(c,"Tel. proprietário", r.TelefoneProprietario!));
            if (!string.IsNullOrWhiteSpace(r.Empresa))
                block.Item().Element(c => DrawKeyRow(c,"Empresa", r.Empresa!));
            if (!string.IsNullOrWhiteSpace(r.NumeroCaderneta))
                block.Item().Element(c => DrawKeyRow(c,"Nº caderneta", r.NumeroCaderneta!));
            block.Item().Element(c => DrawKeyRow(c,"ART", r.NumeroArt));
            if (!string.IsNullOrWhiteSpace(r.NumeroRT))
                block.Item().Element(c => DrawKeyRow(c,"Nº R.T.", r.NumeroRT!));
            block.Item().Element(c => DrawKeyRow(c,"Tipo obra", TipoObraPt(r.TipoObra)));
            block.Item().Element(c => DrawKeyRow(c,"Tipo edificação", r.TipoEdificacao.HasValue ? TipoEdificacaoPt(r.TipoEdificacao.Value) : "—"));
            block.Item().Element(c => DrawKeyRow(c,"Atividade técnica", r.AtividadeTecnica.HasValue ? AtividadeTecnicaPt(r.AtividadeTecnica.Value) : "—"));
            block.Item().Element(c => DrawKeyRow(c,"Direção técnica", r.DirecaoTecnica ? "Sim" : "Não"));
            block.Item().Element(c => DrawKeyRow(c,"Início", r.DataInicio.ToString("dd/MM/yyyy")));
            if (r.DataPrevisaoTermino.HasValue)
                block.Item().Element(c => DrawKeyRow(c,"Previsão término", r.DataPrevisaoTermino.Value.ToString("dd/MM/yyyy")));
        });
    }

    private static void AssinaturaTermoItem(IContainer container, AssinaturaTermoConclusaoDto a)
    {
        container.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(col =>
        {
            col.Spacing(6);
            col.Item().Text($"{a.NomeUsuario} ({a.TipoAssinante})").SemiBold();
            col.Item().Text($"Assinado em: {a.DataAssinatura:dd/MM/yyyy HH:mm}").FontSize(9).FontColor(Colors.Grey.Darken1);
            if (!string.IsNullOrWhiteSpace(a.IpAssinante))
                col.Item().Text($"IP: {a.IpAssinante}").FontSize(8).FontColor(Colors.Grey.Darken1);
            var img = DecodeImage(a.ImagemAssinatura);
            if (img is { Length: > 0 })
                col.Item().Width(260).Image(img).FitArea();
        });
    }

    private static void RegistroDiarioBlock(IContainer container, RegistroDiarioDto reg)
    {
        container.Border(1).BorderColor(Colors.Blue.Lighten4).Padding(10).Column(col =>
        {
            col.Spacing(5);
            col.Item().Row(row =>
            {
                row.RelativeItem().Text($"#{reg.NumeroSequencial} — {reg.Data:dd/MM/yyyy}").SemiBold();
                row.AutoItem().Text($"{reg.TotalAssinaturas} assinatura(s)").FontSize(9).FontColor(Colors.Blue.Darken1);
            });
            col.Item().Element(c => DrawKeyRowMultiline(c,"Atividades", reg.Atividades));
            col.Item().Element(c => DrawKeyRow(c,"Equipe", reg.EquipePresente));
            if (!string.IsNullOrWhiteSpace(reg.CondicaoClimatica))
                col.Item().Element(c => DrawKeyRow(c,"Clima", reg.CondicaoClimatica!));
            if (!string.IsNullOrWhiteSpace(reg.Observacoes))
                col.Item().Element(c => DrawKeyRowMultiline(c,"Observações", reg.Observacoes!));
            if (reg.PosicaoObra.HasValue)
                col.Item().Element(c => DrawKeyRow(c,"Posição da obra", PosicaoObraPt(reg.PosicaoObra.Value)));
            if (!string.IsNullOrWhiteSpace(reg.DecisoesTecnicas))
                col.Item().Element(c => DrawKeyRowMultiline(c,"Decisões técnicas", reg.DecisoesTecnicas!));

            var etapas = EtapasMarcadas(reg);
            if (etapas.Count > 0)
                col.Item().Text("Etapas marcadas: " + string.Join(", ", etapas)).FontSize(9);

            if (!string.IsNullOrWhiteSpace(reg.AssinaturaProprietario))
            {
                col.Item().Text("Assinatura do proprietário (texto)").SemiBold().FontSize(9);
                col.Item().Text(reg.AssinaturaProprietario!);
                if (reg.DataAssinaturaProprietario.HasValue)
                    col.Item().Text($"Data: {reg.DataAssinaturaProprietario:dd/MM/yyyy HH:mm}").FontSize(8).FontColor(Colors.Grey.Darken1);
            }

            if (reg.DataAssinaturaResponsavel.HasValue)
                col.Item().Element(c => DrawKeyRow(c,"Assinatura R.T.", reg.DataAssinaturaResponsavel.Value.ToString("dd/MM/yyyy HH:mm")));

            var imgRt = DecodeImage(reg.ImagemAssinaturaResponsavel);
            if (imgRt is { Length: > 0 })
            {
                col.Item().Text("Assinatura do responsável técnico (imagem)").SemiBold().FontSize(9);
                col.Item().Width(260).Image(imgRt).FitArea();
            }

            col.Item().Text($"Registrado por: {reg.NomeUsuario}").FontSize(8).FontColor(Colors.Grey.Darken1);
            if (reg.QuantidadeAnexos > 0)
                col.Item().Text($"Anexos vinculados: {reg.QuantidadeAnexos} arquivo(s)").FontSize(8).FontColor(Colors.Grey.Darken1);
        });
    }

    private static void OcorrenciaBlock(IContainer container, OcorrenciaDto oc)
    {
        container.Border(1).BorderColor(Colors.Orange.Lighten4).Padding(10).Column(col =>
        {
            col.Spacing(4);
            col.Item().Text(oc.Titulo).SemiBold();
            col.Item().Text($"{TipoOcorrenciaPt(oc.Tipo)} — {oc.DataOcorrencia:dd/MM/yyyy}").FontSize(9).FontColor(Colors.Grey.Darken1);
            col.Item().Element(c => DrawKeyRowMultiline(c,"Descrição", oc.Descricao));
            if (!string.IsNullOrWhiteSpace(oc.Providencias))
                col.Item().Element(c => DrawKeyRowMultiline(c,"Providências", oc.Providencias!));
            col.Item().Text($"Registrado por: {oc.NomeUsuario}").FontSize(8).FontColor(Colors.Grey.Darken1);
            if (oc.QuantidadeAnexos > 0)
                col.Item().Text($"Anexos: {oc.QuantidadeAnexos}").FontSize(8).FontColor(Colors.Grey.Darken1);
        });
    }

    private static void DrawKeyRow(IContainer container, string label, string value) =>
        container.Row(r =>
        {
            r.ConstantItem(130).Text(label + ":").FontSize(9).FontColor(Colors.Grey.Darken1);
            r.RelativeItem().Text(string.IsNullOrWhiteSpace(value) ? "—" : value).FontSize(10);
        });

    private static void DrawKeyRowMultiline(IContainer container, string label, string value) =>
        container.Column(c =>
        {
            c.Item().Text(label + ":").SemiBold().FontSize(9).FontColor(Colors.Grey.Darken1);
            c.Item().Text(value).FontSize(10);
        });

    private static byte[]? DecodeImage(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return null;
        var s = raw.Trim();
        var comma = s.IndexOf(',');
        if (s.StartsWith("data:", StringComparison.OrdinalIgnoreCase) && comma > 0)
            s = s[(comma + 1)..];
        try
        {
            return Convert.FromBase64String(s);
        }
        catch
        {
            return null;
        }
    }

    private static List<string> EtapasMarcadas(RegistroDiarioDto reg)
    {
        var list = new List<string>();
        void Add(bool ok, string name)
        {
            if (ok) list.Add(name);
        }
        Add(reg.ServicosPreliminar, "Preliminares");
        Add(reg.Fundacao, "Fundação");
        Add(reg.Alvenarias, "Alvenarias");
        Add(reg.Superestrutura, "Superestrutura");
        Add(reg.Cobertura, "Cobertura");
        Add(reg.EsquadriasInstalacoesEletricasHidraulicas, "Esquadrias/inst.");
        Add(reg.RevestimentoForroParePiso, "Revestimento");
        Add(reg.Pintura, "Pintura");
        Add(reg.ServicosComplementares, "Complementares");
        return list;
    }

    private static string TipoObraPt(TipoObra t) => t switch
    {
        TipoObra.Residencial => "Residencial",
        TipoObra.Comercial => "Comercial",
        TipoObra.Industrial => "Industrial",
        TipoObra.Infraestrutura => "Infraestrutura",
        TipoObra.Outro => "Outro",
        _ => t.ToString()
    };

    private static string TipoEdificacaoPt(TipoEdificacao t) => t switch
    {
        TipoEdificacao.Residencial => "Residencial",
        TipoEdificacao.Comercial => "Comercial",
        TipoEdificacao.Industrial => "Industrial",
        _ => t.ToString()
    };

    private static string AtividadeTecnicaPt(AtividadeTecnica t) => t switch
    {
        AtividadeTecnica.Execucao => "Execução",
        AtividadeTecnica.Fiscalizacao => "Fiscalização",
        AtividadeTecnica.Projeto => "Projeto",
        _ => t.ToString()
    };

    private static string StatusObraPt(StatusObra s) => s switch
    {
        StatusObra.EmAndamento => "Em andamento",
        StatusObra.Concluida => "Concluída",
        StatusObra.Suspensa => "Suspensa",
        StatusObra.Cancelada => "Cancelada",
        _ => s.ToString()
    };

    private static string PosicaoObraPt(PosicaoObra p) => p switch
    {
        PosicaoObra.DeAcordoComProjeto => "De acordo com o projeto",
        PosicaoObra.EmDesacordoComProjeto => "Em desacordo com o projeto",
        PosicaoObra.EmAndamento => "Em andamento",
        PosicaoObra.Paralisada => "Paralisada",
        _ => p.ToString()
    };

    private static string TipoOcorrenciaPt(TipoOcorrencia t) => t switch
    {
        TipoOcorrencia.ProblemasTecnicos => "Problemas técnicos",
        TipoOcorrencia.Atraso => "Atraso",
        TipoOcorrencia.AlteracaoProjeto => "Alteração de projeto",
        TipoOcorrencia.AcidenteTrabalho => "Acidente de trabalho",
        TipoOcorrencia.Outro => "Outro",
        _ => t.ToString()
    };
}
