using System.Text.Json;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication;
using OpenTOY.Auth;
using OpenTOY.Extensions;
using OpenTOY.Options;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

config.AddJsonFile("services.json", false, true);

builder.Services
    .AddConfiguredOptions<JwtOptions>(config)
    .AddConfiguredOptions<ServiceOptions>(config);

builder.Services.AddSingleton<ITokenValidator, TokenValidator>();

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

app.UseHttpLogging();

app.UseAuthorization();

app.UseFastEndpoints(o =>
{
    o.Serializer.Options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

app.Run();