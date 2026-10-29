using System.ComponentModel.DataAnnotations;
using CREA.Domain.Enums;

namespace CREA.Application.DTOs.Assinaturas;

public class CreateAssinaturaDto
{
    [Required]
    public TipoEntidadeAssinatura TipoEntidade { get; set; }

    [Required]
    public Guid EntidadeId { get; set; }

    public string? Navegador { get; set; }
}
