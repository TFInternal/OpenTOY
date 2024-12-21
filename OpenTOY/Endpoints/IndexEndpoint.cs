using FastEndpoints;

namespace OpenTOY.Endpoints;

public class IndexEndpoint : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/");
        AllowAnonymous();
    }
    
    public override async Task HandleAsync(CancellationToken ct)
    {
        await SendStringAsync("Welcome to OpenTOY!");
    }
}