using System.Text.Json.Serialization;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.Extensions.Options;
using OpenTOY.Extensions;
using OpenTOY.Filters;
using OpenTOY.Options;

namespace OpenTOY.Endpoints;

public class SignInEndpoint : Endpoint<SignInRequest, SignInResponse>
{
    private readonly IOptions<JwtOptions> _jwtOptions;

    public SignInEndpoint(IOptions<JwtOptions> jwtOptions)
    {
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

        var serviceId = int.Parse(req.NpParams.SvcId);
        var userId = 10;

        var digitsServiceId = (int) Math.Log10(serviceId) + 1;
        var digitsUserId = (int) Math.Log10(userId) + 1;
        var zerosToAdd = 17 - digitsServiceId - digitsUserId;

        var npsn = serviceId * (long) Math.Pow(10, zerosToAdd + digitsUserId) + userId;

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
                Id = npsn,
                Token = jwtToken
            }
        };

        await this.SendCommonEncryptedAsync(response);
    }
}

public class SignInRequest : BaseRequest
{
    public string Uuid2 { get; set; } = string.Empty;
    public string? UserId { get; set; }
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