using OpenTOY.Data.Entities;
using OpenTOY.Data.Repositories;
using OpenTOY.Endpoints;

namespace OpenTOY.Services;

public interface IAccountService
{
    Task<UserEntity> GetOrCreateUserAsync(SignInRequest req);
}

public class AccountService : IAccountService
{
    private readonly IUserRepository _userRepository;
    
    private readonly IGuestAccountRepository _guestAccountRepository;

    public AccountService(IUserRepository userRepository, IGuestAccountRepository guestAccountRepository)
    {
        _userRepository = userRepository;
        _guestAccountRepository = guestAccountRepository;
    }

    public async Task<UserEntity> GetOrCreateUserAsync(SignInRequest req)
    {
        var serviceId = int.Parse(req.NpParams.SvcId);
        
        var deviceId = req.Uuid2;
        
        if (req.MemType == (int) MembershipType.Guest)
        {
            var guestAccountEntity = await _guestAccountRepository.GetByIdAsync(serviceId, deviceId);
            if (guestAccountEntity is not null)
            {
                return guestAccountEntity.User!;
            }
            
            var userEntity = new UserEntity
            {
                ServiceId = serviceId,
                MembershipType = MembershipType.Guest
            };
            
            await _userRepository.AddAsync(userEntity);
            
            var guestAccount = new GuestAccountEntity
            {
                Id = userEntity.Id,
                ServiceId = userEntity.ServiceId,
                DeviceId = deviceId
            };
            
            await _guestAccountRepository.AddAsync(guestAccount);
            
            return userEntity;
        }
        
        throw new NotImplementedException();
    }
}