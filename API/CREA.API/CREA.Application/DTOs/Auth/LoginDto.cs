using System.ComponentModel.DataAnnotations;

namespace CREA.Application.DTOs.Auth;

public class LoginDto
{
    [Required]
    [MaxLength(14)]
    public string Cpf { get; set; } = string.Empty;

    [Required]
    public string Senha { get; set; } = string.Empty;
}
