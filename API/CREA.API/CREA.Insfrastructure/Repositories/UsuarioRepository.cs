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

    public async Task<Usuario?> GetByCpfAsync(string cpf)
    {
        var digits = new string(cpf.Where(char.IsDigit).ToArray());
        var formatted = digits.Length == 11
            ? $"{digits[..3]}.{digits[3..6]}.{digits[6..9]}-{digits[9..]}"
            : cpf;
        return await _dbSet.FirstOrDefaultAsync(u =>
            (u.Cpf == cpf || u.Cpf == digits || u.Cpf == formatted) && u.Ativo);
    }

    public async Task<IEnumerable<Usuario>> GetByTipoAsync(TipoUsuario tipo) =>
        await _dbSet.Where(u => u.TipoUsuario == tipo && u.Ativo).ToListAsync();

    public async Task<bool> EmailExisteAsync(string email) =>
        await _dbSet.AnyAsync(u => u.Email == email);
}
