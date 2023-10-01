namespace Asaf.EFCoreAudit.Entities;

public class AuditChange
{
    public int Id { get; set; }
    public int AuditEntryId { get; set; }

    public string ColumnName { get; set; }
    public string? OriginalValue { get; set; }
    public string? OriginalValueName { get; set; }

    public string? NewValue { get; set; }
    public string? NewValueName { get; set; }

    public AuditEntry? AuditEntry { get; set; }
}
