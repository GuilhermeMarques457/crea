using CREA.Application.DTOs.Common;
using CREA.Domain.Entities;

namespace CREA.Application.Interfaces.Repositories;

public interface ILogAuditoriaRepository : IRepository<LogAuditoria>
{
    Task<PagedResult<LogAuditoria>> GetPagedAsync(int page, int pageSize, string? entidade = null, string? acao = null, Guid? usuarioId = null, DateTime? inicio = null, DateTime? fim = null);
    Task<IEnumerable<LogAuditoria>> GetByUsuarioAsync(Guid usuarioId);
    Task<IEnumerable<LogAuditoria>> GetByEntidadeAsync(string entidade, string entidadeId);
    Task<IEnumerable<LogAuditoria>> GetByPeriodoAsync(DateTime inicio, DateTime fim);
}
