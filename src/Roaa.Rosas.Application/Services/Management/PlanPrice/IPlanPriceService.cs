﻿using Roaa.Rosas.Application.Services.Management.PlanPrices.Models;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.PlanPrices
{
    public interface IPlanPriceService
    {
        Task<Result<List<PlanPriceListItemDto>>> GetPlanPricesListByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);

        Task<Result<CreatedResult<Guid>>> CreatePlanPriceAsync(CreatePlanPriceModel model, Guid productId, CancellationToken cancellationToken = default);

        Task<Result> UpdatePlanPriceAsync(Guid planFeatureId, UpdatePlanPriceModel model, Guid productId, CancellationToken cancellationToken = default);

        Task<Result> DeletePlanPriceAsync(Guid planFeatureId, Guid productId, CancellationToken cancellationToken = default);
    }
}
