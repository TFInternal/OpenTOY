using System.Text;
using OpenTOY.Utils;

namespace OpenTOY.Filters;

public abstract class BaseDecryptionFilter : IEndpointFilter
{
    private const string ParamsKey = "npparams";
    
    public abstract ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next);
    
    protected void DecryptParams(HttpRequest request, byte[] key)
    {
        if (!request.Headers.TryGetValue(ParamsKey, out var npParams))
        {
            return;
        }
        
        var decrypted = Crypto.Decrypt(npParams.ToString(), key);
        request.Headers[ParamsKey] = decrypted;
    }

    protected async Task DecryptRequest(HttpRequest request, byte[] key)
    {
        request.EnableBuffering();
        
        using var memoryStream = new MemoryStream();
        await request.Body.CopyToAsync(memoryStream);
        
        var decryptedBody = Crypto.Decrypt(memoryStream.ToArray(), key);

        var decryptedStream = new MemoryStream(Encoding.UTF8.GetBytes(decryptedBody));
        request.Body = decryptedStream;

        request.Body.Position = 0;
    }
}