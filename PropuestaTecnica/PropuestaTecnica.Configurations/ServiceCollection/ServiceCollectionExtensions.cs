using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PropuestaTecnica.Bussiness.Services.Auth;
using PropuestaTecnica.Bussiness.Services.Contracts;
using PropuestaTecnica.Bussiness.Services.Implementations;
using PropuestaTecnica.Common.Configurations;
using PropuestaTecnica.Common.IOptionPattern;
using PropuestaTecnica.Common.Mappers;
using PropuestaTecnica.DataAccess.Data;
using PropuestaTecnica.DataAccess.Repositories.Contracts;
using PropuestaTecnica.DataAccess.Repositories.Implementations;
using PropuestaTecnica.DataAccess.UnitOfWork;
using PropuestaTecnica.DataAccess.Utilities.ADO;

namespace PropuestaTecnica.Configurations.ServiceCollection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registra el servicio genérico de opciones para cualquier T.
    /// </summary>
    public static IServiceCollection AddPropuestaTecnicaSDOptionsCore(this IServiceCollection services)
    {
        // Habilita soporte para IOptions
        services.AddOptions();

        // Registrar DbContext usando IGenericOptionsService
        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            var connOptions = sp
                .GetRequiredService<IGenericOptionsService<DbOptions>>()
                .GetSnapshotOptions();

            options.UseSqlite(connOptions.DefaultConnection);
        });

        // Registra mapeos de Mapster
        MapsterConfig.RegisterMappings();

        //services.AddHttpClient<IHttpService, HttpService>();

        services.AddScoped(typeof(IGenericOptionsService<>), typeof(GenericOptionsService<>));

        services.AddScoped<IIncidentRepository, IncidentRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();

        services.AddScoped<IIncidentService, IncidentService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<ITokenService, TokenService>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();
        services.AddScoped<ISqlExecutor, SqlExecutor>();

        return services;
    }

    /// <summary>
    /// Vincula una clase de configuración TOptions a una sección del IConfiguration.
    /// Incluye validaciones opcionales con DataAnnotations y chequeo en inicio.
    /// </summary>
    public static OptionsBuilder<TOptions> BindOptions<TOptions>(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName,
        bool validateDataAnnotations = true,
        bool validateOnStart = true)
        where TOptions : class, new()
    {
        var builder = services
            .AddOptions<TOptions>()
            .Bind(configuration.GetSection(sectionName));

        if (validateDataAnnotations)
            builder.ValidateDataAnnotations();

        if (validateOnStart)
            builder.ValidateOnStart();

        return builder;
    }
}
