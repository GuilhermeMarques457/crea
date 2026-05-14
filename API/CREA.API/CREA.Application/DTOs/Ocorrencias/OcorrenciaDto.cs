using CREA.Domain.Enums;

namespace CREA.Application.DTOs.Ocorrencias;

public class OcorrenciaDto
{
    public Guid Id { get; set; }
    public Guid ObraId { get; set; }
    public string NomeObra { get; set; } = string.Empty;
    public DateTime DataOcorrencia { get; set; }
    public TipoOcorrencia Tipo { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string? Providencias { get; set; }
    public Guid UsuarioId { get; set; }
    public string NomeUsuario { get; set; } = string.Empty;
    public bool Ativo { get; set; }
    public DateTime CriadoEm { get; set; }
    public int QuantidadeAnexos { get; set; }
}
