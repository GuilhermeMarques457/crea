using CREA.Application.Interfaces.Repositories;
using CREA.Domain.Entities;
using CREA.Domain.Enums;
using CREA.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CREA.Insfrastructure.Repositories;

public class UsuarioRepository(ApplicationDbContext context) : Repository<Usuario>(context), IUsuarioRepository
{
    public async Task<Usuario?> GetByEmailAsync(string email) =>
        await _dbSet.FirstOrDefaultAsync(u => u.Email == email && u.Ativo);

    public async Task<IEnumerable<Usuario>> GetByTipoAsync(TipoUsuario tipo) =>
        await _dbSet.Where(u => u.TipoUsuario == tipo && u.Ativo).ToListAsync();

    public async Task<bool> EmailExisteAsync(string email) =>
        await _dbSet.AnyAsync(u => u.Email == email);
}
