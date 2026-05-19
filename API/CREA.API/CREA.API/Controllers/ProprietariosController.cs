using CREA.Application.DTOs.Proprietarios;
using CREA.Application.Helpers;
using CREA.Application.Interfaces.Repositories;
using CREA.Domain.Entities;
using CREA.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CREA.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProprietariosController(
    IProprietarioRepository proprietarioRepository,
    IObraRepository obraRepository,
    IUsuarioRepository usuarioRepository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProprietarioDto>>> GetAll()
    {
        var list = await proprietarioRepository.GetAllAsync();
        return Ok(list.Select(ToDto));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProprietarioDto>> GetById(Guid id)
    {
        var p = await proprietarioRepository.GetByIdAsync(id);
        if (p is null) return NotFound();
        return Ok(ToDto(p));
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ProprietarioDto>> Create([FromBody] CreateProprietarioDto dto)
    {
        if (!string.IsNullOrWhiteSpace(dto.SenhaAcesso) && string.IsNullOrWhiteSpace(dto.Email))
            return BadRequest(new { mensagem = "E-mail é obrigatório para criar acesso de assinatura do proprietário." });

        if (!string.IsNullOrWhiteSpace(dto.Email))
        {
            var emailExistente = await usuarioRepository.GetByEmailAsync(dto.Email);
            if (emailExistente is not null)
                return Conflict(new { mensagem = "Já existe um usuário com este e-mail." });
        }

        var entity = new Proprietario
        {
            Nome = dto.Nome,
            Cpf = dto.Cpf ?? string.Empty,
            Email = dto.Email ?? string.Empty,
            Telefone = dto.Telefone ?? string.Empty
        };

        if (!string.IsNullOrWhiteSpace(dto.SenhaAcesso) && !string.IsNullOrWhiteSpace(dto.Email))
        {
            var usuario = new Usuario
            {
                Nome = dto.Nome,
                Email = dto.Email!,
                SenhaHash = SenhaHashHelper.Hash(dto.SenhaAcesso),
                TipoUsuario = TipoUsuario.Proprietario
            };
            await usuarioRepository.AddAsync(usuario);
            entity.UsuarioId = usuario.Id;
        }

        await proprietarioRepository.AddAsync(entity);
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, ToDto(entity));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateProprietarioDto dto)
    {
        var p = await proprietarioRepository.GetByIdAsync(id);
        if (p is null) return NotFound();

        p.Nome = dto.Nome;
        p.Cpf = dto.Cpf ?? string.Empty;
        p.Email = dto.Email ?? string.Empty;
        p.Telefone = dto.Telefone ?? string.Empty;

        await proprietarioRepository.UpdateAsync(p);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (!await proprietarioRepository.ExistsAsync(id)) return NotFound();
        if (await obraRepository.ExisteObraAtivaComProprietarioAsync(id))
            return Conflict(new { mensagem = "Não é possível excluir: existem obras ativas vinculadas a este proprietário." });

        await proprietarioRepository.DeleteAsync(id);
        return NoContent();
    }

    private static ProprietarioDto ToDto(Proprietario p) => new()
    {
        Id = p.Id,
        Nome = p.Nome,
        Cpf = p.Cpf,
        Email = p.Email,
        Telefone = p.Telefone,
        UsuarioId = p.UsuarioId,
        Ativo = p.Ativo,
        CriadoEm = p.CriadoEm
    };
}
