using CREA.Application.DTOs.Assinaturas;
using CREA.Domain.Entities;
using CREA.Domain.Enums;

namespace CREA.Application.Interfaces.Repositories;

public interface IAssinaturaRepository : IRepository<Assinatura>
{
    Task<IEnumerable<Assinatura>> GetByEntidadeAsync(TipoEntidadeAssinatura tipoEntidade, Guid entidadeId);
    Task<Assinatura?> GetByEntidadeETipoAssinanteAsync(TipoEntidadeAssinatura tipoEntidade, Guid entidadeId, TipoAssinante tipoAssinante);
    Task<bool> ExisteAssinaturaAsync(TipoEntidadeAssinatura tipoEntidade, Guid entidadeId, TipoAssinante tipoAssinante);
    Task<IEnumerable<PendenteAssinaturaDto>> GetPendentesParaUsuarioAsync(Guid usuarioId, TipoUsuario tipoUsuario);
    Task<IEnumerable<MinhaAssinaturaDto>> GetMinhasAsync(Guid usuarioId);
}
