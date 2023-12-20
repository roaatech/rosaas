using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
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
        public DbSet<EntityAdminPrivilege> EntityAdminPrivileges { get; }
        public DbSet<TenantStatusHistory> TenantStatusHistory { get; }
        public DbSet<TenantHealthCheckHistory> TenantHealthCheckHistory { get; }
        public DbSet<TenantProcessHistory> TenantProcessHistory { get; }
        public DbSet<Subscription> Subscriptions { get; }
        public DbSet<Order> Orders { get; }
        public DbSet<SubscriptionAutoRenewal> SubscriptionAutoRenewals { get; }
        public DbSet<SubscriptionAutoRenewalHistory> SubscriptionAutoRenewalHistories { get; }
        public DbSet<SubscriptionPlanChanging> SubscriptionPlanChanges { get; }
        public DbSet<SubscriptionPlanChangeHistory> SubscriptionPlanChangeHistories { get; }
        public DbSet<SubscriptionCycle> SubscriptionCycles { get; }
        public DbSet<SubscriptionFeature> SubscriptionFeatures { get; }
        public DbSet<SubscriptionFeatureCycle> SubscriptionFeatureCycles { get; }
        public DbSet<TenantHealthStatus> TenantHealthStatuses { get; }
        public DbSet<ExternalSystemDispatch> ExternalSystemDispatches { get; }
        public DbSet<Specification> Specifications { get; }
        public DbSet<SpecificationValue> SpecificationValues { get; set; }
        public DbSet<Feature> Features { get; }
        public DbSet<Plan> Plans { get; }
        public DbSet<PlanFeature> PlanFeatures { get; }
        public DbSet<PlanPrice> PlanPrices { get; }
        public DbSet<JobTask> JobTasks { get; }
        public DbSet<Setting> Settings { get; }
        public DbSet<TenantCreationRequest> TenantCreationRequests { get; }
        public DbSet<TenantCreationRequestItem> TenantCreationRequestItems { get; }
        public DbSet<TenantName> TenantNames { get; }

        #endregion

        public DatabaseFacade Database { get; }

        public IModel Model { get; }

        public EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

        #region Services     
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        Task DispatchDomainEvents();
        Task DispatchDomainEvents(params BaseInternalEvent[] domainEvents);
        #endregion

    }
}
