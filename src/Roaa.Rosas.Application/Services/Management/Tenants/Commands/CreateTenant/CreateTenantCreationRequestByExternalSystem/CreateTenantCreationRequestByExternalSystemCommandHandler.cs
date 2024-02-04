using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.IdentityContextUtilities;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant.CreateTenantCreationRequest;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant.CreateTenantCreationRequestByExternalSystem;

public partial class CreateTenantCreationRequestByExternalSystemCommandHandler : IRequestHandler<CreateTenantCreationRequestByExternalSystemCommand, Result<TenantCreationRequestByExternalSystemResultDto>>
{
    #region Props 
    private readonly IPublisher _publisher;
    private readonly IRosasDbContext _dbContext;
    private readonly IIdentityContextService _identityContextService;
    private readonly ILogger<CreateTenantCreationRequestByExternalSystemCommandHandler> _logger;
    private readonly DateTime _date = DateTime.UtcNow;
    private readonly ISender _mediator;
    private Guid _orderId;
    #endregion

    #region Corts
    public CreateTenantCreationRequestByExternalSystemCommandHandler(
        IPublisher publisher,
        IRosasDbContext dbContext,
        IIdentityContextService identityContextService,
        ISender mediator,
    ILogger<CreateTenantCreationRequestByExternalSystemCommandHandler> logger)
    {
        _publisher = publisher;
        _dbContext = dbContext;
        _identityContextService = identityContextService;
        _logger = logger;
        _mediator = mediator;
    }

    #endregion

    #region Handler   
    public async Task<Result<TenantCreationRequestByExternalSystemResultDto>> Handle(CreateTenantCreationRequestByExternalSystemCommand request, CancellationToken cancellationToken)
    {
        #region Validation  

        var productId = _identityContextService.GetProductId();

        var planPrice = await _dbContext.PlanPrices
                                         .Where(x => request.PlanPriceSystemName.ToLower().Equals(x.SystemName))
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
            return Result<TenantCreationRequestByExternalSystemResultDto>.Fail(CommonErrorKeys.InvalidParameters, _identityContextService.Locale, nameof(request.PlanPriceSystemName));
        }


        if (planPrice.PlanCycle == PlanCycle.Custom && request.CustomPeriodInDays is null && planPrice.TenancyType != TenancyType.Unlimited)
        {
            return Result<TenantCreationRequestByExternalSystemResultDto>.Fail(CommonErrorKeys.ParameterIsRequired, _identityContextService.Locale, nameof(request.CustomPeriodInDays));
        }

        var planId = planPrice.PlanId;

        var planPriceId = planPrice.PlanPriceId;

        List<CreateSpecificationValueModel> specificationsModels = new();

        if (request.Specifications.Any())
        {
            var spesificationsNormalizedNames = request.Specifications.Select(x => x.SystemName.ToUpper()).ToList();

            var spesifications = await _dbContext.Specifications
                                             .Where(x => x.ProductId == productId &&
                                                         spesificationsNormalizedNames.Contains(x.NormalizedSystemName))
                                             .Select(x => new { x.Id, x.NormalizedSystemName })
                                             .ToListAsync(cancellationToken);

            if (spesifications.Count() != request.Specifications.Count())
            {
                return Result<TenantCreationRequestByExternalSystemResultDto>.Fail(CommonErrorKeys.InvalidParameters, _identityContextService.Locale, "Spesifications.SystemName");
            }

            specificationsModels = spesifications.Select(x => new CreateSpecificationValueModel
            {
                SpecificationId = x.Id,
                Value = request.Specifications.Where(rs => rs.SystemName.ToUpper().Equals(x.NormalizedSystemName)).SingleOrDefault().Value
            }).ToList();
        }

        #endregion

        var model = new TenantCreationRequestCommand
        {
            CreationByOneClick = true,
            DisplayName = request.TenantDisplayName,
            SystemName = request.TenantSystemName,
            Subscriptions = new List<CreateSubscriptionModel>
            {
                new CreateSubscriptionModel
                {
                    ProductId = productId,
                    PlanId = planId,
                    PlanPriceId = planPriceId,
                    CustomPeriodInDays = request.CustomPeriodInDays,
                    Specifications = specificationsModels,
                }
            }
        };

        var result = await _mediator.Send(model, cancellationToken);

        if (!result.Success)
        {
            return Result<TenantCreationRequestByExternalSystemResultDto>.Fail(result.Messages);
        }

        return Result<TenantCreationRequestByExternalSystemResultDto>.Successful(new TenantCreationRequestByExternalSystemResultDto(result.Data.NavigationUrl));
    }

    #endregion


}
