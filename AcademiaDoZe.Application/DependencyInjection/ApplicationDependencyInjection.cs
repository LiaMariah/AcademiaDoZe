
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Application.Services;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Infrastructure.Repositories;
using AcademiaDoZe.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AcademiaDoZe.Application.DependencyInjection;

public static class ApplicationDependencyInjection
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services)
    {
        // Registra os serviços da camada de aplicação
        services.AddTransient<ILogradouroService, LogradouroService>();
        services.AddTransient<IColaboradorService, ColaboradorService>();
        services.AddTransient<IAlunoService, AlunoService>();
        services.AddTransient<IMatriculaService, MatriculaService>();
        services.AddTransient<IControleAcessoService, ControleAcessoService>();

        // AddScoped: cria uma instância por escopo.
        // AddSingleton: cria uma única instância durante a aplicação.
        // AddTransient: cria uma nova instância a cada resolução.

        // Fábrica de LogradouroRepository
        services.AddTransient(provider =>
        {
            var config = provider.GetRequiredService<RepositoryConfig>();

            return (Func<ILogradouroRepository>)(() =>
                new LogradouroRepository(
                    config.ConnectionString,
                    config.DatabaseType));
        });

        // Fábrica de ColaboradorRepository
        services.AddTransient(provider =>
        {
            var config = provider.GetRequiredService<RepositoryConfig>();

            return (Func<IColaboradorRepository>)(() =>
                new ColaboradorRepository(
                    config.ConnectionString,
                    config.DatabaseType));
        });

        // Fábrica de AlunoRepository
        services.AddTransient(provider =>
        {
            var config = provider.GetRequiredService<RepositoryConfig>();

            return (Func<IAlunoRepository>)(() =>
                new AlunoRepository(
                    config.ConnectionString,
                    config.DatabaseType));
        });

        // Fábrica de MatriculaRepository
        services.AddTransient(provider =>
        {
            var config = provider.GetRequiredService<RepositoryConfig>();

            return (Func<IMatriculaRepository>)(() =>
                new MatriculaRepository(
                    config.ConnectionString,
                    config.DatabaseType));
        });

        // Repositório de Controle de Acesso
        services.AddTransient<IRegistroAcessoRepository>(provider =>
        {
            var config = provider.GetRequiredService<RepositoryConfig>();

            return new RegistroAcessoRepository(
                config.ConnectionString,
                config.DatabaseType);
        });

        return services;
    }
}
