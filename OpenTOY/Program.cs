using System.Text.Json;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using OpenTOY.Auth;
using OpenTOY.Data;
using OpenTOY.Data.Repositories;
using OpenTOY.Extensions;
using OpenTOY.Options;
using OpenTOY.Services;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

config.AddJsonFile("services.json", false, true);

builder.Services
    .AddConfiguredOptions<JwtOptions>(config)
    .AddConfiguredOptions<ServiceOptions>(config);

builder.Services.AddSingleton<ITokenValidator, TokenValidator>();

var connectionString = config.GetConnectionString("connection");

builder.Services.AddDbContext<AppDb>(o =>
{
    o.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEmailAccountRepository, EmailAccountRepository>();
builder.Services.AddScoped<IGuestAccountRepository, GuestAccountRepository>();

builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IAccountService, AccountService>();

builder.Services.AddHttpLogging(o =>
{
    o.RequestHeaders.Add("acceptCountry");
    o.RequestHeaders.Add("acceptLanguage");
    o.RequestHeaders.Add("uuid");
    o.RequestHeaders.Add("npparams");
    o.RequestHeaders.Add("npsn");
});

builder.Services.AddFastEndpoints();

builder.Services.AddAuthorization();
builder.Services
    .AddAuthentication(TokenAuth.SchemeName)
    .AddScheme<AuthenticationSchemeOptions, TokenAuth>(TokenAuth.SchemeName, null);

var app = builder.Build();

if (app.Environment.IsProduction())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDb>();
    db.Database.Migrate();
}

app.UseHttpLogging();

app.UseAuthorization();

app.UseFastEndpoints(o =>
{
    o.Serializer.Options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

app.Run();