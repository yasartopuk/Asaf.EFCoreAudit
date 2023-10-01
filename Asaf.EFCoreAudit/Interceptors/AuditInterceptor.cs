using Microsoft.EntityFrameworkCore.Diagnostics;
using Asaf.EFCoreAudit.Providers;
using Asaf.EFCoreAudit.Configurations;

namespace Asaf.EFCoreAudit.Interceptors;

public class AuditInterceptor : SaveChangesInterceptor, IAuditInterceptor
{
    private readonly AuditProcessor _processor;

    public AuditInterceptor(IAuditProvider auditProvider)
    {
        _processor = new AuditProcessor(auditProvider);
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        return HandleSavingChanges(eventData, result);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        return await new ValueTask<InterceptionResult<int>>(HandleSavingChanges(eventData, result));
    }

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        return HandleSavedChanges(eventData, result);
    }

    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        return await new ValueTask<int>(HandleSavedChanges(eventData, result));
    }

    private InterceptionResult<int> HandleSavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (AuditContext.Current?.AuditDisable == true) return result;

        if (eventData?.Context is not null)
        {
            _processor.SetDbContext(eventData.Context);
            _processor.PrepareDeletedEntries();
            _processor.PrepareModifiedEntities();

            if (AuditConfiguration.GlobalSettings.AddedEnabled)
            {
                _processor.PrepareAddedEntries();
            }
        }

        return result;
    }

    private int HandleSavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        if (AuditContext.Current?.AuditDisable == true) return result;

        if (eventData?.Context is not null)
        {
            _processor.ProcessAuditLogs();
        }
        return result;
    }
}

