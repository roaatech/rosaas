using Roaa.Rosas.Application.Services.Management.Specifications.Models;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Specifications
{
    public interface ISpecificationService
    {
        Task<Result<List<SpecificationListItemDto>>> GetSpecificationsListByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);

        Task<Result<CreatedResult<Guid>>> CreateSpecificationAsync(Guid productId, CreateSpecificationModel model, CancellationToken cancellationToken = default);

        Task<Result> UpdateSpecificationAsync(Guid id, Guid productId, UpdateSpecificationModel model, CancellationToken cancellationToken = default);

        Task<Result> DeleteSpecificationAsync(Guid id, Guid productId, CancellationToken cancellationToken = default);

        Task<Result> PublishSpecificationAsync(Guid id, Guid productId, PublishSpecificationModel model, CancellationToken cancellationToken = default);

        Task<Result> SetSpecificationsAsSubscribedAsync(Guid tenantId, CancellationToken cancellationToken = default);

        Task<Result> SetSpecificationsAsSubscribedAsync(List<Guid> ids, CancellationToken cancellationToken = default);



        Task<Result<List<ExternalSystemSpecificationListItemDto>>> GetSpecificationsListOfExternalSystemByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    }
}
