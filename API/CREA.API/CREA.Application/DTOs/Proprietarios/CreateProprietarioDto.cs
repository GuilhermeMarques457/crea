using System.ComponentModel.DataAnnotations;

namespace CREA.Application.DTOs.Proprietarios;

public class CreateProprietarioDto
{
    [Required]
    [MaxLength(200)]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(14)]
    public string? Cpf { get; set; }

    [MaxLength(200)]
    public string? Email { get; set; }

    [MaxLength(20)]
    public string? Telefone { get; set; }

    /// <summary>Se informado, cria usuário de acesso (TipoUsuario.Proprietario) vinculado 1:1.</summary>
    [MinLength(6)]
    public string? SenhaAcesso { get; set; }
}
