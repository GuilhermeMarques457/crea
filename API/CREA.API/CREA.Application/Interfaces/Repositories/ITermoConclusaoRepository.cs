using CREA.Domain.Entities;
using CREA.Domain.Enums;

namespace CREA.Application.Interfaces.Repositories;

public interface ITermoConclusaoRepository : IRepository<TermoConclusao>
{
    Task<TermoConclusao?> GetByObraAsync(Guid obraId);
    Task<bool> ObraPossuiTermoAsync(Guid obraId);
    Task<IEnumerable<TermoConclusao>> GetPendentesAsync(Guid usuarioId, TipoUsuario tipo);
    Task<IEnumerable<TermoConclusao>> GetPendentesNaoAssinadosAsync(Guid usuarioId, TipoUsuario tipo);
}
