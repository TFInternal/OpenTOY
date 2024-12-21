using System.Text.Json.Serialization;
using FastEndpoints;
using OpenTOY.Extensions;
using OpenTOY.Filters;

namespace OpenTOY.Endpoints;

public class SignInEndpoint : Endpoint<SignInRequest, SignInResponse>
{
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
        Logger.LogInformation("SignIn - Device: {DeviceName}, {DeviceType} UUID: {Uuid}, {Uuid2}, UserId: {UserId}, Passwd: {Passwd}, MemType: {MemType}",
            req.DeviceInfo.Name, req.DeviceInfo.Device, req.Uuid, req.Uuid2, req.UserId, req.Passwd, req.MemType);

        var serviceId = int.Parse(req.NpParams.SvcId);
        var userId = 10;

        var digitsServiceId = (int) Math.Log10(serviceId) + 1;
        var digitsUserId = (int) Math.Log10(userId) + 1;
        var zerosToAdd = 17 - digitsServiceId - digitsUserId;

        var npsn = serviceId * (long) Math.Pow(10, zerosToAdd + digitsUserId) + userId;
        
        var response = new SignInResponse
        {
            Result = new ToyLoginResult
            {
                Id = npsn,
                Token = "wow"
            }
        };

        await this.SendCommonEncryptedAsync(response);
    }
}

public class SignInRequest : BaseRequest
{
    public string Uuid { get; set; } = string.Empty;
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