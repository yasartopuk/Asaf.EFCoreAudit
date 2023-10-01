using Asaf.EFCoreAudit.Configurations;
using Asaf.EFCoreAudit.Entities;
using Asaf.EFCoreAudit.Providers;
using Asaf.EFCoreAudit.Providers.EntityFramework;
using Asaf.EFCoreAudit.Providers.MongoDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Asaf.EFCoreAudit.DataAccess;

public class MigratorHostedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    public MigratorHostedService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (AuditConfiguration.GlobalSettings?.Provider == AuditLogProvider.EntityFramework) 
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AuditDbContext>();

            await context.Database.EnsureCreatedAsync();
            await context.Database.MigrateAsync();
        }
        else if (AuditConfiguration.GlobalSettings?.Provider == AuditLogProvider.MongoDb)
        {
            BsonClassMapper.Instance.Register<AuditEntry>(cm =>
            {
                cm.AutoMap();
                cm.UnmapMember(m => m.Id);
                cm.SetIgnoreExtraElements(true);
            });

            BsonClassMapper.Instance.Register<AuditChange>(cm =>
            {
                cm.AutoMap();
                cm.UnmapMember(m => m.Id);
                cm.SetIgnoreExtraElements(true);
            });
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
