using BaseCleanWithJwt.Api.Extension;
using BaseCleanWithJwt.Api.Middleware;
using BaseCleanWithJwt.Application;
using BaseCleanWithJwt.Infrastructure;
using DotNetEnv;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.AddSettings();
builder.AddJwtAuthentication();
builder.CreateCors();
builder.ConfigureSwagger();

builder.Services.AddInfrastructureServices();
builder.Services.AddApplicationServices();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCustomResponse();

var app = builder.Build();

app.UseSwaggerApp();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseCustomResponse(app.Environment);

app.UseAuthorization();
app.MapControllers();
app.ConfigureCors();

app.Run();
