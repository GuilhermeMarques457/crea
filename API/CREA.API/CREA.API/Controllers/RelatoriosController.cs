using CREA.API.Services;
using CREA.Application.DTOs.RelatoVisita;
using CREA.Application.DTOs.Relatorios;
using CREA.Application.Interfaces.Repositories;
using CREA.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CREA.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RelatoriosController(
    IObraRepository obraRepository,
    IRelatoVisitaRepository registroDiarioRepository,
    IAnexoRepository anexoRepository,
    ITermoConclusaoRepository termoConclusaoRepository,
    IAssinaturaRepository assinaturaRepository,
    IWebHostEnvironment env) : ControllerBase
{
    [HttpGet("obra/{obraId:guid}")]
    public async Task<ActionResult<RelatorioObraDto>> GetRelatorioObra(Guid obraId)
    {
        var relatorio = await BuildRelatorioAsync(obraId);
        return relatorio is null ? NotFound(new { mensagem = "Obra não encontrada." }) : Ok(relatorio);
    }

    [HttpGet("obra/{obraId:guid}/pdf")]
    public async Task<IActionResult> GetRelatorioObraPdf(Guid obraId)
    {
        var relatorio = await BuildRelatorioAsync(obraId);
        if (relatorio is null)
            return NotFound(new { mensagem = "Obra não encontrada." });

        var bytes = RelatorioObraPdfComposer.Generate(relatorio,
            Path.Combine(env.ContentRootPath, "uploads", "assinaturas"));
        var safeName = string.Join("_", relatorio.NomeObra.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .Trim();
        if (string.IsNullOrEmpty(safeName))
            safeName = "obra";
        var fileName = $"Relatorio_{safeName}_{DateTime.UtcNow:yyyyMMdd}.pdf";
        return File(bytes, "application/pdf", fileDownloadName: fileName);
    }

    private async Task<RelatorioObraDto?> BuildRelatorioAsync(Guid obraId)
    {
        var obra = await obraRepository.GetByIdWithDetailsAsync(obraId);
        if (obra is null) return null;

        var registros = (await registroDiarioRepository.GetByObraAsync(obraId)).ToList();
        var anexos = (await anexoRepository.GetByObraAsync(obraId)).ToList();
        var termo = await termoConclusaoRepository.GetByObraAsync(obraId);

        var assinaturasObra = (await assinaturaRepository.GetByEntidadeAsync(TipoEntidadeAssinatura.Obra, obraId)).ToList();
        var assinaturasTermo = termo is not null
            ? (await assinaturaRepository.GetByEntidadeAsync(TipoEntidadeAssinatura.TermoConclusao, termo.Id)).ToList()
            : [];

        var registrosDto = new List<RelatoVisitaDto>();
        foreach (var r in registros)
        {
            var assinaturasRelato = (await assinaturaRepository.GetByEntidadeAsync(TipoEntidadeAssinatura.RelatoVisita, r.Id)).ToList();
            registrosDto.Add(new RelatoVisitaDto
            {
                Id = r.Id,
                ObraId = r.ObraId,
                NomeObra = obra.Nome,
                NumeroSequencial = r.NumeroSequencial,
                Data = r.Data,
                Atividades = r.Atividades,
                EquipePresente = r.EquipePresente,
                CondicaoClimatica = r.CondicaoClimatica,
                Observacoes = r.Observacoes,
                ServicosPreliminar = r.ServicosPreliminar,
                Fundacao = r.Fundacao,
                Alvenarias = r.Alvenarias,
                Superestrutura = r.Superestrutura,
                Cobertura = r.Cobertura,
                EsquadriasInstalacoesEletricasHidraulicas = r.EsquadriasInstalacoesEletricasHidraulicas,
                RevestimentoForroParePiso = r.RevestimentoForroParePiso,
                Pintura = r.Pintura,
                ServicosComplementares = r.ServicosComplementares,
                PosicaoObra = r.PosicaoObra,
                DecisoesTecnicas = r.DecisoesTecnicas,
                UsuarioId = r.UsuarioId,
                NomeUsuario = r.Usuario?.Nome ?? string.Empty,
                Ativo = r.Ativo,
                CriadoEm = r.CriadoEm,
                TotalAssinaturas = assinaturasRelato.Count,
                AssinadoPeloProfissional = assinaturasRelato.Any(a => a.TipoAssinante == TipoAssinante.Profissional),
                AssinadoPeloProprietario = assinaturasRelato.Any(a => a.TipoAssinante == TipoAssinante.Proprietario),
                QuantidadeAnexos = r.Anexos?.Count(a => a.Ativo) ?? 0,
                Assinaturas = assinaturasRelato.Select(x => TermosConclusaoController.MapAssinatura(x, Request)).ToList()
            });
        }

        var termoAssinadoProfissional = assinaturasTermo.Any(a => a.TipoAssinante == TipoAssinante.Profissional);
        var termoAssinadoProprietario = assinaturasTermo.Any(a => a.TipoAssinante == TipoAssinante.Proprietario);

        return new RelatorioObraDto
        {
            ObraId = obra.Id,
            NomeObra = obra.Nome,
            Endereco = obra.Endereco,
            Cidade = obra.Cidade,
            Estado = obra.Estado,
            Proprietario = obra.Proprietario?.Nome ?? string.Empty,
            TelefoneProprietario = string.IsNullOrWhiteSpace(obra.Proprietario?.Telefone)
                ? null
                : obra.Proprietario!.Telefone,
            Empresa = obra.Empresa,
            NumeroCaderneta = obra.NumeroCaderneta,
            NumeroArt = obra.NumeroArt,
            NumeroRT = obra.NumeroRT,
            TipoObra = obra.TipoObra,
            TipoEdificacao = obra.TipoEdificacao,
            AtividadeTecnica = obra.AtividadeTecnica,
            DirecaoTecnica = obra.DirecaoTecnica,
            Status = obra.Status,
            DataInicio = obra.DataInicio,
            DataPrevisaoTermino = obra.DataPrevisaoTermino,
            NomeProfissionalResponsavel = obra.ProfissionalResponsavel?.Nome ?? string.Empty,
            NumeroRegistroProfissional = obra.ProfissionalResponsavel?.NumeroRegistro ?? string.Empty,
            TotalRelatoVisita = registros.Count,
            TotalAnexos = anexos.Count,
            PossuiTermoConclusao = termo is not null,
            DataConclusao = termo?.DataConclusao,
            AssinadoPeloProfissional = assinaturasObra.Any(a => a.TipoAssinante == TipoAssinante.Profissional),
            AssinadoPeloProprietario = termoAssinadoProprietario,
            AssinadoPeloCrea = assinaturasObra.Any(a => a.TipoAssinante == TipoAssinante.UsuarioCrea),
            TermoConcluido = termo is not null && termoAssinadoProfissional && termoAssinadoProprietario,
            TermoNumero = termo?.NumeroTermo,
            TermoDescricao = termo?.Descricao,
            TermoObservacoes = termo?.Observacoes,
            TermoLocalObra = termo?.LocalObra,
            TermoDeclaracaoTexto = termo?.DeclaracaoTexto,
            AssinaturasObra = assinaturasObra.Select(x => TermosConclusaoController.MapAssinatura(x, Request)).ToList(),
            AssinaturasTermo = assinaturasTermo.Select(x => TermosConclusaoController.MapAssinatura(x, Request)).ToList(),
            RelatoVisita = registrosDto,
            GeradoEm = DateTime.UtcNow
        };
    }
}
