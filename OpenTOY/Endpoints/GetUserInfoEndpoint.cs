using System.Text.Json.Serialization;
using FastEndpoints;
using OpenTOY.Data.Repositories;
using OpenTOY.Extensions;
using OpenTOY.Filters;

namespace OpenTOY.Endpoints;

public class GetUserInfoEndpoint : Endpoint<GetUserInfoRequest, GetUserInfoResponse>
{
    private readonly IUserRepository _userRepository;

    public GetUserInfoEndpoint(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public override void Configure()
    {
        Post("/sdk/getUserInfo.nx");
        AllowFormData(true);
        Options(x =>
        {
            x
                .AddEndpointFilter<JsonFilter>()
                .AddEndpointFilter<UserDecryptionFilter>();
        });
    }

    public override async Task HandleAsync(GetUserInfoRequest req, CancellationToken ct)
    {
        Logger.LogInformation("GetUserInfo - ID: {Id} AdvertisingId: {AdvertisingId} Params: {Params}",
            req.Id, req.AdvertisingId, req.NpParams.ToString(true));

        var userId = this.GetUserId();
        var serviceId = this.GetServiceId();
        var user = await _userRepository.GetByIdAsync(userId, serviceId);
        if (user is null)
        {
            await Send.NotFoundAsync();
            return;
        }

        var response = new GetUserInfoResponse
        {
            Result = new UserInfoResult
            {
                DoToast = true,
                NpsnUserInfo = new ToyUserInfo
                {
                    MemType = (int) user.MembershipType
                },
                PushAgree = 1
            }
        };
        
        await this.SendUserEncryptedAsync(response, req.Id);
    }
}

public class GetUserInfoRequest : BaseRequest
{
    [JsonPropertyName("adid")]
    public string AdvertisingId { get; set; } = string.Empty;
}

public class GetUserInfoResponse : BaseResponse
{
    public required UserInfoResult Result { get; set; }
}

public class UserInfoResult
{
    public bool DoToast { get; set; }
    public required ToyUserInfo NpsnUserInfo { get; set; }
    public int PushAgree { get; set; }
}

public class ToyUserInfo
{
    public int MemType { get; set; }
}