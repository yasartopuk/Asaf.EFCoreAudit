namespace Asaf.EFCoreAudit.Entities;

public interface IAuditable<T> : IAuditable
{
    public new T Id { get; set; }
}

public interface IAuditable
{
    public int Id { get; set; }
}

public interface INameable
{
    public string Name { get; set; }
}
