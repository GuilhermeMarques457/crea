using System.ComponentModel.DataAnnotations;
using CREA.Domain.Enums;

namespace CREA.Application.DTOs.Assinaturas;

public class CreateAssinaturaDto
{
    [Required]
    public TipoEntidadeAssinatura TipoEntidade { get; set; }

    [Required]
    public Guid EntidadeId { get; set; }

    [Required]
    public string ImagemAssinatura { get; set; } = string.Empty;

    public string? Navegador { get; set; }
    public string? SistemaOperacional { get; set; }
    public string? Dispositivo { get; set; }
}
