namespace CREA.Application.DTOs.Anexos;

public class AnexoDto
{
    public Guid Id { get; set; }
    public string NomeArquivoOriginal { get; set; } = string.Empty;
    public string NomeArquivo { get; set; } = string.Empty;
    public string TipoArquivo { get; set; } = string.Empty;
    public long TamanhoBytes { get; set; }
    public Guid? ObraId { get; set; }
    public Guid? RegistroDiarioId { get; set; }
    public Guid? OcorrenciaId { get; set; }
    public Guid UsuarioId { get; set; }
    public string NomeUsuario { get; set; } = string.Empty;
    public string UrlDownload { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; }
}
