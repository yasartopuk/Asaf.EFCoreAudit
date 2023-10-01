using MongoDB.Bson.Serialization;

namespace Asaf.EFCoreAudit.Providers.MongoDB;

public sealed class BsonClassMapper
{

    private static BsonClassMapper instance = null;

    private static readonly object _lock = new object();

    public static BsonClassMapper Instance
    {
        get
        {
            instance ??= new BsonClassMapper();
            return instance;
        }
    }

    public BsonClassMapper Register<T>(Action<BsonClassMap<T>> classMapInitializer)
    {
        lock (_lock)
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(T)))
            {
                BsonClassMap.RegisterClassMap<T>(classMapInitializer);
            }
        }
        return this;
    }
}
