using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Asaf.EFCoreAudit.Entities;
using System;

namespace Asaf.EFCoreAudit.Providers.EntityFramework;

internal class AuditEntityFrameworkProvider : IAuditProvider
{
    private readonly AuditDbContext _db;
    internal AuditEntityFrameworkProvider(AuditDbContext db)
    {
        _db = db;
    }

    public void AddAudit(AuditEntry auditEntry)
    {
        _db.AuditEntries.Add(auditEntry);
        _db.SaveChanges();
    }

    public void AddAudit(IEnumerable<AuditEntry> auditEntities)
    {
        _db.AuditEntries.AddRange(auditEntities);
        _db.SaveChanges();
    }

    public bool ClearAllAudits()
    {
        var changes = _db.AuditChanges.ToList();
        _db.AuditChanges.RemoveRange(changes);

        var entries = _db.AuditEntries.ToList();
        _db.AuditEntries.RemoveRange(entries);
        return _db.SaveChanges() > 0;
    }

    public List<AuditEntry> GetAudits()
    {
        return _db.AuditEntries.Include(x => x.AuditChanges).ToList();
    }

    public List<AuditEntry> GetAudits(string tableName, int recordId)
    {
        return _db.AuditEntries
            .Include(x => x.AuditChanges)
            .Where(x => x.TableName == tableName && x.RecordId == recordId)
            .ToList();
    }

    public List<AuditEntry> GetAudits(Func<AuditEntry, bool> predicate)
    {
        return _db.AuditEntries.Include(x => x.AuditChanges).Where(predicate).ToList();
    }

    public List<AuditEntry> GetAudits(string tableName, int page = 1, int limit = 100)
    {
        return _db.AuditEntries
            .Where(x => x.TableName == tableName)
            .Include(x => x.AuditChanges)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToList();
    }
}
