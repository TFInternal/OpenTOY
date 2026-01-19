using FastEndpoints;
using OpenTOY.Attributes;
using OpenTOY.Filters;

namespace OpenTOY.Endpoints;

[CommonEncryption]
public class RegisterPushEndpoint : Endpoint<RegisterPushRequest>
{
    public override void Configure()
    {
        Post("/sdk/registerPush.nx");
        AllowAnonymous();
        AllowFormData(true);
        Options(x =>
        {
            x
                .AddEndpointFilter<JsonFilter>()
                .AddEndpointFilter<CommonDecryptionFilter>();
        });
    }

    public override async Task HandleAsync(RegisterPushRequest req, CancellationToken ct)
    {
        Logger.LogInformation("RegisterPush - ID: {Id} PushKey: {PushKey} Params: {Params}",
            req.Id, req.PushKey, req.NpParams.ToString(true));

        await Send.OkAsync();
    }
}

public class RegisterPushRequest : BaseRequest
{
    public string PushKey { get; set; } = string.Empty;
}