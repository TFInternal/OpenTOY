using OpenTOY.Data.Entities;

namespace OpenTOY.Data.Repositories;

public interface IGuestAccountRepository : IRepository<GuestAccountEntity>
{
}

public class GuestAccountRepository : RepositoryBase<GuestAccountEntity>, IGuestAccountRepository
{
    public GuestAccountRepository(AppDb db) : base(db)
    {
    }
}