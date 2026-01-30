using FastEndpoints.Security;
using Microsoft.Extensions.Options;
using OpenTOY.Data.Entities;
using OpenTOY.Data.Repositories;
using OpenTOY.Endpoints;
using OpenTOY.Options;

namespace OpenTOY.Services;

public interface IAccountService
{
    Task<UserEntity?> GetOrCreateGuestAsync(SignInRequest req);
    Task<(UserEntity? user, string? error)> SignInEmailAsync(SignInRequest req);
    Task<UserEntity> CreateEmailAccountAsync(int serviceId, string email, string password);
    Task<bool> CheckEmailRegisteredAsync(int serviceId, string email);
    string GenerateJwtToken(int serviceId, int userId);
}

public class AccountService : IAccountService
{
    private readonly ILogger<AccountService> _logger;

    private readonly IPasswordService _passwordService;
    
    private readonly IUserRepository _userRepository;
    
    private readonly IEmailAccountRepository _emailAccountRepository;
    
    private readonly IGuestAccountRepository _guestAccountRepository;
    
    private readonly IOptions<JwtOptions> _jwtOptions;

    public AccountService(ILogger<AccountService> logger, IPasswordService passwordService,
        IUserRepository userRepository, IEmailAccountRepository emailAccountRepository,
        IGuestAccountRepository guestAccountRepository, IOptions<JwtOptions> jwtOptions)
    {
        _logger = logger;
        _passwordService = passwordService;
        _userRepository = userRepository;
        _emailAccountRepository = emailAccountRepository;
        _guestAccountRepository = guestAccountRepository;
        _jwtOptions = jwtOptions;
    }

    public async Task<UserEntity?> GetOrCreateGuestAsync(SignInRequest req)
    {
        var serviceId = int.Parse(req.NpParams.SvcId);
        var deviceId = req.Uuid2;

        if (deviceId.Length != 16 && deviceId.Length != 36)
        {
            _logger.LogWarning("Invalid device ID length for guest account: {DeviceId}", deviceId);
            return null;
        }

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

    public async Task<(UserEntity? user, string? error)> SignInEmailAsync(SignInRequest req)
    {
        var serviceId = int.Parse(req.NpParams.SvcId);
        
        var emailAccountEntity = await _emailAccountRepository.GetByEmailAsync(serviceId, req.UserId!.ToLower());
        if (emailAccountEntity is null)
        {
            return (null, "Email not found");
        }

        if (!_passwordService.VerifyPassword(req.Passwd, emailAccountEntity.Password, emailAccountEntity.Salt))
        {
            return (null, "Invalid password");
        }
        
        return (emailAccountEntity.User!, null);
    }

    public async Task<UserEntity> CreateEmailAccountAsync(int serviceId, string email, string password)
    {
        var hashedPassword = _passwordService.HashPassword(password, out var salt);
        
        var userEntity = new UserEntity
        {
            ServiceId = serviceId,
            MembershipType = MembershipType.Email
        };
        
        await _userRepository.AddAsync(userEntity);
        
        var emailAccountEntity = new EmailAccountEntity
        {
            Id = userEntity.Id,
            ServiceId = userEntity.ServiceId,
            Email = email,
            Password = hashedPassword,
            Salt = Convert.ToHexString(salt)
        };
        
        await _emailAccountRepository.AddAsync(emailAccountEntity);
        
        return userEntity;
    }

    public async Task<bool> CheckEmailRegisteredAsync(int serviceId, string email)
    {
        return await _emailAccountRepository.CheckEmailRegisteredAsync(serviceId, email.ToLower());
    }

    public string GenerateJwtToken(int serviceId, int userId)
    {
        // JWTs are supposed to be short-lived, but I haven't figured out how to get TOY to refresh them yet
        // Or if that's even something it can do
        var jwtToken = JwtBearer.CreateToken(o =>
        {
            o.SigningKey = _jwtOptions.Value.Key;
            o.ExpireAt = DateTime.UtcNow.AddYears(5);
            o.User["UserId"] = userId.ToString();
            o.User["ServiceId"] = serviceId.ToString();
        });

        return jwtToken;
    }
}