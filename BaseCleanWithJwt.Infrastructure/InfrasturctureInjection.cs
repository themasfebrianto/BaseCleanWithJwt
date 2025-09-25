using BaseCleanWithJwt.Application.Interface.InfrastructureInterface;
using BaseCleanWithJwt.Infrastructure.Identity;
using BaseCleanWithJwt.Infrastructure.MongoDb;
using BaseCleanWithJwt.Infrastructure.MongoDb.Repository;
using BaseCleanWithJwt.Infrastructure.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace BaseCleanWithJwt.Infrastructure;

public static class InfrastructureInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IJwtProvider, JwtProvider>();
        services.AddScoped<IImageStorageService, ImageStorageService>();
        services.AddScoped<IMongoDbContext, MongoDbContext>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();

        // Map Id to MongoDB ObjectId
        // MongoMappings.RegisterAll();
        return services;
    }

}