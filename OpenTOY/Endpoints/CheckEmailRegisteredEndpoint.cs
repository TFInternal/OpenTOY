using System.Text.Json.Serialization;
using FastEndpoints;
using Microsoft.Extensions.Options;
using OpenTOY.Attributes;
using OpenTOY.Extensions;
using OpenTOY.Filters;
using OpenTOY.Options;
using OpenTOY.Services;

namespace OpenTOY.Endpoints;

[CommonEncryption]
public class CheckEmailRegisteredEndpoint : Endpoint<CheckEmailRegisteredRequest, CheckEmailRegisteredResponse>
{
    private readonly IAccountService _accountService;
    
    private readonly IOptions<ServiceOptions> _serviceOptions;
    
    public CheckEmailRegisteredEndpoint(IAccountService accountService, IOptions<ServiceOptions> serviceOptions)
    {
        _accountService = accountService;
        _serviceOptions = serviceOptions;
    }
    
    public override void Configure()
    {
        Post("/sdk/isRegisteredNPAA.nx");
        AllowAnonymous();
        AllowFormData(true);
        Options(x =>
        {
            x
                .AddEndpointFilter<JsonFilter>()
                .AddEndpointFilter<CommonDecryptionFilter>();
        });
    }

    public override async Task HandleAsync(CheckEmailRegisteredRequest req, CancellationToken ct)
    {
        Logger.LogInformation("CheckEmailRegistered - Email: {Email} ServiceId: {ServiceId}",
            req.Email, req.NpParams.SvcId);
        
        var serviceExists = _serviceOptions.Value.Services.TryGetValue(req.NpParams.SvcId, out _);
        if (!serviceExists)
        {
            Logger.LogError("Service doesn't exist: {ServiceId}", req.NpParams.SvcId);
            await Send.NotFoundAsync();
            return;
        }
        
        var isRegistered = await _accountService.CheckEmailRegisteredAsync(int.Parse(req.NpParams.SvcId), req.Email);
        
        var response = new CheckEmailRegisteredResponse
        {
            Result = new EmailRegisteredResult
            {
                IsRegistered = isRegistered ? 1 : 0
            }
        };
        
        await this.SendCommonEncryptedAsync(response);
    }
}

public class CheckEmailRegisteredRequest : BaseRequest
{
    [JsonPropertyName("userID")]
    public string Email { get; set; } = string.Empty;
}

public class CheckEmailRegisteredResponse : BaseResponse
{
    public required EmailRegisteredResult Result { get; set; }
}

public class EmailRegisteredResult
{
    public int IsRegistered { get; set; }
}