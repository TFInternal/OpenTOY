namespace OpenTOY.Filters;

public class JsonFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        context.HttpContext.Request.ContentType = "application/json";
        
        return await next(context);
    }
}