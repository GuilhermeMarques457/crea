using System.ComponentModel.DataAnnotations;

namespace CREA.Application.DTOs.Auth;

public class TrocarSenhaDto
{
    [Required]
    public string SenhaAtual { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string NovaSenha { get; set; } = string.Empty;
}
