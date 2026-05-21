using CREA.Application.DTOs.Assinaturas;

namespace CREA.Application.DTOs.TermosConclusao;

public class TermoConclusaoDto
{
    public Guid Id { get; set; }
    public Guid ObraId { get; set; }
    public string NomeObra { get; set; } = string.Empty;
    public int NumeroTermo { get; set; }
    public DateTime DataConclusao { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string? Observacoes { get; set; }
    public string? DeclaracaoTexto { get; set; }
    public string? LocalDeclaracao { get; set; }
    public DateTime? DataDeclaracao { get; set; }
    public Guid ProfissionalId { get; set; }
    public string NomeProfissional { get; set; } = string.Empty;
    public string NumeroRegistro { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; }
    public bool AssinadoPeloProfissional { get; set; }
    public bool AssinadoPeloProprietario { get; set; }
    public bool Concluido { get; set; }
    public List<AssinaturaDto> Assinaturas { get; set; } = [];
}
