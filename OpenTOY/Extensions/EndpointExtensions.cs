using System.Text;
using System.Text.Json;
using FastEndpoints;
using OpenTOY.Utils;

namespace OpenTOY.Extensions;

public static class EndpointExtensions
{
    private static readonly JsonSerializerOptions SerializeOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    
    public static async Task SendCommonEncryptedAsync<TResponse>(this IEndpoint ep,
        TResponse response,
        int statusCode = 200,
        CancellationToken cancellationToken = default)
    {
        ep.HttpContext.MarkResponseStart();
        ep.HttpContext.Response.StatusCode = statusCode;
        
        var json = JsonSerializer.Serialize(response, SerializeOptions);
        var encryptedJson = Crypto.Encrypt(Encoding.ASCII.GetBytes(json), Constants.Key);

        using var stream = new MemoryStream();
        await stream.WriteAsync(encryptedJson, cancellationToken);
        stream.Position = 0;
        await stream.CopyToAsync(ep.HttpContext.Response.Body, cancellationToken);
    }
    
    public static async Task SendUserEncryptedAsync<TResponse>(this IEndpoint ep,
        TResponse response,
        long npsn,
        int statusCode = 200,
        CancellationToken cancellationToken = default)
    {
        ep.HttpContext.MarkResponseStart();
        ep.HttpContext.Response.StatusCode = statusCode;

        var tmpkey = npsn.ToString()[2..];
        var key = Crypto.Encrypt(Encoding.ASCII.GetBytes(tmpkey),
            Encoding.ASCII.GetBytes(npsn.ToString()[1..]));
        
        var json = JsonSerializer.Serialize(response, SerializeOptions);
        var encryptedJson = Crypto.Encrypt(Encoding.ASCII.GetBytes(json), key);

        using var stream = new MemoryStream();
        await stream.WriteAsync(encryptedJson, cancellationToken);
        stream.Position = 0;
        await stream.CopyToAsync(ep.HttpContext.Response.Body, cancellationToken);
    }
}