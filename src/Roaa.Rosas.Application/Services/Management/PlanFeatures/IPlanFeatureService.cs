using Roaa.Rosas.Application.Services.Management.PlanFeatures.Models;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.PlanFeatures
{
    public interface IPlanFeatureService
    {
        Task<Result<List<PlanFeatureListItemDto>>> GetPlanFeaturesListByProductNameAsync(string productName, CancellationToken cancellationToken = default);

        Task<Result<List<PlanFeatureListItemDto>>> GetPlanFeaturesListByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);

        Task<Result<CreatedResult<Guid>>> CreatePlanFeatureAsync(CreatePlanFeatureModel model, Guid productId, CancellationToken cancellationToken = default);

        Task<Result> UpdatePlanFeatureAsync(Guid planFeatureId, UpdatePlanFeatureModel model, Guid productId, CancellationToken cancellationToken = default);

        Task<Result> DeletePlanFeatureAsync(Guid planFeatureId, Guid productId, CancellationToken cancellationToken = default);
    }
}
