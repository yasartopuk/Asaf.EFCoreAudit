namespace Asaf.EFCoreAudit.Entities;

public class AuditEntry
{
    public int Id { get; set; }
    public string TableName { get; set; }
    public int RecordId { get; set; }
    public string Action { get; set; }
    public string? UserId { get; set; }
    public string TransactionId { get; set; }
    public DateTime TimeStamp { get; set; }

    public ICollection<AuditChange> AuditChanges { get; set; }

    public AuditEntry()
    {
        AuditChanges = new List<AuditChange>();
    }
}

