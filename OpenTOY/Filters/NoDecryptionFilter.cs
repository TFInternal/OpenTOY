using OpenTOY.Utils;

namespace OpenTOY.Filters;

public class NoDecryptionFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (context.HttpContext.Request.Headers.TryGetValue(Constants.ParamsKey, out var npParams))
        {
            var ascii = Crypto.HexStringToAscii(npParams.ToString());
            context.HttpContext.Request.Headers[Constants.ParamsKey] = ascii;
        }

        return await next(context);
    }
}