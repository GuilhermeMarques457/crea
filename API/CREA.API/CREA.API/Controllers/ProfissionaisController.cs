using CREA.Application.DTOs.Profissionais;
using CREA.Application.Interfaces.Repositories;
using CREA.Domain.Entities;
using CREA.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CREA.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfissionaisController(
    IProfissionalRepository profissionalRepository,
    IUsuarioRepository usuarioRepository,
    IObraRepository obraRepository) : ControllerBase
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

        if (dto.UsuarioId.HasValue)
        {
            var usuario = await usuarioRepository.GetByIdAsync(dto.UsuarioId.Value);
            if (usuario is null)
                return BadRequest(new { mensagem = "Usuário não encontrado." });
            if (usuario.TipoUsuario != TipoUsuario.ResponsavelTecnico)
                return BadRequest(new { mensagem = "O usuário selecionado não é do tipo Responsável Técnico." });
            var jaVinculado = await profissionalRepository.GetByUsuarioIdAsync(dto.UsuarioId.Value);
            if (jaVinculado is not null)
                return Conflict(new { mensagem = "Este usuário já está vinculado a outro profissional." });
        }

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

        if (dto.UsuarioId.HasValue)
        {
            var usuario = await usuarioRepository.GetByIdAsync(dto.UsuarioId.Value);
            if (usuario is null)
                return BadRequest(new { mensagem = "Usuário não encontrado." });
            if (usuario.TipoUsuario != TipoUsuario.ResponsavelTecnico)
                return BadRequest(new { mensagem = "O usuário selecionado não é do tipo Responsável Técnico." });
            var jaVinculado = await profissionalRepository.GetByUsuarioIdAsync(dto.UsuarioId.Value);
            if (jaVinculado is not null && jaVinculado.Id != id)
                return Conflict(new { mensagem = "Este usuário já está vinculado a outro profissional." });
        }

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
        if (await obraRepository.ExisteObraAtivaComProfissionalAsync(id))
            return Conflict(new { mensagem = "Não é possível excluir: existem obras ativas vinculadas a este profissional." });
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
