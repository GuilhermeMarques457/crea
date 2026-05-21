using System.ComponentModel.DataAnnotations;

namespace CREA.Application.DTOs.TermosConclusao;

public class CreateTermoConclusaoDto
{
    [Required]
    public Guid ObraId { get; set; }

    public int NumeroTermo { get; set; }

    [Required]
    public DateTime DataConclusao { get; set; }

    [Required]
    public string Descricao { get; set; } = string.Empty;

    public string? Observacoes { get; set; }

    public string? DeclaracaoTexto { get; set; }
    public string? LocalDeclaracao { get; set; }
    public DateTime? DataDeclaracao { get; set; }
}
