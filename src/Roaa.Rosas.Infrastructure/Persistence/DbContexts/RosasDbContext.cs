﻿using MediatR;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Auditing.Contexts;
using Roaa.Rosas.Auditing.Models;
using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Identity;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Infrastructure.Common;
using Roaa.Rosas.Infrastructure.Persistence.Interceptors;
using System.Reflection;

namespace Roaa.Rosas.Infrastructure.Persistence.DbContexts
{
    public class RosasDbContext : IdentityDbContext<User,
                                                         Role, Guid,
                                                         UserClaim,
                                                         UserRole,
                                                         UserLogin,
                                                         RoleClaim,
                                                         UserToken>, IRosasDbContext, IAuditDbContext
    {

        #region Props   
        private readonly IMediator _mediator;
        private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;
        #endregion

        #region Ctors     
        public RosasDbContext(DbContextOptions<RosasDbContext> options,
                                 AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor,
                                 IMediator mediator)
            : base(options)
        {
            _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
            _mediator = mediator;
        }
        #endregion


        #region DbSets      
        public override DbSet<User> Users { get; set; }
        public override DbSet<UserClaim> UserClaims { get; set; }
        public override DbSet<UserRole> UserRoles { get; set; }
        public override DbSet<Role> Roles { get; set; }
        public override DbSet<RoleClaim> RoleClaims { get; set; }



        public DbSet<Client> Clients { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<TenantProcess> TenantProcesses { get; set; }
        public DbSet<ProductTenant> ProductTenants { get; set; }

        public DbSet<AuditEntity> Audits { get; set; }
        #endregion


        #region Services    
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.AddInterceptors(_auditableEntitySaveChangesInterceptor);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var domainEvents = await _mediator.FetchDomainEvents(this);

            int changes = await base.SaveChangesAsync(cancellationToken);

            if (changes > 0 && domainEvents.Count > 0)
            {
                await _mediator.DispatchDomainEvents(domainEvents);
            }

            return changes;

        }


        public async Task DispatchDomainEvents()
        {
            var domainEvents = await _mediator.FetchDomainEvents(this);

            await _mediator.DispatchDomainEvents(domainEvents);
        }
        public async Task DispatchDomainEvents(params BaseInternalEvent[] domainEvents)
        {
            await _mediator.DispatchDomainEvents(domainEvents.ToList());
        }
        #endregion
    }
}