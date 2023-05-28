using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Identity;

namespace Roaa.Rosas.Application.Interfaces.DbContexts
{
    public interface IRosasIdentityDbContext
    {
        #region DbSets      
        public DbSet<User> Users { get; }
        public DbSet<UserClaim> UserClaims { get; }
        public DbSet<UserRole> UserRoles { get; }
        public DbSet<Role> Roles { get; }
        public DbSet<RoleClaim> RoleClaims { get; }

        #endregion

        public DatabaseFacade Database { get; }

        public EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

        #region Services     
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        Task DispatchDomainEvents();
        Task DispatchDomainEvents(params BaseInternalEvent[] domainEvents);
        #endregion

    }
}
