using CREA.Application.Interfaces.Repositories;
using CREA.Domain.Entities;
using CREA.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CREA.Insfrastructure.Repositories;

public class ProprietarioRepository(ApplicationDbContext context) : Repository<Proprietario>(context), IProprietarioRepository
{
    public async Task<Proprietario?> GetByUsuarioIdAsync(Guid usuarioId) =>
        await _dbSet.FirstOrDefaultAsync(p => p.UsuarioId == usuarioId && p.Ativo);
}
