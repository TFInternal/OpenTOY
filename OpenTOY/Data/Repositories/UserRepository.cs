using OpenTOY.Data.Entities;

namespace OpenTOY.Data.Repositories;

public interface IUserRepository : IRepository<UserEntity>
{
}

public class UserRepository : RepositoryBase<UserEntity>, IUserRepository
{
    public UserRepository(AppDb db) : base(db)
    {
    }
}