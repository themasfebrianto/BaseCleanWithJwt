using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BaseCleanWithJwt.Infrastructure.MongoDb;

public interface IMongoDbContext
{
    IMongoCollection<T> GetMongoCollection<T>(string name);
}

public class MongoDbContext : IMongoDbContext
{
    private readonly IMongoDatabase _database;
    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        var mongoClient = new MongoClient(settings.Value.ConnectionString);
        _database = mongoClient.GetDatabase(settings.Value.DatabaseName);
    }
    public IMongoCollection<T> GetMongoCollection<T>(string name) => _database.GetCollection<T>(name);

}