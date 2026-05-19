using System.Security.Claims;
using CREA.Application.DTOs.RegistrosDiarios;
using CREA.Application.Interfaces.Repositories;
using CREA.Domain.Entities;
using CREA.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static CREA.API.Controllers.TermosConclusaoController;

namespace CREA.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RegistrosDiariosController(
    IRelatoVisitaRepository registroDiarioRepository,
    IObraRepository obraRepository,
    IAssinaturaRepository assinaturaRepository) : ControllerBase
{
    [HttpGet("por-obra/{obraId:guid}")]
    public async Task<ActionResult<IEnumerable<RelatoVisitaDto>>> GetByObra(Guid obraId)
    {
        if (!await obraRepository.ExistsAsync(obraId)) return NotFound(new { mensagem = "Obra não encontrada." });

        var registros = await registroDiarioRepository.GetByObraAsync(obraId);
        var dtos = new List<RelatoVisitaDto>();
        foreach (var r in registros)
            dtos.Add(await ToDtoAsync(r));
        return Ok(dtos);
    }

    [HttpGet("por-obra/{obraId:guid}/periodo")]
    public async Task<ActionResult<IEnumerable<RelatoVisitaDto>>> GetByPeriodo(
        Guid obraId, [FromQuery] DateTime inicio, [FromQuery] DateTime fim)
    {
        if (!await obraRepository.ExistsAsync(obraId)) return NotFound(new { mensagem = "Obra não encontrada." });

        var registros = await registroDiarioRepository.GetByObraAndPeriodoAsync(obraId, inicio, fim);
        var dtos = new List<RelatoVisitaDto>();
        foreach (var r in registros)
            dtos.Add(await ToDtoAsync(r));
        return Ok(dtos);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RelatoVisitaDto>> GetById(Guid id)
    {
        var registro = await registroDiarioRepository.GetByIdWithDetailsAsync(id);
        if (registro is null) return NotFound();
        return Ok(await ToDtoAsync(registro));
    }

    [HttpPost]
    public async Task<ActionResult<RelatoVisitaDto>> Create([FromBody] CreateRelatoVisitaDto dto)
    {
        if (!await obraRepository.ExistsAsync(dto.ObraId))
            return BadRequest(new { mensagem = "Obra não encontrada." });

        var usuarioId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var existentes = await registroDiarioRepository.GetByObraAsync(dto.ObraId);
        var proximoNumero = existentes.Any() ? existentes.Max(r => r.NumeroSequencial) + 1 : 1;

        var registro = new RelatoVisita
        {
            ObraId = dto.ObraId,
            NumeroSequencial = proximoNumero,
            Data = dto.Data,
            Atividades = dto.Atividades,
            EquipePresente = dto.EquipePresente,
            CondicaoClimatica = dto.CondicaoClimatica,
            Observacoes = dto.Observacoes,
            ServicosPreliminar = dto.ServicosPreliminar,
            Fundacao = dto.Fundacao,
            Alvenarias = dto.Alvenarias,
            Superestrutura = dto.Superestrutura,
            Cobertura = dto.Cobertura,
            EsquadriasInstalacoesEletricasHidraulicas = dto.EsquadriasInstalacoesEletricasHidraulicas,
            RevestimentoForroParePiso = dto.RevestimentoForroParePiso,
            Pintura = dto.Pintura,
            ServicosComplementares = dto.ServicosComplementares,
            PosicaoObra = dto.PosicaoObra,
            DecisoesTecnicas = dto.DecisoesTecnicas,
            UsuarioId = usuarioId
        };

        await registroDiarioRepository.AddAsync(registro);
        var criado = await registroDiarioRepository.GetByIdWithDetailsAsync(registro.Id);
        return CreatedAtAction(nameof(GetById), new { id = registro.Id }, await ToDtoAsync(criado!));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateRelatoVisitaDto dto)
    {
        var registro = await registroDiarioRepository.GetByIdAsync(id);
        if (registro is null) return NotFound();

        registro.Data = dto.Data;
        registro.Atividades = dto.Atividades;
        registro.EquipePresente = dto.EquipePresente;
        registro.CondicaoClimatica = dto.CondicaoClimatica;
        registro.Observacoes = dto.Observacoes;
        registro.ServicosPreliminar = dto.ServicosPreliminar;
        registro.Fundacao = dto.Fundacao;
        registro.Alvenarias = dto.Alvenarias;
        registro.Superestrutura = dto.Superestrutura;
        registro.Cobertura = dto.Cobertura;
        registro.EsquadriasInstalacoesEletricasHidraulicas = dto.EsquadriasInstalacoesEletricasHidraulicas;
        registro.RevestimentoForroParePiso = dto.RevestimentoForroParePiso;
        registro.Pintura = dto.Pintura;
        registro.ServicosComplementares = dto.ServicosComplementares;
        registro.PosicaoObra = dto.PosicaoObra;
        registro.DecisoesTecnicas = dto.DecisoesTecnicas;

        await registroDiarioRepository.UpdateAsync(registro);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Administrador,ResponsavelTecnico")]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (!await registroDiarioRepository.ExistsAsync(id)) return NotFound();
        await registroDiarioRepository.DeleteAsync(id);
        return NoContent();
    }

    private async Task<RelatoVisitaDto> ToDtoAsync(RelatoVisita r)
    {
        var assinaturas = (await assinaturaRepository.GetByEntidadeAsync(TipoEntidadeAssinatura.RelatoVisita, r.Id)).ToList();
        var assinadoProfissional = assinaturas.Any(a => a.TipoAssinante == TipoAssinante.Profissional);
        var assinadoProprietario = assinaturas.Any(a => a.TipoAssinante == TipoAssinante.Proprietario);

        return new RelatoVisitaDto
        {
            Id = r.Id,
            ObraId = r.ObraId,
            NomeObra = r.Obra?.Nome ?? string.Empty,
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
            TotalAssinaturas = assinaturas.Count,
            AssinadoPeloProfissional = assinadoProfissional,
            AssinadoPeloProprietario = assinadoProprietario,
            QuantidadeAnexos = r.Anexos?.Count(a => a.Ativo) ?? 0,
            Assinaturas = assinaturas.Select(MapAssinatura).ToList()
        };
    }
}
