using FastEndpoints;
using OpenTOY.Attributes;
using OpenTOY.Data.Repositories;
using OpenTOY.Extensions;
using OpenTOY.Filters;

namespace OpenTOY.Endpoints;

[NoEncryption]
public class GetEmailUserInfoEndpoint : Endpoint<GetEmailUserInfoRequest, GetEmailUserInfoResponse>
{
    private readonly IEmailAccountRepository _emailAccountRepository;

    public GetEmailUserInfoEndpoint(IEmailAccountRepository emailAccountRepository)
    {
        _emailAccountRepository = emailAccountRepository;
    }

    public override void Configure()
    {
        Post("/auth/me.nx");
        AllowFormData(true);
        Options(x =>
        {
            x
                .AddEndpointFilter<JsonFilter>()
                .AddEndpointFilter<NoDecryptionFilter>();
        });
    }

    public override async Task HandleAsync(GetEmailUserInfoRequest req, CancellationToken ct)
    {
        Logger.LogInformation("GetEmailUserInfo - ID: {Id} Params: {Params}",
            req.Id, req.NpParams.ToString(true));

        var userId = this.GetUserId();
        var serviceId = this.GetServiceId();
        var emailAccount = await _emailAccountRepository.GetByIdAsync(serviceId, userId);
        if (emailAccount is null)
        {
            Logger.LogWarning("User not found - ID: {UserId} ServiceID: {ServiceId}",
                userId, serviceId);
            await Send.NotFoundAsync();
            return;
        }

        var response = new GetEmailUserInfoResponse
        {
            Email = emailAccount.Email,
            Extend = new EmailUserInfo
            {
                Id = emailAccount.Id.ToString()
            }
        };

        await Send.OkAsync(response);
    }
}

public class GetEmailUserInfoRequest : BaseRequest
{
}

public class GetEmailUserInfoResponse : BaseResponse
{
    public string Email { get; set; } = string.Empty;
    public required EmailUserInfo Extend { get; set; }
}

public class EmailUserInfo
{
    public string Id { get; set; } = string.Empty;
}