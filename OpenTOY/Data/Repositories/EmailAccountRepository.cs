using Microsoft.EntityFrameworkCore;
using OpenTOY.Data.Entities;

namespace OpenTOY.Data.Repositories;

public interface IEmailAccountRepository : IRepository<EmailAccountEntity>
{
    Task<EmailAccountEntity?> GetByEmailAsync(int serviceId, string email);
    Task<bool> CheckEmailRegisteredAsync(int serviceId, string email);
}

public class EmailAccountRepository : RepositoryBase<EmailAccountEntity>, IEmailAccountRepository
{
    public EmailAccountRepository(AppDb db) : base(db)
    {
    }

    public async Task<EmailAccountEntity?> GetByEmailAsync(int serviceId, string email)
    {
        return await Db.EmailAccounts
            .Include(ea => ea.User)
            .FirstOrDefaultAsync(ea => ea.ServiceId == serviceId && ea.Email == email);
    }

    public async Task<bool> CheckEmailRegisteredAsync(int serviceId, string email)
    {
        return await Db.EmailAccounts.AnyAsync(ea => ea.ServiceId == serviceId && ea.Email == email);
    }
}