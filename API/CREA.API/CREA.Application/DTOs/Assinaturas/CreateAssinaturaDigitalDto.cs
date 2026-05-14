using System.ComponentModel.DataAnnotations;

namespace CREA.Application.DTOs.Assinaturas;

public class CreateAssinaturaDigitalDto
{
    [Required]
    public Guid RegistroDiarioId { get; set; }

    [Required]
    public Guid ProfissionalId { get; set; }

    public string? Observacao { get; set; }
}
