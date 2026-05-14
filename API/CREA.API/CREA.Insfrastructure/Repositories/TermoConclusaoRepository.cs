using CREA.Application.Interfaces.Repositories;
using CREA.Domain.Entities;
using CREA.Domain.Enums;
using CREA.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CREA.Insfrastructure.Repositories;

public class TermoConclusaoRepository(ApplicationDbContext context) : Repository<TermoConclusao>(context), ITermoConclusaoRepository
{
    public async Task<TermoConclusao?> GetByObraAsync(Guid obraId) =>
        await _dbSet
            .Include(t => t.Profissional)
            .Include(t => t.Obra)
            .Include(t => t.Assinaturas).ThenInclude(a => a.Usuario)
            .FirstOrDefaultAsync(t => t.ObraId == obraId && t.Ativo);

    public async Task<bool> ObraPossuiTermoAsync(Guid obraId) =>
        await _dbSet.AnyAsync(t => t.ObraId == obraId && t.Ativo);

    public async Task<IEnumerable<TermoConclusao>> GetPendentesAsync(Guid usuarioId, TipoUsuario tipo) =>
        await _dbSet
            .Include(t => t.Profissional).ThenInclude(a => a.Usuario)
            .Include(t => t.Obra)
            .Include(t => t.Assinaturas).ThenInclude(a => a.Usuario)
            .Where(t => (t.Ativo && (!t.AssinadoPeloResponsavel || !t.AssinadoPeloAdmin)) && (t.Profissional.UsuarioId == usuarioId || tipo == TipoUsuario.Administrador))
            .OrderByDescending(t => t.CriadoEm)
            .ToListAsync();

    public async Task<IEnumerable<TermoConclusao>> GetPendentesNaoAssinadosAsync(Guid usuarioId, TipoUsuario tipo) =>
        await _dbSet
            .Include(t => t.Profissional)
            .Include(t => t.Obra)
            .Include(t => t.Assinaturas).ThenInclude(a => a.Usuario)
            .Where(t => !t.Assinaturas.Any(a => a.UsuarioId == usuarioId) && (!t.AssinadoPeloResponsavel || !t.AssinadoPeloAdmin) && (t.Profissional.UsuarioId == usuarioId || tipo == TipoUsuario.Administrador))
            .OrderByDescending(t => t.CriadoEm)
            .ToListAsync();
}
