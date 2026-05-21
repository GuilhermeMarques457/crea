using CREA.Application.DTOs.Assinaturas;
using CREA.Application.DTOs.RelatoVisita;
using CREA.Application.DTOs.Relatorios;
using CREA.Domain.Enums;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CREA.API.Services;

public static class RelatorioObraPdfComposer
{
    static RelatorioObraPdfComposer() =>
        QuestPDF.Settings.License = LicenseType.Community;

    public static byte[] Generate(RelatorioObraDto r, string assinaturasPath) =>
        Document.Create(d => d.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(36);
            page.DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Black));
            page.Header().Element(c => DrawHeader(c, r));
            page.Content().Element(c => DrawBody(c, r, assinaturasPath));
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

    private static void DrawBody(IContainer container, RelatorioObraDto r, string assinaturasPath)
    {
        container.PaddingVertical(12).Column(col =>
        {
            col.Spacing(10);
            col.Item().Text($"Gerado em: {r.GeradoEm:dd/MM/yyyy HH:mm} (UTC)").FontSize(8).FontColor(Colors.Grey.Darken1);

            col.Item().Text("Resumo").SemiBold().FontSize(12);
            col.Item().Row(row =>
            {
                row.Spacing(12);
                row.RelativeItem().Element(c => DrawMetric(c, "Relato de visita", r.TotalRelatoVisita.ToString()));
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

            col.Item().Text("Assinaturas da obra").SemiBold().FontSize(11);
            if (r.AssinaturasObra.Count == 0)
                col.Item().Text("Nenhuma assinatura digital registrada.").Italic().FontColor(Colors.Grey.Darken1);
            else
            {
                foreach (var a in r.AssinaturasObra)
                    col.Item().Element(c => AssinaturaItem(c, a, assinaturasPath));
            }

            if (r.PossuiTermoConclusao)
            {
                col.Item().Text("Termo de conclusão").SemiBold().FontSize(12);
                col.Item().Column(block =>
                {
                    block.Spacing(4);
                    block.Item().Element(c => DrawKeyRow(c,"Situação", r.TermoConcluido ? "Concluído" : "Pendente de assinatura(s)"));
                    block.Item().Element(c => DrawKeyRow(c,"Profissional assinou", r.AssinadoPeloProfissional ? "Sim" : "Não"));
                    block.Item().Element(c => DrawKeyRow(c,"Proprietário assinou (termo)", r.AssinadoPeloProprietario ? "Sim" : "Não"));
                    if (r.DataConclusao.HasValue)
                        block.Item().Element(c => DrawKeyRow(c,"Data conclusão", r.DataConclusao.Value.ToString("dd/MM/yyyy")));
                    if (r.TermoNumero.HasValue)
                        block.Item().Element(c => DrawKeyRow(c,"Nº termo", r.TermoNumero.Value.ToString()));
                    if (!string.IsNullOrWhiteSpace(r.TermoDescricao))
                        block.Item().Element(c => DrawKeyRowMultiline(c,"Descrição", r.TermoDescricao!));
                    if (!string.IsNullOrWhiteSpace(r.TermoObservacoes))
                        block.Item().Element(c => DrawKeyRowMultiline(c,"Observações (termo)", r.TermoObservacoes!));
                    if (!string.IsNullOrWhiteSpace(r.TermoDeclaracaoTexto))
                        block.Item().Element(c => DrawKeyRowMultiline(c,"Declaração", r.TermoDeclaracaoTexto!));
                });

                col.Item().Text("Assinaturas do termo").SemiBold().FontSize(11);
                if (r.AssinaturasTermo.Count == 0)
                    col.Item().Text("Nenhuma assinatura digital registrada.").Italic().FontColor(Colors.Grey.Darken1);
                else
                {
                    foreach (var a in r.AssinaturasTermo)
                        col.Item().Element(c => AssinaturaItem(c, a, assinaturasPath));
                }
            }

            col.Item().Text("Relato de visita").SemiBold().FontSize(12);
            var regs = r.RelatoVisita.ToList();
            if (regs.Count == 0)
                col.Item().Text("Nenhum registro diário.").Italic().FontColor(Colors.Grey.Darken1);
            else
            {
                foreach (var reg in regs.OrderBy(x => x.NumeroSequencial))
                    col.Item().Element(c => RelatoVisitaBlock(c, reg, assinaturasPath));
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

    private static void AssinaturaItem(IContainer container, AssinaturaDto a, string assinaturasPath)
    {
        container.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(col =>
        {
            col.Spacing(6);
            col.Item().Text($"{a.NomeUsuario} ({TipoAssinantePt(a.TipoAssinante)})").SemiBold();
            col.Item().Text($"Assinado em: {a.DataAssinatura:dd/MM/yyyy HH:mm}").FontSize(9).FontColor(Colors.Grey.Darken1);
            if (!string.IsNullOrWhiteSpace(a.IpAssinante))
                col.Item().Text($"IP: {a.IpAssinante}").FontSize(8).FontColor(Colors.Grey.Darken1);
            if (!string.IsNullOrWhiteSpace(a.Navegador))
                col.Item().Text($"Navegador: {a.Navegador}").FontSize(8).FontColor(Colors.Grey.Darken1);
            var img = ReadImageFile(assinaturasPath, a.ImagemAssinatura);
            if (img is { Length: > 0 })
                col.Item().Width(260).Image(img).FitArea();
        });
    }

    private static string TipoAssinantePt(TipoAssinante t) => t switch
    {
        TipoAssinante.Profissional => "Profissional",
        TipoAssinante.UsuarioCrea => "CREA",
        TipoAssinante.Proprietario => "Proprietário",
        _ => t.ToString()
    };

    private static void RelatoVisitaBlock(IContainer container, RelatoVisitaDto reg, string assinaturasPath)
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

            if (reg.Assinaturas.Count > 0)
            {
                col.Item().Text("Assinaturas digitais").SemiBold().FontSize(9);
                foreach (var assinatura in reg.Assinaturas)
                    col.Item().Element(c => AssinaturaItem(c, assinatura, assinaturasPath));
            }

            col.Item().Text($"Registrado por: {reg.NomeUsuario}").FontSize(8).FontColor(Colors.Grey.Darken1);
            if (reg.QuantidadeAnexos > 0)
                col.Item().Text($"Anexos vinculados: {reg.QuantidadeAnexos} arquivo(s)").FontSize(8).FontColor(Colors.Grey.Darken1);
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

    private static byte[]? ReadImageFile(string assinaturasPath, string? fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName)) return null;
        var path = Path.Combine(assinaturasPath, fileName);
        if (!File.Exists(path)) return null;
        try
        {
            return File.ReadAllBytes(path);
        }
        catch
        {
            return null;
        }
    }

    private static List<string> EtapasMarcadas(RelatoVisitaDto reg)
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
}
