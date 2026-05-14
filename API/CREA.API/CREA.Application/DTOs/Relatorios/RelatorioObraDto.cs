using CREA.Application.DTOs.Ocorrencias;
using CREA.Application.DTOs.RegistrosDiarios;
using CREA.Application.DTOs.TermosConclusao;
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
    public int TotalRegistrosDiarios { get; set; }
    public int TotalOcorrencias { get; set; }
    public int TotalAnexos { get; set; }
    public bool PossuiTermoConclusao { get; set; }
    public DateTime? DataConclusao { get; set; }
    public bool AssinadoPeloResponsavel { get; set; }
    public bool AssinadoPeloAdmin { get; set; }
    public bool TermoConcluido { get; set; }
    public List<AssinaturaTermoConclusaoDto> Assinaturas { get; set; } = [];
    public IEnumerable<RegistroDiarioDto> RegistrosDiarios { get; set; } = [];
    public IEnumerable<OcorrenciaDto> Ocorrencias { get; set; } = [];
    public DateTime GeradoEm { get; set; } = DateTime.UtcNow;
}
