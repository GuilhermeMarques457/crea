namespace CREA.Domain.Entities;

public class Proprietario : BaseEntity
{
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;

    public Guid? UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

    public ICollection<Obra> Obras { get; set; } = [];
}
