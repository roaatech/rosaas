using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.IdentityContextUtilities;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant;

public partial class CreateTenantByExternalSystemCommandHandler : IRequestHandler<CreateTenantByExternalSystemCommand, Result<TenantCreatedResultDto>>
{
    #region Props 
    private readonly IPublisher _publisher;
    private readonly IRosasDbContext _dbContext;
    private readonly IIdentityContextService _identityContextService;
    private readonly ILogger<CreateTenantByExternalSystemCommandHandler> _logger;
    private readonly DateTime _date = DateTime.UtcNow;
    private readonly ISender _mediator;
    private Guid _orderId;
    #endregion

    #region Corts
    public CreateTenantByExternalSystemCommandHandler(
        IPublisher publisher,
        IRosasDbContext dbContext,
        IIdentityContextService identityContextService,
        ISender mediator,
    ILogger<CreateTenantByExternalSystemCommandHandler> logger)
    {
        _publisher = publisher;
        _dbContext = dbContext;
        _identityContextService = identityContextService;
        _logger = logger;
        _mediator = mediator;
    }

    #endregion

    #region Handler   
    public async Task<Result<TenantCreatedResultDto>> Handle(CreateTenantByExternalSystemCommand request, CancellationToken cancellationToken)
    {
        #region Validation  

        var productId = _identityContextService.GetProductId();

        var planPrice = await _dbContext.PlanPrices
                                         .Where(x => request.PlanPriceName.ToLower().Equals(x.Name))
                                         .Select(x => new
                                         {
                                             PlanPriceId = x.Id,
                                             x.PlanId,
                                             x.PlanCycle,
                                             x.Plan.TenancyType,
                                         })
                                         .SingleOrDefaultAsync(cancellationToken);
        if (planPrice is null)
        {
            return Result<TenantCreatedResultDto>.Fail(CommonErrorKeys.InvalidParameters, _identityContextService.Locale, nameof(request.PlanPriceName));
        }


        if (planPrice.PlanCycle == PlanCycle.Custom && request.CustomPeriodInDays is null && planPrice.TenancyType != TenancyType.Unlimited)
        {
            return Result<TenantCreatedResultDto>.Fail(CommonErrorKeys.ParameterIsRequired, _identityContextService.Locale, nameof(request.CustomPeriodInDays));
        }

        var planId = planPrice.PlanId;

        var planPriceId = planPrice.PlanPriceId;

        List<CreateSpecificationValueModel> specificationsModels = new();
        if (request.Specifications.Any())
        {
            var spesificationsNormalizedNames = request.Specifications.Select(x => x.Name.ToUpper()).ToList();

            var spesifications = await _dbContext.Specifications
                                             .Where(x => x.ProductId == productId &&
                                                         spesificationsNormalizedNames.Contains(x.NormalizedName))
                                             .Select(x => new { x.Id, x.NormalizedName })
                                             .ToListAsync(cancellationToken);

            if (spesifications.Count() != request.Specifications.Count())
            {
                return Result<TenantCreatedResultDto>.Fail(CommonErrorKeys.InvalidParameters, _identityContextService.Locale, nameof(request.PlanPriceName));
            }

            specificationsModels = spesifications.Select(x => new CreateSpecificationValueModel
            {
                SpecificationId = x.Id,
                Value = request.Specifications.Where(rs => rs.Name.ToUpper().Equals(x.NormalizedName)).SingleOrDefault().Value
            }).ToList();
        }

        #endregion

        var model = new CreateTenantCommand
        {
            Title = request.TenantDisplayName,
            UniqueName = request.TenantName,
            Subscriptions = new List<CreateSubscriptionModel>
            {
                new CreateSubscriptionModel
                {
                    ProductId = productId,
                    PlanId = planId,
                    PlanPriceId = planPriceId,
                    Specifications = specificationsModels
                }
            }
        };

        return await _mediator.Send(model, cancellationToken);
    }

    #endregion


}
