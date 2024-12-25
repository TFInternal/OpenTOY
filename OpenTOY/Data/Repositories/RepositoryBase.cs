using Microsoft.EntityFrameworkCore;

namespace OpenTOY.Data.Repositories;

public class RepositoryBase<T> : IRepository<T> where T : class
{
    protected readonly AppDb Db;

    public RepositoryBase(AppDb db)
    {
        Db = db;
    }

    public async Task<T?> GetByIdAsync(params object[] keyValues)
    {
        return await Db.FindAsync<T>(keyValues);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await Db.Set<T>().ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await Db.AddAsync(entity);
        await Db.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        Db.Update(entity);
        await Db.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        Db.Remove(entity);
        await Db.SaveChangesAsync();
    }
}