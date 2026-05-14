namespace CREA.Application.DTOs.Profissionais;

public class ProfissionalDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string NumeroRegistro { get; set; } = string.Empty;
    public string TipoRegistro { get; set; } = string.Empty;
    public string? Empresa { get; set; }
    public string Especialidade { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public Guid? UsuarioId { get; set; }
    public bool Ativo { get; set; }
    public DateTime CriadoEm { get; set; }
}
