using FastEndpoints;
using Microsoft.Extensions.Options;
using OpenTOY.Auth;
using OpenTOY.Options;

namespace OpenTOY.Endpoints.GameServer;

public class VerifyUserEndpoint : Endpoint<VerifyUserRequest>
{
    private readonly ITokenValidator _tokenValidator;
    
    private readonly IOptions<ServiceOptions> _serviceOptions;

    public VerifyUserEndpoint(ITokenValidator tokenValidator, IOptions<ServiceOptions> serviceOptions)
    {
        _tokenValidator = tokenValidator;
        _serviceOptions = serviceOptions;
    }

    public override void Configure()
    {
        Post("/gameserver/verifyUser");
        AllowAnonymous();
    }

    public override async Task HandleAsync(VerifyUserRequest req, CancellationToken ct)
    {
        Logger.LogInformation("Got request to verify user {UserId} from {Server}",
            req.Id, HttpContext.Connection.RemoteIpAddress);

        if (!_tokenValidator.IsValidToken(req.Password, out var jwt))
        {
            Logger.LogWarning("Invalid token for user {UserId}", req.Id);
            await Send.UnauthorizedAsync();
            return;
        }
        
        var userId = jwt.Claims.First(x => x.Type == "UserId").Value;
        var serviceId = jwt.Claims.First(x => x.Type == "ServiceId").Value;
        
        if (!_serviceOptions.Value.Services.ContainsKey(serviceId))
        {
            Logger.LogError("Token was valid but service doesn't exist: {ServiceId}", serviceId);
            await Send.NotFoundAsync();
            return;
        }
        
        // TODO: verify the user exists in the database
        
        Logger.LogInformation("User {UserId} has a valid token", userId);

        await Send.OkAsync();
    }
}

public class VerifyUserRequest
{
    public long Id { get; set; }
    public string Password { get; set; } = string.Empty;
}