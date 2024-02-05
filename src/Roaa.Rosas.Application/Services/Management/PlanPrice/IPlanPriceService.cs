using Roaa.Rosas.Application.Services.Management.PlanPrices.Models;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.PlanPrices
{
    public interface IPlanPriceService
    {
        Task<Result<List<PlanPriceListItemDto>>> GetPlanPricesListByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);

        Task<Result<List<PlanPricePublishedListItemDto>>> GetPublishedPlanPricesListByProductNameAsync(string productName, CancellationToken cancellationToken = default);

        Task<Result<PlanPricePublishedDto>> GetPublishedPlanPriceByPlanPriceNameAsync(string productName, string planPriceName, CancellationToken cancellationToken = default);

        Task<Result<CreatedResult<Guid>>> CreatePlanPriceAsync(CreatePlanPriceModel model, Guid productId, CancellationToken cancellationToken = default);

        Task<Result> UpdatePlanPriceAsync(Guid planPriceId, UpdatePlanPriceModel model, Guid productId, CancellationToken cancellationToken = default);

        Task<Result> PublishPlanPriceAsync(Guid planPriceId, PublishPlanPriceModel model, Guid productId, CancellationToken cancellationToken = default);

        Task<Result> DeletePlanPriceAsync(Guid planPriceId, Guid productId, CancellationToken cancellationToken = default);
    }
}
