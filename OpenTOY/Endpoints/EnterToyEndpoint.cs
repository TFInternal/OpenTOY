using FastEndpoints;
using Microsoft.Extensions.Options;
using OpenTOY.Attributes;
using OpenTOY.Filters;
using OpenTOY.Options;

namespace OpenTOY.Endpoints;

[CommonEncryption]
public class EnterToyEndpoint : Endpoint<EnterToyRequest, EnterToyResponse>
{
    private readonly IOptions<ServiceOptions> _serviceOptions;
    
    public EnterToyEndpoint(IOptions<ServiceOptions> serviceOptions)
    {
        _serviceOptions = serviceOptions;
    }
    
    public override void Configure()
    {
        Post("/sdk/enterToy.nx");
        AllowAnonymous();
        AllowFormData(true);
        Options(x =>
        {
            x
                .AddEndpointFilter<JsonFilter>()
                .AddEndpointFilter<CommonDecryptionFilter>();
        });
    }

    public override async Task HandleAsync(EnterToyRequest req, CancellationToken ct)
    {
        Logger.LogInformation("EnterToy - MNC: {MNC} MCC: {MCC} Params: {Params}",
            req.Mnc, req.Mcc, req.NpParams.ToString(true));

        var serviceExists = _serviceOptions.Value.Services.TryGetValue(req.NpParams.SvcId, out var serviceInfo);
        if (!serviceExists)
        {
            Logger.LogError("Service doesn't exist: {ServiceId}", req.NpParams.SvcId);
            await Send.NotFoundAsync();
            return;
        }

        var response = new EnterToyResponse
        {
            Result = new ToyEnterResult
            {
                Country = "FI",
                Service = new ToyService
                {
                    Title = serviceInfo!.Title,
                    LoginUiType = "1",
                    ClientId = "OTI3MzA2MDA4", // TODO: how to generate this?
                    UseMemberships = serviceInfo.LoginMethods,
                    UseMembershipsInfo = new Dictionary<string, string>
                    {
                        { "nexonNetSecretKey", "" },
                        { "nexonNetProductId", "" },
                        { "nexonNetRedirectUri", "" }
                    }
                }
            }
        };

        await Send.OkAsync(response);
    }
}

public class EnterToyRequest : BaseRequest
{
    /// <summary>
    /// Mobile network code. For example, 05 for Elisa.
    /// </summary>
    public int Mnc { get; set; }
    /// <summary>
    /// Mobile country code. For example, 244 for Finland.
    /// </summary>
    public int Mcc { get; set; }
}

public class EnterToyResponse : BaseResponse
{
    public required ToyEnterResult Result { get; set; }
}

public class ToyEnterResult
{
    public string Country { get; set; } = string.Empty;
    public required ToyService Service { get; set; }
}

public class ToyService
{
    public string Title { get; set; } = string.Empty;
    public string LoginUiType { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public required List<int> UseMemberships { get; set; }
    public required Dictionary<string, string> UseMembershipsInfo { get; set; }
}