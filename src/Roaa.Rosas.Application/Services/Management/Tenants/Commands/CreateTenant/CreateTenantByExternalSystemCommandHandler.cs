using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.IdentityContextUtilities;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;

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

        var planId = await _dbContext.Plans
                                         .Where(x => productId == x.ProductId &&
                                                        request.PlanName.ToLower().Equals(x.Name))
                                         .Select(x => x.Id)
                                         .SingleOrDefaultAsync(cancellationToken);

        if (planId == default(Guid) || planId == Guid.Empty)
        {
            return Result<TenantCreatedResultDto>.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale, nameof(request.PlanName));
        }

        var planPriceId = await _dbContext.PlanPrices
                                         .Where(x => request.PlanPriceName.ToLower().Equals(x.Name))
                                         .Select(x => x.Id)
                                         .SingleOrDefaultAsync(cancellationToken);

        if (planPriceId == default(Guid) || planPriceId == Guid.Empty)
        {
            return Result<TenantCreatedResultDto>.Fail(CommonErrorKeys.InvalidParameters, _identityContextService.Locale, nameof(request.PlanPriceName));
        }

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
