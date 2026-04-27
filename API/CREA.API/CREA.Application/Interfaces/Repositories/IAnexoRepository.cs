using CREA.Domain.Entities;

namespace CREA.Application.Interfaces.Repositories;

public interface IAnexoRepository : IRepository<Anexo>
{
    Task<IEnumerable<Anexo>> GetByObraAsync(Guid obraId);
    Task<IEnumerable<Anexo>> GetByRegistroDiarioAsync(Guid registroDiarioId);
    Task<IEnumerable<Anexo>> GetByOcorrenciaAsync(Guid ocorrenciaId);
}
