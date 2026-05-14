using CREA.API.Services;
using CREA.Application.DTOs.Ocorrencias;
using CREA.Application.DTOs.RegistrosDiarios;
using CREA.Application.DTOs.Relatorios;
using CREA.Application.DTOs.TermosConclusao;
using CREA.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CREA.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RelatoriosController(
    IObraRepository obraRepository,
    IRegistroDiarioRepository registroDiarioRepository,
    IOcorrenciaRepository ocorrenciaRepository,
    IAnexoRepository anexoRepository,
    ITermoConclusaoRepository termoConclusaoRepository) : ControllerBase
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

        var bytes = RelatorioObraPdfComposer.Generate(relatorio);
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
        var ocorrencias = (await ocorrenciaRepository.GetByObraAsync(obraId)).ToList();
        var anexos = (await anexoRepository.GetByObraAsync(obraId)).ToList();
        var termo = await termoConclusaoRepository.GetByObraAsync(obraId);

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
            TotalRegistrosDiarios = registros.Count,
            TotalOcorrencias = ocorrencias.Count,
            TotalAnexos = anexos.Count,
            PossuiTermoConclusao = termo is not null,
            DataConclusao = termo?.DataConclusao,
            AssinadoPeloResponsavel = termo?.AssinadoPeloResponsavel ?? false,
            AssinadoPeloAdmin = termo?.AssinadoPeloAdmin ?? false,
            TermoConcluido = termo is not null && termo.AssinadoPeloResponsavel && termo.AssinadoPeloAdmin,
            TermoNumero = termo?.NumeroTermo,
            TermoDescricao = termo?.Descricao,
            TermoObservacoes = termo?.Observacoes,
            TermoLocalObra = termo?.LocalObra,
            TermoDeclaracaoTexto = termo?.DeclaracaoTexto,
            TermoAssinaturaProprietario = termo?.AssinaturaProprietario,
            TermoDataAssinaturaProprietario = termo?.DataAssinaturaProprietario,
            Assinaturas = termo?.Assinaturas?.Where(a => a.Ativo).Select(a => new AssinaturaTermoConclusaoDto
            {
                Id = a.Id,
                TermoConclusaoId = a.TermoConclusaoId,
                UsuarioId = a.UsuarioId,
                NomeUsuario = a.Usuario?.Nome ?? string.Empty,
                TipoAssinante = a.TipoAssinante,
                HashAssinatura = a.HashAssinatura,
                DataAssinatura = a.DataAssinatura,
                ImagemAssinatura = a.ImagemAssinatura ?? string.Empty,
                IpAssinante = a.IpAssinante ?? string.Empty
            }).ToList() ?? [],
            RegistrosDiarios = registros.Select(r => new RegistroDiarioDto
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
                ImagemAssinaturaResponsavel = r.ImagemAssinaturaResponsavel,
                DataAssinaturaResponsavel = r.DataAssinaturaResponsavel,
                UsuarioId = r.UsuarioId,
                NomeUsuario = r.Usuario?.Nome ?? string.Empty,
                Ativo = r.Ativo,
                CriadoEm = r.CriadoEm,
                TotalAssinaturas = (r.DataAssinaturaResponsavel.HasValue ? 1 : 0),
                QuantidadeAnexos = r.Anexos?.Count(a => a.Ativo) ?? 0
            }),
            Ocorrencias = ocorrencias.Select(o => new OcorrenciaDto
            {
                Id = o.Id,
                ObraId = o.ObraId,
                NomeObra = obra.Nome,
                DataOcorrencia = o.DataOcorrencia,
                Tipo = o.Tipo,
                Titulo = o.Titulo,
                Descricao = o.Descricao,
                Providencias = o.Providencias,
                UsuarioId = o.UsuarioId,
                NomeUsuario = o.Usuario?.Nome ?? string.Empty,
                Ativo = o.Ativo,
                CriadoEm = o.CriadoEm,
                QuantidadeAnexos = o.Anexos?.Count(a => a.Ativo) ?? 0
            }),
            GeradoEm = DateTime.UtcNow
        };
    }
}
