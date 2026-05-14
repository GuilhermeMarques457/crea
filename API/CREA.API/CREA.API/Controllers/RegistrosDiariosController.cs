using System.Security.Claims;
using CREA.Application.DTOs.RegistrosDiarios;
using CREA.Application.Interfaces.Repositories;
using CREA.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CREA.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RegistrosDiariosController(
    IRegistroDiarioRepository registroDiarioRepository,
    IObraRepository obraRepository) : ControllerBase
{
    [HttpGet("por-obra/{obraId:guid}")]
    public async Task<ActionResult<IEnumerable<RegistroDiarioDto>>> GetByObra(Guid obraId)
    {
        if (!await obraRepository.ExistsAsync(obraId)) return NotFound(new { mensagem = "Obra não encontrada." });

        var registros = await registroDiarioRepository.GetByObraAsync(obraId);
        return Ok(registros.Select(ToDto));
    }

    [HttpGet("por-obra/{obraId:guid}/periodo")]
    public async Task<ActionResult<IEnumerable<RegistroDiarioDto>>> GetByPeriodo(
        Guid obraId, [FromQuery] DateTime inicio, [FromQuery] DateTime fim)
    {
        if (!await obraRepository.ExistsAsync(obraId)) return NotFound(new { mensagem = "Obra não encontrada." });

        var registros = await registroDiarioRepository.GetByObraAndPeriodoAsync(obraId, inicio, fim);
        return Ok(registros.Select(ToDto));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RegistroDiarioDto>> GetById(Guid id)
    {
        var registro = await registroDiarioRepository.GetByIdWithDetailsAsync(id);
        if (registro is null) return NotFound();
        return Ok(ToDto(registro));
    }

    [HttpPost]
    public async Task<ActionResult<RegistroDiarioDto>> Create([FromBody] CreateRegistroDiarioDto dto)
    {
        if (!await obraRepository.ExistsAsync(dto.ObraId))
            return BadRequest(new { mensagem = "Obra não encontrada." });

        var usuarioId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var existentes = await registroDiarioRepository.GetByObraAsync(dto.ObraId);
        var proximoNumero = existentes.Any() ? existentes.Max(r => r.NumeroSequencial) + 1 : 1;

        var registro = new RegistroDiario
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
            AssinaturaProprietario = dto.AssinaturaProprietario,
            DataAssinaturaProprietario = dto.DataAssinaturaProprietario,
            ImagemAssinaturaResponsavel = dto.ImagemAssinaturaResponsavel,
            DataAssinaturaResponsavel = dto.ImagemAssinaturaResponsavel != null ? DateTime.UtcNow : null,
            UsuarioId = usuarioId
        };

        await registroDiarioRepository.AddAsync(registro);
        var criado = await registroDiarioRepository.GetByIdWithDetailsAsync(registro.Id);
        return CreatedAtAction(nameof(GetById), new { id = registro.Id }, ToDto(criado!));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateRegistroDiarioDto dto)
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
        registro.AssinaturaProprietario = dto.AssinaturaProprietario;
        registro.DataAssinaturaProprietario = dto.DataAssinaturaProprietario;
        if (dto.ImagemAssinaturaResponsavel != null)
        {
            registro.ImagemAssinaturaResponsavel = dto.ImagemAssinaturaResponsavel;
            registro.DataAssinaturaResponsavel = DateTime.UtcNow;
        }

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

    [HttpGet("pendentes-assinatura")]
    [Authorize(Roles = "ResponsavelTecnico,Administrador")]
    public async Task<ActionResult<IEnumerable<RegistroDiarioDto>>> GetPendentesAssinatura()
    {
        var usuarioId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var registros = await registroDiarioRepository.GetPendentesAssinaturaAsync(usuarioId);
        return Ok(registros.Select(ToDto));
    }

    [HttpPost("{id:guid}/assinar")]
    [Authorize(Roles = "ResponsavelTecnico,Administrador")]
    public async Task<ActionResult<RegistroDiarioDto>> Assinar(Guid id, [FromBody] AssinarRegistroDiarioDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.ImagemAssinatura))
            return BadRequest(new { mensagem = "A imagem da assinatura é obrigatória." });

        var registro = await registroDiarioRepository.GetByIdWithDetailsAsync(id);
        if (registro is null) return NotFound();

        if (registro.ImagemAssinaturaResponsavel != null)
            return Conflict(new { mensagem = "Este registro já foi assinado." });

        registro.ImagemAssinaturaResponsavel = dto.ImagemAssinatura;
        registro.DataAssinaturaResponsavel = DateTime.UtcNow;

        await registroDiarioRepository.UpdateAsync(registro);

        var updated = await registroDiarioRepository.GetByIdWithDetailsAsync(id);
        return Ok(ToDto(updated!));
    }

    private static RegistroDiarioDto ToDto(RegistroDiario r) => new()
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
        AssinaturaProprietario = r.AssinaturaProprietario,
        DataAssinaturaProprietario = r.DataAssinaturaProprietario,
        ImagemAssinaturaResponsavel = r.ImagemAssinaturaResponsavel,
        DataAssinaturaResponsavel = r.DataAssinaturaResponsavel,
        UsuarioId = r.UsuarioId,
        NomeUsuario = r.Usuario?.Nome ?? string.Empty,
        Ativo = r.Ativo,
        CriadoEm = r.CriadoEm
    };
}
