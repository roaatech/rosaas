using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Roaa.Rosas.Common.EFCore
{
    public interface IDbContext
    {

        public DatabaseFacade Database { get; }

        public EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;



        #region Services     
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        public Task DispatchDomainEvents();

        #endregion
    }
}
