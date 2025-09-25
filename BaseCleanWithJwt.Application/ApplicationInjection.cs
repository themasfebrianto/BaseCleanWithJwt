using BaseCleanWithJwt.Application.Interface.ApplicationInterface;
using BaseCleanWithJwt.Application.Interface.InfrastructureInterface;
using BaseCleanWithJwt.Application.Service;
using Microsoft.Extensions.DependencyInjection;

namespace BaseCleanWithJwt.Application;

public static class ApplicationInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        return services;
    }

}