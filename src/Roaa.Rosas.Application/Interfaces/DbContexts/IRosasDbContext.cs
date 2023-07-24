using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Identity;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Interfaces.DbContexts
{
    public interface IRosasDbContext
    {
        #region DbSets      
        public DbSet<User> Users { get; }
        public DbSet<UserClaim> UserClaims { get; }
        public DbSet<UserRole> UserRoles { get; }
        public DbSet<Role> Roles { get; }
        public DbSet<RoleClaim> RoleClaims { get; }





        public DbSet<Client> Clients { get; }
        public DbSet<Product> Products { get; }
        public DbSet<Tenant> Tenants { get; }
        public DbSet<TenantProcess> TenantProcesses { get; }
        public DbSet<TenantHealthCheck> TenantHealthChecks { get; }
        public DbSet<ProductTenant> ProductTenants { get; }
        public DbSet<Feature> Features { get; }
        public DbSet<Plan> Plans { get; }
        public DbSet<PlanFeature> PlanFeatures { get; }
        public DbSet<JobTask> JobTasks { get; }

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
