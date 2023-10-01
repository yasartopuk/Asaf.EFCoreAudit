namespace Asaf.AuditLog.Web.Data;

public class AppDbMigratorService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    public AppDbMigratorService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _ = AppDbInitializer.SeedAsync(_serviceProvider);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
