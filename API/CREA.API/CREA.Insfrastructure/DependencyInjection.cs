using CREA.Application.Interfaces.Repositories;
using CREA.Insfrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace CREA.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IProfissionalRepository, ProfissionalRepository>();
        services.AddScoped<IProprietarioRepository, ProprietarioRepository>();
        services.AddScoped<IObraRepository, ObraRepository>();
        services.AddScoped<IRelatoVisitaRepository, RelatoVisitaRepository>();
        services.AddScoped<IAnexoRepository, AnexoRepository>();
        services.AddScoped<ITermoConclusaoRepository, TermoConclusaoRepository>();
        services.AddScoped<IAssinaturaRepository, AssinaturaRepository>();
        services.AddScoped<ILogAuditoriaRepository, LogAuditoriaRepository>();

        return services;
    }
}
