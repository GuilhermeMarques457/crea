namespace CREA.Application.DTOs.Proprietarios;

public class ProprietarioDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public Guid? UsuarioId { get; set; }
    public bool Ativo { get; set; }
    public DateTime CriadoEm { get; set; }
}
