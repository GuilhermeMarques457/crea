namespace CREA.Domain.Entities;

public class LogAuditoria : BaseEntity
{
    public Guid? UsuarioId { get; set; }
    public string NomeUsuario { get; set; } = string.Empty;
    public string Acao { get; set; } = string.Empty;
    public string Entidade { get; set; } = string.Empty;
    public string? EntidadeId { get; set; }
    public string? DadosAntigos { get; set; }
    public string? DadosNovos { get; set; }
    public string? EnderecoIp { get; set; }
    public DateTime DataAcao { get; set; } = DateTime.UtcNow;
}
