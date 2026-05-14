using CREA.Application.DTOs.Relatorios;
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
        var obra = await obraRepository.GetByIdWithDetailsAsync(obraId);
        if (obra is null) return NotFound(new { mensagem = "Obra não encontrada." });

        var registros = (await registroDiarioRepository.GetByObraAsync(obraId)).ToList();
        var ocorrencias = (await ocorrenciaRepository.GetByObraAsync(obraId)).ToList();
        var anexos = (await anexoRepository.GetByObraAsync(obraId)).ToList();
        var termo = await termoConclusaoRepository.GetByObraAsync(obraId);

        var relatorio = new RelatorioObraDto
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
            Assinaturas = termo?.Assinaturas?.Where(a => a.Ativo).Select(a => new Application.DTOs.TermosConclusao.AssinaturaTermoConclusaoDto
            {
                Id = a.Id,
                TermoConclusaoId = a.TermoConclusaoId,
                UsuarioId = a.UsuarioId,
                NomeUsuario = a.Usuario?.Nome ?? string.Empty,
                TipoAssinante = a.TipoAssinante.ToString(),
                HashAssinatura = a.HashAssinatura,
                DataAssinatura = a.DataAssinatura,
                ImagemAssinatura = a.ImagemAssinatura,
                IpAssinante = a.IpAssinante
            }).ToList() ?? [],
            RegistrosDiarios = registros.Select(r => new Application.DTOs.RegistrosDiarios.RegistroDiarioDto
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
                AssinaturaProprietario = r.AssinaturaProprietario,
                DataAssinaturaProprietario = r.DataAssinaturaProprietario,
                UsuarioId = r.UsuarioId,
                NomeUsuario = r.Usuario?.Nome ?? string.Empty,
                Ativo = r.Ativo,
                CriadoEm = r.CriadoEm
            }),
            Ocorrencias = ocorrencias.Select(o => new Application.DTOs.Ocorrencias.OcorrenciaDto
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
                CriadoEm = o.CriadoEm
            }),
            GeradoEm = DateTime.UtcNow
        };

        return Ok(relatorio);
    }
}
