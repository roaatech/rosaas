using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Domain.Entities;

public class IdentityServerPersistedGrantDbContext : PersistedGrantDbContext<IdentityServerPersistedGrantDbContext>, IIdentityServerPersistedGrantDbContext
{
    #region Ctors    
    public IdentityServerPersistedGrantDbContext(DbContextOptions<IdentityServerPersistedGrantDbContext> options, OperationalStoreOptions storeOptions)
        : base(options, storeOptions)
    {
    }
    #endregion

    #region DbSets      
    public DbSet<PersistedUserGrant> UserPersistedGrants { get; set; }
    #endregion

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<PersistedUserGrant>().ToTable("IdS4g_IdentityPersistedUserGrants");
        builder.Entity<PersistedUserGrant>().HasKey(x => x.Key);
        builder.Entity<PersistedUserGrant>().Property(r => r.Key).HasMaxLength(100);
        builder.Entity<PersistedUserGrant>().Property(r => r.AuthenticationMethod).HasMaxLength(50);
        builder.Entity<PersistedUserGrant>().Property(r => r.ClientId).HasMaxLength(50);
        builder.Entity<PersistedUserGrant>().Property(r => r.Type).HasMaxLength(50);
    }
}