using Microsoft.EntityFrameworkCore;
using Asaf.EFCoreAudit.Entities;

namespace Asaf.EFCoreAudit.Providers.EntityFramework;

public class AuditDbContext : DbContext
{
    public DbSet<AuditEntry> AuditEntries { get; set; }
    public DbSet<AuditChange> AuditChanges { get; set; }

    public AuditDbContext(DbContextOptions<AuditDbContext> options) : base(options)
    {

    }
}
