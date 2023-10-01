using Asaf.EFCoreAudit.Entities;
using Asaf.EFCoreAudit.Providers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq.Dynamic.Core;

namespace Asaf.EFCoreAudit.Interceptors;

internal class AuditProcessor : IDisposable
{
    private readonly List<AuditEntry> _auditEntities = new();
    private readonly Dictionary<EntityState, List<EntityEntry>> _entries = new();

    private readonly IAuditProvider _provider;
    private DbContext? _dbContext;
    private bool disposed = false;

    internal AuditProcessor(IAuditProvider auditLogProvider) => _provider = auditLogProvider;
    internal void SetDbContext(DbContext dbContext) => _dbContext = dbContext;

    internal void PrepareAddedEntries()
    {
        var addedEntries = _dbContext?.ChangeTracker.Entries().Where(e => e.State is EntityState.Added && e.Entity is IAuditable).ToList();
        if (addedEntries != null && addedEntries.Any())
        {
            _entries[EntityState.Added] = addedEntries;
        }
    }

    internal void PrepareDeletedEntries()
    {
        var deletedEntries = _dbContext?.ChangeTracker.Entries().Where(e => e.State is EntityState.Deleted && e.Entity is IAuditable).ToList();
        if (deletedEntries != null && deletedEntries.Any())
        {
            _entries[EntityState.Deleted] = deletedEntries;
        }
    }

    private void PrepareAddedAndDeletedEntities()
    {
        foreach (var kv in _entries)
        {
            foreach (var entry in kv.Value)
            {
                var auditEntity = CreateAuditLogForEntity(entry, kv.Key);
                if (auditEntity.AuditChanges.Any())
                    _auditEntities.Add(auditEntity);
            }
        }
    }

    internal void PrepareModifiedEntities()
    {
        var modifiedEntries = _dbContext?.ChangeTracker.Entries().Where(e => e.State is EntityState.Modified);

        if (modifiedEntries != null && modifiedEntries.Any())
        {
            var auditableEntries = modifiedEntries.Where(x => x.Entity is IAuditable).ToList();
            foreach (var item in auditableEntries)
            {
                var entity = CreateAuditLogForEntity(item, item.State);
                _auditEntities.Add(entity);
            }
        }
    }

    internal string? GetNameFromRelatedEntity(IProperty property, object? value)
    {
        if (_dbContext == null || value == null || !property.IsForeignKey()) return null;

        var foreignEntity = property.GetContainingForeignKeys().First();
        var dbSetProperty = Array.Find(_dbContext.GetType().GetProperties(),
                                        x => x.PropertyType.GenericTypeArguments.Contains(foreignEntity.PrincipalEntityType.ClrType));

        if (dbSetProperty?.GetValue(_dbContext) is IQueryable<INameable> query)
        {
            var type = property?.PropertyInfo?.PropertyType ?? typeof(int);
            var columnName = foreignEntity.PrincipalKey.Properties.FirstOrDefault()?.Name;
            var convertedValue = Convert.ChangeType(value, type);

            if (!string.IsNullOrEmpty(columnName))
            {
                var entity = query.Where($"{columnName} == @0", convertedValue).FirstOrDefault();
                return entity?.Name;
            }
        }

        return null;
    }

    internal AuditEntry CreateAuditLogForEntity(EntityEntry entry, EntityState state)
    {
        var auditEntry = new AuditEntry
        {
            TableName = entry.Metadata.GetTableName(),
            RecordId = (entry.Entity as IAuditable).Id,
            Action = state.ToString(),
            TimeStamp = DateTime.UtcNow,
        };

        foreach (var property in entry.CurrentValues.Properties)
        {
            var original = entry.OriginalValues[property];
            var current = entry.CurrentValues[property];

            if (original?.ToString() == current?.ToString() && state is EntityState.Modified)
                continue;

            var auditChange = new AuditChange();
            auditChange.ColumnName = property.Name;

            if (state is not EntityState.Deleted)
            {
                auditChange.NewValue = current?.ToString();
                auditChange.NewValueName = GetNameFromRelatedEntity(property, auditChange.NewValue);
            }

            if (state is not EntityState.Added)
            {
                auditChange.OriginalValue = original?.ToString();
                auditChange.OriginalValueName = GetNameFromRelatedEntity(property, auditChange.OriginalValue);
            }

            auditEntry.AuditChanges.Add(auditChange);
        }

        return auditEntry;
    }

    internal void ProcessAuditLogs()
    {
        try
        {
            PrepareAddedAndDeletedEntities();

            if (!_auditEntities.Any())
                return;

            var transactionId = Guid.NewGuid();
            foreach (var entity in _auditEntities)
            {
                entity.TransactionId = transactionId.ToString();
#if DEBUG
                Console.WriteLine($"{entity.Action}:{entity.TableName}:{entity.RecordId}");
#endif
            }
            _provider.AddAudit(_auditEntities);
        }
        catch (Exception ex)
        {
            throw;
        }
        finally
        {
            _auditEntities.Clear();
            _entries.Clear();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                _auditEntities.Clear();
            }

            disposed = true;
        }
    }
}
