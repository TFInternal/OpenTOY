using System.Text;
using OpenTOY.Utils;

namespace OpenTOY.Filters;

public class UserDecryptionFilter : BaseDecryptionFilter
{
    public override async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var key = GetKey(context.HttpContext.Request);
        if (key is null)
        {
            context.HttpContext.Response.StatusCode = 400;
            return null;
        }
        
        DecryptParams(context.HttpContext.Request, key);
        await DecryptRequest(context.HttpContext.Request, key);
        
        return await next(context);
    }
    
    private byte[]? GetKey(HttpRequest request)
    {
        if (!request.Headers.TryGetValue("npsn", out var npsn))
        {
            return null;
        }

        if (npsn.ToString().Length != 17)
        {
            return null;
        }

        var tmpkey = npsn.ToString()[2..];
        var key = Crypto.Encrypt(Encoding.ASCII.GetBytes(tmpkey),
            Encoding.ASCII.GetBytes(npsn.ToString()[1..]));
        
        return key;
    }
}