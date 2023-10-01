using MongoDB.Driver;
using Asaf.EFCoreAudit.Entities;

namespace Asaf.EFCoreAudit.Providers.MongoDB;

internal partial class AuditMongoDbProvider : IAuditProvider
{
    private const string databaseName = "AuditLog";
    private const string collectionName = "AuditEntries";
    private readonly IMongoCollection<AuditEntry> _auditCollection;

    public AuditMongoDbProvider(string connectionString)
    {
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        _auditCollection = database.GetCollection<AuditEntry>(collectionName);
    }

    public void AddAudit(AuditEntry auditEntry)
    {
        _auditCollection.InsertOne(auditEntry);
    }

    public void AddAudit(IEnumerable<AuditEntry> auditEntities)
    {
        _auditCollection.InsertMany(auditEntities);
    }

    public bool ClearAllAudits()
    {
        _auditCollection.DeleteManyAsync(FilterDefinition<AuditEntry>.Empty);
        return true;
    }

    public List<AuditEntry> GetAudits()
    {
        return _auditCollection
            .Find(FilterDefinition<AuditEntry>.Empty)
            .ToList();
    }

    public List<AuditEntry> GetAudits(string tableName, int recordId)
    {
        return _auditCollection
            .Find(x => x.TableName == tableName && x.RecordId == recordId)
            .ToList();
    }

    public List<AuditEntry> GetAudits(Func<AuditEntry, bool> predicate)
    {
        return _auditCollection
            .Find(Builders<AuditEntry>.Filter.Where(x => predicate(x)))
            .ToList();
    }

    public List<AuditEntry> GetAudits(string tableName, int page = 1, int limit = 100)
    {
        return _auditCollection
            .Find(x => x.TableName == tableName)
            .Skip((page - 1) * limit)
            .Limit(limit)
            .ToList();
    }
}
