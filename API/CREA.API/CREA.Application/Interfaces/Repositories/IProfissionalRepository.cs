using CREA.Domain.Entities;

namespace CREA.Application.Interfaces.Repositories;

public interface IProfissionalRepository : IRepository<Profissional>
{
    Task<Profissional?> GetByNumeroRegistroAsync(string numeroRegistro);
    Task<Profissional?> GetByCpfAsync(string cpf);
    Task<IEnumerable<Profissional>> GetByTipoRegistroAsync(string tipoRegistro);
    Task<bool> NumeroRegistroExisteAsync(string numeroRegistro);
    Task<Profissional?> GetByUsuarioIdAsync(Guid usuarioId);
}
