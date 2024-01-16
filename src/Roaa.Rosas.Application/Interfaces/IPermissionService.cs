using Roaa.Rosas.Common.Enums;

namespace Roaa.Rosas.Application.Interfaces
{
    public interface IPermissionService
    {
        Task<bool> HasPermissionAsync(Guid userId, UserType userType, Guid entityId, EntityType entityType, CancellationToken cancellationToken = default);
    }
}
