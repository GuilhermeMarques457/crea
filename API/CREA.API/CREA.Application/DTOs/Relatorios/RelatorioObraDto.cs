using CREA.Application.DTOs.Assinaturas;
using CREA.Application.DTOs.RelatoVisita;
using CREA.Domain.Enums;

namespace CREA.Application.DTOs.Relatorios;

public class RelatorioObraDto
{
    public Guid ObraId { get; set; }
    public string NomeObra { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string Proprietario { get; set; } = string.Empty;
    public string? TelefoneProprietario { get; set; }
    public string? Empresa { get; set; }
    public string? NumeroCaderneta { get; set; }
    public string NumeroArt { get; set; } = string.Empty;
    public string? NumeroRT { get; set; }
    public TipoObra TipoObra { get; set; }
    public TipoEdificacao? TipoEdificacao { get; set; }
    public AtividadeTecnica? AtividadeTecnica { get; set; }
    public bool DirecaoTecnica { get; set; }
    public StatusObra Status { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime? DataPrevisaoTermino { get; set; }
    public string NomeProfissionalResponsavel { get; set; } = string.Empty;
    public string NumeroRegistroProfissional { get; set; } = string.Empty;
    public int TotalRelatoVisita { get; set; }
    public int TotalAnexos { get; set; }
    public bool PossuiTermoConclusao { get; set; }
    public DateTime? DataConclusao { get; set; }
    public bool AssinadoPeloProfissional { get; set; }
    public bool AssinadoPeloProprietario { get; set; }
    public bool AssinadoPeloCrea { get; set; }
    public bool TermoConcluido { get; set; }

    public int? TermoNumero { get; set; }
    public string? TermoDescricao { get; set; }
    public string? TermoObservacoes { get; set; }
    public string? TermoDeclaracaoTexto { get; set; }

    public List<AssinaturaDto> AssinaturasObra { get; set; } = [];
    public List<AssinaturaDto> AssinaturasTermo { get; set; } = [];
    public IEnumerable<RelatoVisitaDto> RelatoVisita { get; set; } = [];
    public DateTime GeradoEm { get; set; } = DateTime.UtcNow;
}
