namespace Asaf.EFCoreAudit.Configurations;

public class AuditContext : IDisposable
{
    private static readonly AsyncLocal<AuditContext?> _current = new AsyncLocal<AuditContext?>();

    public bool AuditDisable { get; set; }

    public static AuditContext? Current
    {
        get { return _current.Value; }
        private set { _current.Value = value; }
    }

    private AuditContext() { }

    public static AuditContext Create()
    {
        Current = new AuditContext();
        return Current;
    }

    public static void NoAudit()
    {
        if (Current == null)
            Create();

        Current.AuditDisable = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        Current = null;
    }
}
