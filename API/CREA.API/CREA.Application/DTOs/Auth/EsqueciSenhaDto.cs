using System.ComponentModel.DataAnnotations;

namespace CREA.Application.DTOs.Auth;

public class EsqueciSenhaDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
