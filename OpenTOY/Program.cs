using System.Text.Json;
using FastEndpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpLogging(o =>
{
    o.RequestHeaders.Add("acceptCountry");
    o.RequestHeaders.Add("acceptLanguage");
    o.RequestHeaders.Add("uuid");
    o.RequestHeaders.Add("npparams");
    o.RequestHeaders.Add("npsn");
});

builder.Services.AddFastEndpoints();

var app = builder.Build();

app.UseHttpLogging();

app.UseFastEndpoints(o =>
{
    o.Serializer.Options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

app.Run();