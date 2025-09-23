namespace BaseCleanWithJwt.Infrastructure.MongoDb;

public class MongoDbSettings
{
    public string ConnectionString { get; set; } = "mongodb://localhost:27017/"!;
    public string DatabaseName { get; set; } = "MiniBlog"!;
    public string UserCollection { get; set; } = "Auth"!;
    public string RefreshTokensCollection { get; set; } = "RefreshTokens"!;
    public string RoleCollection { get; set; } = "Roles"!;
}