using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Domain.Entities;

namespace Roaa.Rosas.Infrastructure.Persistence.Interceptors
{
    public class AuditableEntitySaveChangesInterceptor : SaveChangesInterceptor
    {

        private readonly IIdentityContextService _identityContextService;

        #region Corts
        public AuditableEntitySaveChangesInterceptor(IIdentityContextService identityContextService)
        {
            _identityContextService = identityContextService;
        }
        #endregion

        #region Services 
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            UpdateEntities(eventData.Context);

            return base.SavingChanges(eventData, result);
        }



        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            UpdateEntities(eventData.Context);

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }



        public void UpdateEntities(DbContext? context)
        {
            if (context == null) return;

            foreach (var entry in context.ChangeTracker.Entries<BaseAuditableEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    if (entry.Entity.CreatedByUserId == Guid.Empty)
                    {
                        entry.Entity.CreatedByUserId = _identityContextService.UserId;
                        entry.Entity.EditedByUserId = _identityContextService.UserId;
                    }
                    entry.Entity.Created = DateTime.UtcNow;
                    entry.Entity.Edited = entry.Entity.Created;
                }
                if (entry.State == EntityState.Modified)
                {
                    if (entry.Entity.EditedByUserId == Guid.Empty)
                    {
                        entry.Entity.EditedByUserId = _identityContextService.UserId;
                    }
                    entry.Entity.Edited = DateTime.UtcNow;
                }
            }
        }

        #endregion
    }
}
