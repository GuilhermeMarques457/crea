using System.ComponentModel.DataAnnotations;
using CREA.Domain.Enums;

namespace CREA.Application.DTOs.Obras;

public class CreateObraDto
{
    [Required]
    [MaxLength(300)]
    public string LocalObra { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Logradouro { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Quadra { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Lote { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int Numero { get; set; } = 0;

    [Required]
    public Guid ProprietarioId { get; set; }

    [MaxLength(200)]
    public string? Empresa { get; set; }

    [MaxLength(20)]
    public string? NumeroCaderneta { get; set; }

    [MaxLength(50)]
    public string? NumeroRT { get; set; }

    public TipoEdificacao? TipoEdificacao { get; set; }

    public AtividadeTecnica? AtividadeTecnica { get; set; }


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
