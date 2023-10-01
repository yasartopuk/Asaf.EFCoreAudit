using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Asaf.EFCoreAudit.DataAccess;
using Asaf.EFCoreAudit.Interceptors;
using Asaf.EFCoreAudit.Providers;
using Asaf.EFCoreAudit.Providers.EntityFramework;
using Asaf.EFCoreAudit.Providers.MongoDB;

namespace Asaf.EFCoreAudit.Configurations;

public static class AuditConfiguration
{
    public static AuditSettings GlobalSettings { get; set; } = new();

    public static IServiceCollection AddAudit(this IServiceCollection services, Action<AuditSettings> settingsAction)
    {
        var settings = new AuditSettings();
        if (settingsAction != null)
            settingsAction(settings);

        if (settings.Provider == AuditLogProvider.EntityFramework && settings.DbContextOptions == null)
            throw new ArgumentNullException(nameof(settings.DbContextOptions));

        if (settings.Provider == AuditLogProvider.MongoDb && settings.Connection == null)
            throw new ArgumentNullException(nameof(settings.Connection));

        services.AddHostedService<MigratorHostedService>();
        services.AddTransient<IAuditInterceptor, AuditInterceptor>();
        services.AddTransient(s=> new AuditDbContext(settings.DbContextOptions));
        services.AddTransient<IAuditProvider>(sp =>
        {
            if (settings.Provider == AuditLogProvider.MongoDb)
            {
                return new AuditMongoDbProvider(settings.Connection);
            }
            else
            {
                var context = sp.GetRequiredService<AuditDbContext>();
                return new AuditEntityFrameworkProvider(context); 
            }
        });

        GlobalSettings = settings;
        return services;
    }

    public static IApplicationBuilder UseAudit(this IApplicationBuilder builder)
    {
        builder.Use(async (context, next) =>
        {
            using (var auditContext = AuditContext.Create())
            {
                await next();
            }
        });
        return builder;
    }
}
