using Microsoft.EntityFrameworkCore;
using OpenTOY.Data.Entities;

namespace OpenTOY.Data.Repositories;

public interface IGuestAccountRepository : IRepository<GuestAccountEntity>
{
    Task<GuestAccountEntity?> GetByIdAsync(int serviceId, string deviceId);
}

public class GuestAccountRepository : RepositoryBase<GuestAccountEntity>, IGuestAccountRepository
{
    public GuestAccountRepository(AppDb db) : base(db)
    {
    }

    public async Task<GuestAccountEntity?> GetByIdAsync(int serviceId, string deviceId)
    {
        return await Db.GuestAccounts
            .Include(ga => ga.User)
            .FirstOrDefaultAsync(ga => ga.ServiceId == serviceId && ga.DeviceId == deviceId);
    }
}