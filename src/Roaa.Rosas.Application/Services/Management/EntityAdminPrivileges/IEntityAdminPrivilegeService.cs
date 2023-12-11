using Roaa.Rosas.Application.Services.Management.EntityAdminPrivileges.Models;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Common.Models.Results;
using System.Linq.Expressions;

namespace Roaa.Rosas.Application.Services.Management.EntityAdminPrivileges
{
    public interface IEntityAdminPrivilegeService
    {
        Task<Result> CreateEntityAdminPrivilegeAsync(CreateResourceAdminModel model, CancellationToken cancellationToken = default);

        Expression<Func<TEntity, bool>> GenerateExpressionFilter<TEntity>(EntityType entityType, Func<TEntity, Guid> tenantIdSelector);
    }
}
