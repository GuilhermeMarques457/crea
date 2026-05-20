using CREA.Domain.Entities;
using CREA.Domain.Enums;

namespace CREA.Application.Interfaces.Repositories;

public interface IObraRepository : IRepository<Obra>
{
    Task<Obra?> GetByIdWithDetailsAsync(Guid id);
    Task<IEnumerable<Obra>> ListWithDetailsAsync();
    Task<IEnumerable<Obra>> GetByProfissionalAsync(Guid profissionalId);
    Task<IEnumerable<Obra>> GetByStatusAsync(StatusObra status);
    Task<IEnumerable<Obra>> GetByUsuarioCriadorAsync(Guid usuarioId);
    Task<bool> ExisteObraAtivaComProprietarioAsync(Guid proprietarioId);
    Task<bool> ExisteObraAtivaComProfissionalAsync(Guid profissionalId);
}
