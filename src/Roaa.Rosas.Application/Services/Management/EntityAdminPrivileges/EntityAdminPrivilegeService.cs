using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.IdentityContextUtilities;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.EntityAdminPrivileges.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Management;
using System.Linq.Expressions;

namespace Roaa.Rosas.Application.Services.Management.EntityAdminPrivileges
{
    public class EntityAdminPrivilegeService : IEntityAdminPrivilegeService
    {
        #region Props 
        private readonly ILogger<EntityAdminPrivilegeService> _logger;
        private readonly IRosasDbContext _dbContext;
        private readonly IIdentityContextService _identityContextService;
        #endregion


        #region Corts
        public EntityAdminPrivilegeService(
            ILogger<EntityAdminPrivilegeService> logger,
            IRosasDbContext dbContext,
            IIdentityContextService identityContextService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _identityContextService = identityContextService;
        }

        #endregion


        #region Services  

        public async Task<Result> CreateEntityAdminPrivilegeAsync(CreateResourceAdminModel model, CancellationToken cancellationToken = default)
        {
            var date = DateTime.UtcNow;

            var admin = new EntityAdminPrivilege
            {
                Id = Guid.NewGuid(),
                EntityId = model.EntityId,
                EntityType = model.EntityType,
                UserId = model.UserId,
                UserType = model.UserType,
                IsMajor = model.IsMajor,
                CreatedByUserId = _identityContextService.UserId,
                ModifiedByUserId = _identityContextService.UserId,
                CreationDate = date,
                ModificationDate = date,
            };

            _dbContext.EntityAdminPrivileges.Add(admin);

            var res = await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Successful();
        }


        public Expression<Func<TEntity, bool>> GenerateExpressionFilter<TEntity>(EntityType entityType, Func<TEntity, Guid> tenantIdSelector)
        {
            var isSuperAdmin = _identityContextService.IsSuperAdmin();
            var userId = _identityContextService.UserId;

            Expression<Func<TEntity, bool>> predicate = x => isSuperAdmin || _dbContext.EntityAdminPrivileges.Any(a =>
                                                                                            a.UserId == userId &&
                                                                                            a.EntityId == tenantIdSelector(x) &&
                                                                                            a.EntityType == entityType);
            return predicate;
        }

        #endregion
    }
}
