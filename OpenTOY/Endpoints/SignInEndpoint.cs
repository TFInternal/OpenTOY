using System.Text.Json.Serialization;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.Extensions.Options;
using OpenTOY.Extensions;
using OpenTOY.Filters;
using OpenTOY.Options;
using OpenTOY.Services;
using OpenTOY.Utils;

namespace OpenTOY.Endpoints;

public class SignInEndpoint : Endpoint<SignInRequest, SignInResponse>
{
    private readonly IAccountService _accountService;
    
    private readonly IOptions<ServiceOptions> _serviceOptions;
    
    private readonly IOptions<JwtOptions> _jwtOptions;

    public SignInEndpoint(IAccountService accountService, IOptions<ServiceOptions> serviceOptions,
        IOptions<JwtOptions> jwtOptions)
    {
        _accountService = accountService;
        _serviceOptions = serviceOptions;
        _jwtOptions = jwtOptions;
    }

    public override void Configure()
    {
        Post("/sdk/signIn.nx");
        AllowAnonymous();
        AllowFormData(true);
        Options(x =>
        {
            x
                .AddEndpointFilter<JsonFilter>()
                .AddEndpointFilter<CommonDecryptionFilter>();
        });
    }

    public override async Task HandleAsync(SignInRequest req, CancellationToken ct)
    {
        Logger.LogInformation("SignIn - Device: {DeviceType} UUID2: {Uuid2}, UserId: {UserId}, Passwd: {Passwd}, MemType: {MemType} Params: {Params}",
            req.DeviceInfo.Device, req.Uuid2, req.UserId, req.Passwd, req.MemType, req.NpParams);

        var serviceExists = _serviceOptions.Value.Services.TryGetValue(req.NpParams.SvcId, out _);
        if (!serviceExists)
        {
            Logger.LogError("Service doesn't exist: {ServiceId}", req.NpParams.SvcId);
            await SendNotFoundAsync();
            return;
        }

        var user = await _accountService.GetOrCreateUserAsync(req);
        var serviceId = user.ServiceId;
        var userId = user.Id;

        var jwtToken = JwtBearer.CreateToken(o =>
        {
            o.SigningKey = _jwtOptions.Value.Key;
            o.ExpireAt = DateTime.UtcNow.AddDays(7);
            o.User["UserId"] = userId.ToString();
            o.User["ServiceId"] = serviceId.ToString();
        });
        
        var response = new SignInResponse
        {
            Result = new ToyLoginResult
            {
                Id = ToyUser.GenerateNpsn(serviceId, userId),
                Token = jwtToken
            }
        };

        await this.SendCommonEncryptedAsync(response);
    }
}

public class SignInRequest : BaseRequest
{
    public string Uuid2 { get; set; } = string.Empty;
    // Contains the email address
    public string? UserId { get; set; }
    // When logging in with an email, this will be encrypted
    public string Passwd { get; set; } = string.Empty;
    public int MemType { get; set; }
    [JsonPropertyName("optional")]
    public required DeviceInfo DeviceInfo { get; set; }
}

public class DeviceInfo
{
    public string Name { get; set; } = string.Empty;
    public string Device { get; set; } = string.Empty;
}

public class SignInResponse : BaseResponse
{
    public required ToyLoginResult Result { get; set; }
}

public class ToyLoginResult
{
    [JsonPropertyName("npSN")]
    public long Id { get; set; }
    [JsonPropertyName("npToken")]
    public string Token { get; set; } = string.Empty;
}