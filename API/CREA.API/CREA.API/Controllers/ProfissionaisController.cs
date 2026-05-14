using CREA.Application.DTOs.Profissionais;
using CREA.Application.Interfaces.Repositories;
using CREA.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CREA.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfissionaisController(IProfissionalRepository profissionalRepository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProfissionalDto>>> GetAll()
    {
        var profissionais = await profissionalRepository.GetAllAsync();
        return Ok(profissionais.Select(ToDto));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProfissionalDto>> GetById(Guid id)
    {
        var profissional = await profissionalRepository.GetByIdAsync(id);
        if (profissional is null) return NotFound();
        return Ok(ToDto(profissional));
    }

    [HttpGet("por-registro/{numeroRegistro}")]
    public async Task<ActionResult<ProfissionalDto>> GetByNumeroRegistro(string numeroRegistro)
    {
        var profissional = await profissionalRepository.GetByNumeroRegistroAsync(numeroRegistro);
        if (profissional is null) return NotFound();
        return Ok(ToDto(profissional));
    }

    [HttpGet("por-tipo/{tipoRegistro}")]
    public async Task<ActionResult<IEnumerable<ProfissionalDto>>> GetByTipoRegistro(string tipoRegistro)
    {
        var profissionais = await profissionalRepository.GetByTipoRegistroAsync(tipoRegistro);
        return Ok(profissionais.Select(ToDto));
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ProfissionalDto>> Create([FromBody] CreateProfissionalDto dto)
    {
        if (await profissionalRepository.NumeroRegistroExisteAsync(dto.NumeroRegistro))
            return Conflict(new { mensagem = "Número de registro já cadastrado." });

        var profissional = new Profissional
        {
            Nome = dto.Nome,
            Cpf = dto.Cpf,
            NumeroRegistro = dto.NumeroRegistro,
            TipoRegistro = dto.TipoRegistro,
            Empresa = dto.Empresa,
            Especialidade = dto.Especialidade,
            Email = dto.Email,
            Telefone = dto.Telefone,
            UsuarioId = dto.UsuarioId
        };

        await profissionalRepository.AddAsync(profissional);
        return CreatedAtAction(nameof(GetById), new { id = profissional.Id }, ToDto(profissional));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateProfissionalDto dto)
    {
        var profissional = await profissionalRepository.GetByIdAsync(id);
        if (profissional is null) return NotFound();

        if (profissional.NumeroRegistro != dto.NumeroRegistro &&
            await profissionalRepository.NumeroRegistroExisteAsync(dto.NumeroRegistro))
            return Conflict(new { mensagem = "Número de registro já em uso." });

        profissional.Nome = dto.Nome;
        profissional.Cpf = dto.Cpf;
        profissional.NumeroRegistro = dto.NumeroRegistro;
        profissional.TipoRegistro = dto.TipoRegistro;
        profissional.Empresa = dto.Empresa;
        profissional.Especialidade = dto.Especialidade;
        profissional.Email = dto.Email;
        profissional.Telefone = dto.Telefone;
        profissional.UsuarioId = dto.UsuarioId;

        await profissionalRepository.UpdateAsync(profissional);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (!await profissionalRepository.ExistsAsync(id)) return NotFound();
        await profissionalRepository.DeleteAsync(id);
        return NoContent();
    }

    private static ProfissionalDto ToDto(Profissional p) => new()
    {
        Id = p.Id,
        Nome = p.Nome,
        Cpf = p.Cpf,
        NumeroRegistro = p.NumeroRegistro,
        TipoRegistro = p.TipoRegistro,
        Empresa = p.Empresa,
        Especialidade = p.Especialidade,
        Email = p.Email,
        Telefone = p.Telefone,
        UsuarioId = p.UsuarioId,
        Ativo = p.Ativo,
        CriadoEm = p.CriadoEm
    };
}
