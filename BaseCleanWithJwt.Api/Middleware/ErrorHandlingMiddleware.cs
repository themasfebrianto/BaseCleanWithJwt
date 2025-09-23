
using BaseCleanWithJwt.Api.Common.Response;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BaseCleanWithJwt.Api.Middleware;

public static class ErrorHandlingExtensions
{
    /// <summary>
    /// Registrasi service dan override ModelState agar return CustomResponse.
    /// </summary>
    public static IServiceCollection AddCustomResponse(this IServiceCollection services)
    {
        services.AddControllers()
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .SelectMany(kv => kv.Value!.Errors.Select(e =>
                            string.IsNullOrWhiteSpace(kv.Key)
                                ? e.ErrorMessage
                                : $"{kv.Key}: {e.ErrorMessage}"
                        ));

                    return CustomResponse.StatusCode(
                        StatusCodes.Status400BadRequest,
                        "Validation failed",
                        errors
                    );
                };
            });

        return services;
    }

    /// <summary>
    /// Middleware global untuk JSON parse error & unhandled exception.
    /// </summary>
    public static IApplicationBuilder UseCustomResponse(this IApplicationBuilder app, IHostEnvironment env)
    {
        // JSON malformed
        app.Use(async (context, next) =>
        {
            try
            {
                await next();
            }
            catch (BadHttpRequestException ex)
            {
                // Selalu aman, JSON error bukan internal leak
                var resp = CustomResponse.StatusCode(
                    StatusCodes.Status400BadRequest,
                    env.IsDevelopment() ? $"Invalid JSON: {ex.Message}" : "Invalid request body.",
                    null,
                    context.TraceIdentifier
                );
                await resp.ExecuteResultAsync(new ActionContext { HttpContext = context });
            }
        });

        // Unhandled exception
        app.UseExceptionHandler(cfg =>
        {
            cfg.Run(async context =>
            {
                var feature = context.Features.Get<IExceptionHandlerPathFeature>();
                var ex = feature?.Error;

                if (env.IsDevelopment())
                {
                    // Dev: tampilkan pesan asli
                    await CustomResponse.Exception(ex ?? new Exception("Unknown error"))
                        .ExecuteResultAsync(new ActionContext { HttpContext = context });
                }
                else
                {
                    // Prod: hanya pesan generik, tapi log detailnya
                    Serilog.Log.Error(ex, "Unhandled exception (traceId: {TraceId})", context.TraceIdentifier);

                    await CustomResponse.StatusCode(
                        StatusCodes.Status500InternalServerError,
                        "An unexpected error occurred. Please contact support if the issue persists.",
                        null,
                        context.TraceIdentifier
                    ).ExecuteResultAsync(new ActionContext { HttpContext = context });
                }
            });
        });

        return app;
    }

}
