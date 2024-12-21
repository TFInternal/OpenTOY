using FastEndpoints;
using OpenTOY.Filters;

namespace OpenTOY.Endpoints;

public class EnterToyEndpoint : Endpoint<EnterToyRequest, EnterToyResponse>
{
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
        Logger.LogInformation("EnterToy - MNC: {MNC} MCC: {MCC} Params: {Params}", req.Mnc, req.Mcc, req.NpParams);

        var response = new EnterToyResponse
        {
            Result = new ToyEnterResult
            {
                Country = "FI",
                Service = new ToyService
                {
                    Title = "Titanfall Frontline_Live",
                    LoginUiType = "1",
                    ClientId = "OTI3MzA2MDA4", // TODO: how to generate this?
                    UseMemberships = [9999],
                    UseMembershipsInfo = new Dictionary<string, string>
                    {
                        { "nexonNetSecretKey", "" },
                        { "nexonNetProductId", "" },
                        { "nexonNetRedirectUri", "" }
                    }
                }
            }
        };

        await SendAsync(response);
    }
}

public class EnterToyRequest : BaseRequest
{
    // Mobile network code. For example, 05 for Elisa.
    public int Mnc { get; set; }
    // Mobile country code. For example, 244 for Finland.
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