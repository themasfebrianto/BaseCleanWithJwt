using DotNetEnv;

namespace BaseCleanWithJwt.Api.Extension;

public static class CorsExtensions
{
    public const string AllowOrigins = "AllowOrigins";
    public static WebApplicationBuilder CreateCors(this WebApplicationBuilder builder)
    {
        var origins = Env.GetString("MiniBlog__AppSettings__AllowOrigins")!.Split(';');

        builder.Services.AddCors(options =>
        {
            if (origins.Contains("*"))
            {
                options.AddPolicy(AllowOrigins, builder => builder
                    .SetIsOriginAllowed(origin => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
                return;
            }

            options.AddPolicy(AllowOrigins, builder => builder
                .AllowAnyMethod().AllowAnyHeader().WithOrigins(origins).AllowCredentials());
        });
        return builder;
    }
    public static WebApplication ConfigureCors(this WebApplication app)
    {
        app.UseCors(AllowOrigins);
        return app;
    }
}
