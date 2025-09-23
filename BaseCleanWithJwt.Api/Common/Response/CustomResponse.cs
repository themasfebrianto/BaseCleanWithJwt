using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace BaseCleanWithJwt.Api.Common.Response;

public class CustomResponse(int codeStatus, string message, object? data = null, string? traceId = null) : IActionResult
{
    public int CodeStatus = codeStatus;
    public string Message = message;
    public object? Data = data;
    public string? TraceId = traceId;

    public async Task ExecuteResultAsync(ActionContext context)
    {
        context.HttpContext.Response.StatusCode = CodeStatus;

        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            statusCode = CodeStatus,
            message = Message,
            data = Data
        });
    }

    public static CustomResponse StatusCode(int statusCode, string message, object? data = null, string? traceId = null) =>
        new(statusCode, message, data, traceId);

    public static CustomResponse Exception(Exception ex)
    {
        var code = 500;
        var msg = "Internal server error.";
        var traceId = Guid.NewGuid().ToString();

        var parts = ex.Message.Split('@');
        if (parts.Length == 2 && int.TryParse(parts[1], out var parsed))
        {
            code = parsed;
            msg = parts[0];
        }

        if (code == 500)
            Log.ForContext<CustomResponse>().Error(ex, "{TraceId} - Error", traceId);

        return new CustomResponse(
            code,
            code == 500 ? $"Internal server error, traceId: {traceId}" : msg,
            null,
            code == 500 ? traceId : null
        );
    }
}