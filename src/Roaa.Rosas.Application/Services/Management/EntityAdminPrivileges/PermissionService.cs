using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Enums;

namespace Roaa.Rosas.Application.Services.Management.EntityAdminPrivileges
{
    public class PermissionService : IPermissionService
    {
        #region Props 
        private readonly ILogger<PermissionService> _logger;
        private readonly IRosasDbContext _dbContext;
        private readonly IIdentityContextService _identityContextService;
        #endregion


        #region Corts
        public PermissionService(
            ILogger<PermissionService> logger,
            IRosasDbContext dbContext,
            IIdentityContextService identityContextService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _identityContextService = identityContextService;
        }

        #endregion


        #region Services  
        public async Task<bool> HasPermissionAsync(Guid userId, UserType userType, Guid entityId, EntityType entityType, CancellationToken cancellationToken = default)
        {
            return userType == UserType.SuperAdmin || await _dbContext.EntityAdminPrivileges
                                                                            .AnyAsync(x =>
                                                                                x.UserId == userId &&
                                                                                x.EntityId == entityId &&
                                                                                x.EntityType == entityType
                                                                                , cancellationToken);
        }
        #endregion

    }
}
