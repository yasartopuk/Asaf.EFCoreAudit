using Asaf.EFCoreAudit.Entities;

namespace Asaf.EFCoreAudit.Providers;

public interface IAuditProvider
{
    void AddAudit(AuditEntry auditEntry);
    void AddAudit(IEnumerable<AuditEntry> auditEntities);

    List<AuditEntry> GetAudits();
    List<AuditEntry> GetAudits(string tableName, int recordId);
    List<AuditEntry> GetAudits(string tableName, int page = 1, int limit = 100);
    List<AuditEntry> GetAudits(Func<AuditEntry, bool> predicate);
    bool ClearAllAudits();
}
