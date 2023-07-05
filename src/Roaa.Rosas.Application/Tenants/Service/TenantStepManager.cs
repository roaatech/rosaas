﻿using MediatR;
using Roaa.Rosas.Application.Services.Management.Tenants;
using Roaa.Rosas.Common.Utilities;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Application.Tenants.Service
{
    public abstract class TenantStatusManager : Enumeration<TenantStatusManager, TenantStatus>
    {
        #region Props
        public static readonly TenantStatusManager PreCreating = new PreCreatingTenant();
        public static readonly TenantStatusManager Creating = new CreatingTenant();
        public static readonly TenantStatusManager Created = new CreatedTenant();
        public static readonly TenantStatusManager PreActivating = new PreActivatingTenant();
        public static readonly TenantStatusManager Active = new ActiveTenant();
        public static readonly TenantStatusManager PreDeactivating = new PreDeactivatingTenant();
        public static readonly TenantStatusManager Deactive = new DeactiveTenant();
        public static readonly TenantStatusManager PreDeleting = new PreDeletingTenant();
        public static readonly TenantStatusManager Deleted = new DeletedTenant();
        #endregion

        #region Corts
        protected TenantStatusManager(TenantStatus tenantStatus) : base(tenantStatus)
        {
        }
        #endregion

        #region abst   
        public abstract Task PublishEventAsync(IPublisher publisher, Tenant tenant, TenantStatus previousStatus, CancellationToken cancellationToken);
        #endregion



        #region inners  

        private sealed class PreCreatingTenant : TenantStatusManager
        {
            #region Corts
            public PreCreatingTenant() : base(TenantStatus.PreCreating) { }
            #endregion 

            #region overrides  
            public override async Task PublishEventAsync(IPublisher publisher, Tenant tenant, TenantStatus previousStatus, CancellationToken cancellationToken)
            {
                await publisher.Publish(new TenantPreCreatingEvent(tenant, previousStatus), cancellationToken);
            }
            #endregion
        }

        private sealed class CreatingTenant : TenantStatusManager
        {
            #region Corts
            public CreatingTenant() : base(TenantStatus.Creating) { }
            #endregion

            #region overrides   
            public override async Task PublishEventAsync(IPublisher publisher, Tenant tenant, TenantStatus previousStatus, CancellationToken cancellationToken)
            {
                await publisher.Publish(new TenantCreatedInStoreEvent(tenant));
            }
            #endregion
        }

        private sealed class PreActivatingTenant : TenantStatusManager
        {
            #region Corts
            public PreActivatingTenant() : base(TenantStatus.PreActivating) { }
            #endregion

            #region overrides  
            public override async Task PublishEventAsync(IPublisher publisher, Tenant tenant, TenantStatus previousStatus, CancellationToken cancellationToken)
            {
                await publisher.Publish(new TenantPreActivatingEvent(tenant, previousStatus), cancellationToken);
            }
            #endregion
        }

        private sealed class ActiveTenant : TenantStatusManager
        {
            #region Corts
            public ActiveTenant() : base(TenantStatus.Active) { }
            #endregion

            #region overrides  
            public override Task PublishEventAsync(IPublisher publisher, Tenant tenant, TenantStatus previousStatus, CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }
            #endregion
        }

        private sealed class PreDeactivatingTenant : TenantStatusManager
        {
            #region Corts
            public PreDeactivatingTenant() : base(TenantStatus.PreDeactivating) { }
            #endregion

            #region overrides  
            public override async Task PublishEventAsync(IPublisher publisher, Tenant tenant, TenantStatus previousStatus, CancellationToken cancellationToken)
            {
                await publisher.Publish(new TenantPreDeactivatingEvent(tenant, previousStatus), cancellationToken);
            }
            #endregion
        }

        private sealed class DeactiveTenant : TenantStatusManager
        {
            #region Corts
            public DeactiveTenant() : base(TenantStatus.Deactive) { }
            #endregion

            #region overrides  
            public override Task PublishEventAsync(IPublisher publisher, Tenant tenant, TenantStatus previousStatus, CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }
            #endregion
        }

        private sealed class PreDeletingTenant : TenantStatusManager
        {
            #region Corts
            public PreDeletingTenant() : base(TenantStatus.PreDeleting) { }
            #endregion

            #region overrides  
            public override async Task PublishEventAsync(IPublisher publisher, Tenant tenant, TenantStatus previousStatus, CancellationToken cancellationToken)
            {
                await publisher.Publish(new TenantPreDeletingEvent(tenant, previousStatus), cancellationToken);
            }
            #endregion
        }

        private sealed class DeletedTenant : TenantStatusManager
        {
            #region Corts
            public DeletedTenant() : base(TenantStatus.Deleted) { }
            #endregion

            #region overrides  
            public override Task PublishEventAsync(IPublisher publisher, Tenant tenant, TenantStatus previousStatus, CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }
            #endregion
        }

        private sealed class CreatedTenant : TenantStatusManager
        {
            #region Corts
            public CreatedTenant() : base(TenantStatus.CreatedAsActive) { }
            #endregion

            #region overrides  
            public override Task PublishEventAsync(IPublisher publisher, Tenant tenant, TenantStatus previousStatus, CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }
            #endregion
        }
        #endregion

    }
}