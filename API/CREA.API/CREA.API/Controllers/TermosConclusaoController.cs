using Azure.Core;
using CREA.Application.DTOs.Assinaturas;
using CREA.Application.DTOs.TermosConclusao;
using CREA.Application.Interfaces.Repositories;
using CREA.Domain.Entities;
using CREA.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CREA.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TermosConclusaoController(
    ITermoConclusaoRepository termoConclusaoRepository,
    IObraRepository obraRepository,
    IProfissionalRepository profissionalRepository,
    IAssinaturaRepository assinaturaRepository) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TermoConclusaoDto>> GetById(Guid id)
    {
        var termo = await termoConclusaoRepository.GetByIdAsync(id);
        if (termo is null) return NotFound();
        return Ok(await ToDtoAsync(termo));
    }

    [HttpGet("por-obra/{obraId:guid}")]
    public async Task<ActionResult<TermoConclusaoDto>> GetByObra(Guid obraId)
    {
        var termo = await termoConclusaoRepository.GetByObraAsync(obraId);
        if (termo is null) return NotFound(new { mensagem = "Termo de conclusão não encontrado para esta obra." });
        return Ok(await ToDtoAsync(termo));
    }

    [HttpPost]
    [Authorize(Roles = "ResponsavelTecnico,Administrador")]
    public async Task<ActionResult<TermoConclusaoDto>> Create([FromBody] CreateTermoConclusaoDto dto)
    {
        if (!await obraRepository.ExistsAsync(dto.ObraId))
            return BadRequest(new { mensagem = "Obra não encontrada." });

        if (await termoConclusaoRepository.ObraPossuiTermoAsync(dto.ObraId))
            return Conflict(new { mensagem = "Esta obra já possui um termo de conclusão." });

        var termo = new TermoConclusao
        {
            ObraId = dto.ObraId,
            NumeroTermo = dto.NumeroTermo,
            DataConclusao = dto.DataConclusao,
            Descricao = dto.Descricao,
            Observacoes = dto.Observacoes,
            DeclaracaoTexto = dto.DeclaracaoTexto,
            LocalDeclaracao = dto.LocalDeclaracao,
            DataDeclaracao = dto.DataDeclaracao,
        };

        await termoConclusaoRepository.AddAsync(termo);

        var criado = await termoConclusaoRepository.GetByObraAsync(dto.ObraId);
        return CreatedAtAction(nameof(GetById), new { id = termo.Id }, await ToDtoAsync(criado!));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (!await termoConclusaoRepository.ExistsAsync(id)) return NotFound();
        await termoConclusaoRepository.DeleteAsync(id);
        return NoContent();
    }

    private async Task<TermoConclusaoDto> ToDtoAsync(TermoConclusao t)
    {
        var assinaturas = (await assinaturaRepository.GetByEntidadeAsync(TipoEntidadeAssinatura.TermoConclusao, t.Id)).ToList();
        var assinadoProfissional = assinaturas.Any(a => a.TipoAssinante == TipoAssinante.Profissional);
        var assinadoProprietario = assinaturas.Any(a => a.TipoAssinante == TipoAssinante.Proprietario);

        if (t.Obra is null && t.ObraId != Guid.Empty)
            t = await termoConclusaoRepository.GetByObraAsync(t.ObraId) ?? t;

        return new TermoConclusaoDto
        {
            Id = t.Id,
            ObraId = t.ObraId,
            NomeObra = t.Obra?.Nome ?? string.Empty,
            NumeroTermo = t.NumeroTermo,
            DataConclusao = t.DataConclusao,
            Descricao = t.Descricao,
            Observacoes = t.Observacoes,
            DeclaracaoTexto = t.DeclaracaoTexto,
            LocalDeclaracao = t.LocalDeclaracao,
            DataDeclaracao = t.DataDeclaracao,
            ProfissionalId = t.Obra?.ProfissionalResponsavelId ?? Guid.Empty,
            NomeProfissional = t.Obra?.ProfissionalResponsavel?.Nome ?? string.Empty,
            NumeroRegistro = t.Obra?.ProfissionalResponsavel?.NumeroRegistro ?? string.Empty,
            CriadoEm = t.CriadoEm,
            AssinadoPeloProfissional = assinadoProfissional,
            AssinadoPeloProprietario = assinadoProprietario,
            Concluido = assinadoProfissional && assinadoProprietario,
            Assinaturas = assinaturas.Select(x => MapAssinatura(x, Request)).ToList()
        };
    }

    internal static AssinaturaDto MapAssinatura(Assinatura a, HttpRequest request) => new()
    {
        Id = a.Id,
        TipoEntidade = a.TipoEntidade,
        EntidadeId = a.EntidadeId,
        TipoAssinante = a.TipoAssinante,
        UsuarioId = a.UsuarioId,
        NomeUsuario = a.Usuario?.Nome ?? string.Empty,
        HashAssinatura = a.HashAssinatura,
        DataAssinatura = a.DataAssinatura,
        ImagemAssinatura = a.ImagemAssinatura,
        IpAssinante = a.IpAssinante,
        UserAgent = a.UserAgent,
        Navegador = a.Navegador,
        SistemaOperacional = a.SistemaOperacional,
        Dispositivo = a.Dispositivo,
        UrlImagemAssinatura = $"{request.Scheme}://{request.Host}/api/assinaturas/imagem/{a.Id}",
    };
}
