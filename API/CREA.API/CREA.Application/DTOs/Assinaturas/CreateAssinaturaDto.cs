using System.ComponentModel.DataAnnotations;
using CREA.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace CREA.Application.DTOs.Assinaturas;

public class CreateAssinaturaDto
{
    [Required]
    public TipoEntidadeAssinatura TipoEntidade { get; set; }

    [Required]
    public Guid EntidadeId { get; set; }

    [Required]
    public IFormFile ImagemAssinatura { get; set; } = null!;

    public string? Navegador { get; set; }
    public string? SistemaOperacional { get; set; }
    public string? Dispositivo { get; set; }
}
