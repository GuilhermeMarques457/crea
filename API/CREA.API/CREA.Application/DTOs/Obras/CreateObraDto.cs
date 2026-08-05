using System.ComponentModel.DataAnnotations;
using CREA.Domain.Enums;

namespace CREA.Application.DTOs.Obras;

public class CreateObraDto
{
    [Required]
    [MaxLength(300)]
    public string LocalObra { get; set; } = string.Empty;

    [Required]
    public Guid ProprietarioId { get; set; }

    [MaxLength(200)]
    public string? Empresa { get; set; }

    [MaxLength(20)]
    public string? NumeroCaderneta { get; set; }

    [Required]
    [MaxLength(50)]
    public string NumeroArt { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? NumeroRT { get; set; }

    public TipoEdificacao? TipoEdificacao { get; set; }

    public AtividadeTecnica? AtividadeTecnica { get; set; }

    public bool DirecaoTecnica { get; set; }

    [Required]
    public DateTime DataInicio { get; set; }

    public decimal? AreaConstruir { get; set; }
    public decimal? AreaRegularizar { get; set; }
    public decimal? AreaAmpliar { get; set; }
    public decimal? AreaReformar { get; set; }
    public decimal? AreaTotalEdificada { get; set; }
    public decimal? ValorRecibo { get; set; }

    [Required]
    public Guid ProfissionalId { get; set; }
}
