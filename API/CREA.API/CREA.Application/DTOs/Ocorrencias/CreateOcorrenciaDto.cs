using System.ComponentModel.DataAnnotations;
using CREA.Domain.Enums;

namespace CREA.Application.DTOs.Ocorrencias;

public class CreateOcorrenciaDto
{
    [Required]
    public Guid ObraId { get; set; }

    [Required]
    public DateTime DataOcorrencia { get; set; }

    [Required]
    public TipoOcorrencia Tipo { get; set; }

    [Required]
    [MaxLength(200)]
    public string Titulo { get; set; } = string.Empty;

    [Required]
    public string Descricao { get; set; } = string.Empty;

    public string? Providencias { get; set; }
}
