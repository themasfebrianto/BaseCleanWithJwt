using BaseCleanWithJwt.Domain.Common.Settings;
using BaseCleanWithJwt.Infrastructure.MongoDb;
using DotNetEnv;

namespace BaseCleanWithJwt.Api.Extension;

public static class SettingExtensions
{
    public static IHostApplicationBuilder AddSettings(this IHostApplicationBuilder builder)
    {
        Env.Load();
        builder.Configuration.AddEnvironmentVariables(prefix: "MiniBlog__");

        builder.Services.AddSettings(builder.Configuration);

        return builder;
    }
    public static IServiceCollection AddSettings(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<AppSettings>(config.ValidateAndGetSection<AppSettings>());
        services.Configure<JwtSettings>(config.ValidateAndGetSection<JwtSettings>());
        services.Configure<MongoDbSettings>(config.ValidateAndGetSection<MongoDbSettings>());

        return services;
    }
    private static IConfigurationSection ValidateAndGetSection<T>(this IConfiguration config) where T : class, new()
    {
        var t = new T();
        var name = typeof(T).Name;
        var section = config.GetSection(name);
        section.Bind(t);

        var nullProps = new List<string>();
        foreach (var prop in typeof(T).GetProperties())
            if (prop.GetValue(t) == null) nullProps.Add(prop.Name);
        if (nullProps.Any()) throw new Exception($"Some property in {name} are null: ({string.Join(", ", nullProps)})");

        return section;
    }
}
