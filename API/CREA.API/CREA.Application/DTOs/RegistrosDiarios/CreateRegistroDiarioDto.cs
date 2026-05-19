using System.ComponentModel.DataAnnotations;
using CREA.Domain.Enums;

namespace CREA.Application.DTOs.RegistrosDiarios;

public class CreateRelatoVisitaDto
{
    [Required]
    public Guid ObraId { get; set; }

    [Required]
    public DateTime Data { get; set; }

    [Required]
    public string Atividades { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string EquipePresente { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? CondicaoClimatica { get; set; }

    public string? Observacoes { get; set; }

    public bool ServicosPreliminar { get; set; }
    public bool Fundacao { get; set; }
    public bool Alvenarias { get; set; }
    public bool Superestrutura { get; set; }
    public bool Cobertura { get; set; }
    public bool EsquadriasInstalacoesEletricasHidraulicas { get; set; }
    public bool RevestimentoForroParePiso { get; set; }
    public bool Pintura { get; set; }
    public bool ServicosComplementares { get; set; }

    public PosicaoObra? PosicaoObra { get; set; }
    public string? DecisoesTecnicas { get; set; }
}
