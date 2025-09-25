using BaseCleanWithJwt.Api.Extension;
using BaseCleanWithJwt.Api.Middleware;
using BaseCleanWithJwt.Application;
using BaseCleanWithJwt.Infrastructure;
using BaseCleanWithJwt.Infrastructure.MongoDb;
using DotNetEnv;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.AddSettings();
builder.AddJwtAuthentication();
builder.CreateCors();
builder.ConfigureSwagger();

MongoMappings.Register();

builder.Services.AddInfrastructureServices();
builder.Services.AddApplicationServices();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCustomResponse();

var app = builder.Build();

app.UseSwaggerApp();

app.UseAuthentication();
app.UseCustomResponse(app.Environment);

app.UseAuthorization();
app.MapControllers();
app.ConfigureCors();

app.Run();
