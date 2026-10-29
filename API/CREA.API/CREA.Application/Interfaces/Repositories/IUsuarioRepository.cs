using CREA.Domain.Entities;
using CREA.Domain.Enums;

namespace CREA.Application.Interfaces.Repositories;

public interface IUsuarioRepository : IRepository<Usuario>
{
    Task<Usuario?> GetByEmailAsync(string email);
    Task<Usuario?> GetByCpfAsync(string cpf);
    Task<IEnumerable<Usuario>> GetByTipoAsync(TipoUsuario tipo);
    Task<bool> EmailExisteAsync(string email);
}
