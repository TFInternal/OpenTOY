using Microsoft.EntityFrameworkCore;
using OpenTOY.Data.Entities;

namespace OpenTOY.Data;

public class AppDb : DbContext
{
    public DbSet<UserEntity> Users { get; set; }
    
    public DbSet<EmailAccountEntity> EmailAccounts { get; set; }
    
    public DbSet<GuestAccountEntity> GuestAccounts { get; set; }
    
    public AppDb(DbContextOptions<AppDb> options) : base(options)
    {
    }
}