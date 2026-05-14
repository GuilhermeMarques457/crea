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
        services.AddScoped<IRegistroDiarioRepository, RegistroDiarioRepository>();
        services.AddScoped<IOcorrenciaRepository, OcorrenciaRepository>();
        services.AddScoped<IAnexoRepository, AnexoRepository>();
        services.AddScoped<ITermoConclusaoRepository, TermoConclusaoRepository>();
        services.AddScoped<IAssinaturaTermoConclusaoRepository, AssinaturaTermoConclusaoRepository>();
        services.AddScoped<ILogAuditoriaRepository, LogAuditoriaRepository>();

        return services;
    }
}
