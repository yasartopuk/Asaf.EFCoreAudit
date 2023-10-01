using Microsoft.EntityFrameworkCore;
using Asaf.EFCoreAudit.Providers;
using Asaf.EFCoreAudit.Providers.EntityFramework;

namespace Asaf.EFCoreAudit.Configurations;

public class AuditSettings
{
    public AuditLogProvider Provider { get; private set; }
    public string? Connection { get; private set; }
    public bool AddedEnabled { get; set; }
    internal DbContextOptions<AuditDbContext>? DbContextOptions { get; private set; }

    public void AddEntityFrameworkProvider(Action<DbContextOptionsBuilder<AuditDbContext>> builderAction)
    {
        Provider = AuditLogProvider.EntityFramework;
        var builder = new DbContextOptionsBuilder<AuditDbContext>();
        builderAction(builder);
        DbContextOptions = builder.Options;
    }

    public void AddMongoDbProvider(string connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
            throw new ArgumentNullException(nameof(connectionString));

        Provider = AuditLogProvider.MongoDb;
        Connection = connectionString;
    }
}
