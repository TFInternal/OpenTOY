using OpenTOY.Utils;

namespace OpenTOY.Filters;

public class CommonDecryptionFilter : BaseDecryptionFilter
{
    public override async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var key = Crypto.HexStringToByteArray(Constants.Key);
        
        DecryptParams(context.HttpContext.Request, key);
        await DecryptRequest(context.HttpContext.Request, key);
        
        return await next(context);
    }
}