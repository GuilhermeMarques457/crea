namespace CREA.Application.DTOs.Auditoria;

public class LogAuditoriaDto
{
    public Guid Id { get; init; }
    public Guid? UsuarioId { get; init; }
    public string NomeUsuario { get; init; } = string.Empty;
    public string Acao { get; init; } = string.Empty;
    public string Entidade { get; init; } = string.Empty;
    public string? EntidadeId { get; init; }
    public string? DadosAntigos { get; init; }
    public string? DadosNovos { get; init; }
    public string? EnderecoIp { get; init; }
    public DateTime DataAcao { get; init; }
}
