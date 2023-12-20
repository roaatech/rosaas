using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.IdentityContextUtilities;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Products;
using Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant.CreateTenantInDB;
using Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant.Models;
using Roaa.Rosas.Application.Services.Management.Tenants.Service;
using Roaa.Rosas.Application.Services.Management.Tenants.Utilities;
using Roaa.Rosas.Application.SystemMessages;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;
using Roaa.Rosas.Domain.Models;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant.CreateTenantCreationRequest;

public partial class TenantCreationRequestCommandHandler : IRequestHandler<TenantCreationRequestCommand, Result<TenantCreatedResultDto>>
{
    #region Props 
    private readonly IPublisher _publisher;
    private readonly ISender _mediator;
    private readonly IRosasDbContext _dbContext;
    private readonly ITenantWorkflow _workflow;
    private readonly IProductService _productService;
    private readonly IExternalSystemAPI _externalSystemAPI;
    private readonly IIdentityContextService _identityContextService;
    private readonly ILogger<TenantCreationRequestCommandHandler> _logger;
    private readonly DateTime _date = DateTime.UtcNow;
    private Guid _orderId;
    #endregion

    #region Corts
    public TenantCreationRequestCommandHandler(
        IPublisher publisher,
        ISender mediator,
        IRosasDbContext dbContext,
        ITenantWorkflow workflow,
        IProductService productService,
        IExternalSystemAPI externalSystemAPI,
        IIdentityContextService identityContextService,
        ILogger<TenantCreationRequestCommandHandler> logger)
    {
        _publisher = publisher;
        _mediator = mediator;
        _dbContext = dbContext;
        _workflow = workflow;
        _productService = productService;
        _externalSystemAPI = externalSystemAPI;
        _identityContextService = identityContextService;
        _logger = logger;
    }

    #endregion


    #region Handler   
    public async Task<Result<TenantCreatedResultDto>> Handle(TenantCreationRequestCommand request, CancellationToken cancellationToken)
    {
        #region Validation  
        var planPriceIds = request.Subscriptions.Select(x => x.PlanPriceId).ToList();

        var planDataList = await _dbContext.PlanPrices
                                        .AsNoTracking()
                                        .Where(x => planPriceIds.Contains(x.Id))
                                        .Select(x => new PlanDataModel
                                        {
                                            PlanDisplayName = x.Plan.DisplayName,
                                            PlanName = x.Plan.Name,
                                            PlanTenancyType = x.Plan.TenancyType,
                                            Price = x.Price,
                                            PlanPriceId = x.Id,
                                            PlanId = x.PlanId,
                                            IsPublished = x.Plan.IsPublished,
                                            PlanCycle = x.PlanCycle,
                                            Product = new ProductInfoModel
                                            {
                                                Id = x.Plan.ProductId,
                                                ClientId = x.Plan.Product.ClientId,
                                                Name = x.Plan.Product.Name,
                                                DisplayName = x.Plan.Product.DisplayName,
                                                Url = x.Plan.Product.DefaultHealthCheckUrl
                                            },

                                        })
                                        .ToListAsync(cancellationToken);

        if (planDataList is null || !planDataList.Any())
        {
            return Result<TenantCreatedResultDto>.Fail(CommonErrorKeys.ParameterIsRequired, _identityContextService.Locale, nameof(request.Subscriptions));
        }


        foreach (var item in planDataList)
        {
            var req = request.Subscriptions.Where(x => x.ProductId == item.Product.Id).FirstOrDefault();
            if (req is null)
            {
                return Result<TenantCreatedResultDto>.Fail(CommonErrorKeys.InvalidParameters, _identityContextService.Locale, nameof(request.Subscriptions));
            }

            if (request.Subscriptions.Where(x => x.ProductId == item.Product.Id).Count() > 1)
            {
                return Result<TenantCreatedResultDto>.Fail(CommonErrorKeys.InvalidParameters, _identityContextService.Locale, nameof(request.Subscriptions));
            }

            if (item.PlanTenancyType == TenancyType.Planed && !item.IsPublished)
            {
                return Result<TenantCreatedResultDto>.Fail(CommonErrorKeys.InvalidParameters, _identityContextService.Locale, "PlanId");
            }

            if (item.PlanCycle == PlanCycle.Custom && req.CustomPeriodInDays is null && item.PlanTenancyType != TenancyType.Unlimited)
            {
                return Result<TenantCreatedResultDto>.Fail(CommonErrorKeys.ParameterIsRequired, _identityContextService.Locale, nameof(req.CustomPeriodInDays));
            }
        }


        if (!await EnsureUniqueNameAsync(request.Subscriptions.Select(x => x.ProductId).ToList(), request.UniqueName))
        {
            return Result<TenantCreatedResultDto>.Fail(ErrorMessage.NameAlreadyUsed, _identityContextService.Locale, nameof(request.UniqueName));
        }



        var initialProcess = await _workflow.GetNextStageAsync(expectedResourceStatus: ExpectedTenantResourceStatus.None,
                                                                    currentStatus: TenantStatus.None,
                                                                    currentStep: TenantStep.None,
                                                                    userType: _identityContextService.GetUserType());

        if (initialProcess is null)
        {
            return Result<TenantCreatedResultDto>.Fail(CommonErrorKeys.UnAuthorizedAction, _identityContextService.Locale, nameof(request.UniqueName));
        }

        #endregion

        planDataList = await PreparePlanDataListAsync(request, planDataList, cancellationToken);

        if (planDataList.Where(x => x.PlanTenancyType == TenancyType.Planed).Any())
        {
            // go to checkout

            var tenantCreationRequest = BuildTenantCreationRequestEntity(request.UniqueName, request.Title, planDataList);
            var tenantNameEntities = BuildTenantNameEntities(request.UniqueName,
                                                             request.Subscriptions
                                                                    .Select(x => x.ProductId)
                                                                    .ToList());

            _dbContext.TenantCreationRequests.Add(tenantCreationRequest);
            _dbContext.TenantNames.AddRange(tenantNameEntities);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<TenantCreatedResultDto>.Successful(new TenantCreatedResultDto(tenantCreationRequest.Id));

        }
        else
        {
            // create tenant and activate subscribtion 

            return await _mediator.Send(
                                        new CreateTenantInDBCommand
                                        {
                                            PlanDataList = planDataList,
                                            Workflow = initialProcess,
                                            Title = request.Title,
                                            UniqueName = request.UniqueName,
                                            Subscriptions = request.Subscriptions,
                                        },
                                         cancellationToken);
        }
    }

    #endregion


    #region Utilities    

    private async Task<List<PlanDataModel>> PreparePlanDataListAsync(TenantCreationRequestCommand request, List<PlanDataModel> planDataList, CancellationToken cancellationToken = default)
    {
        var planIds = request.Subscriptions.Select(x => x.PlanId)
                                           .ToList();
        var featuresInfo = await _dbContext.PlanFeatures
                                            .AsNoTracking()
                                            .Where(x => planIds.Contains(x.PlanId))
                                            .Select(x => new PlanFeatureInfoModel
                                            {
                                                PlanFeatureId = x.Id,
                                                FeatureId = x.FeatureId,
                                                FeatureUnit = x.FeatureUnit,
                                                PlanId = x.PlanId,
                                                Limit = x.Limit,
                                                FeatureDisplayName = x.Feature.DisplayName,
                                                FeatureName = x.Feature.Name,
                                                FeatureType = x.Feature.Type,
                                                FeatureReset = x.FeatureReset,
                                            })
                                            .ToListAsync(cancellationToken);



        var productsIds = request.Subscriptions.Select(x => x.ProductId)
                                               .ToList();
        var specifications = await _dbContext.Specifications
                                         .Where(x => productsIds.Contains(x.ProductId) &&
                                                     x.IsPublished)
                                         .Select(x => new SpecificationInfoModel
                                         {
                                             ProductId = x.ProductId,
                                             SpecificationId = x.Id,
                                         })
                                         .ToListAsync();

        foreach (var item in planDataList)
        {
            var req = request.Subscriptions.Where(x => x.ProductId == item.Product.Id).FirstOrDefault();
            item.CustomPeriodInDays = req.CustomPeriodInDays;
            item.Features = featuresInfo.Where(x => x.PlanId == item.PlanId).ToList();
            item.Specifications = specifications.Where(x => x.ProductId == item.Product.Id).ToList();
        }

        return planDataList;
    }
    private async Task<bool> EnsureUniqueNameAsync(List<Guid> productsIds, string uniqueName, Guid id = new Guid(), CancellationToken cancellationToken = default)
    {
        var any = await _dbContext.TenantNames
                                  .Where(x => productsIds.Contains(x.ProductId) &&
                                              uniqueName.ToLower().Equals(x.Name))
                                  .AnyAsync(cancellationToken);

        return !any && !await _dbContext.Subscriptions
                                .Where(x => x.TenantId != id && x.Tenant != null &&
                                            productsIds.Contains(x.ProductId) &&
                                            uniqueName.ToLower().Equals(x.Tenant.UniqueName))
                                .AnyAsync(cancellationToken);
    }

    private TenantCreationRequest BuildTenantCreationRequestEntity(string tenantName, string tenantDisplayName, List<PlanDataModel> plansInfo)
    {
        var quantity = 1;

        var items = plansInfo.Select(planData => new TenantCreationRequestItem()
        {
            Id = Guid.NewGuid(),
            StartDate = _date,
            EndDate = PlanCycleManager.FromKey(planData.PlanCycle).CalculateExpiryDate(_date, planData.CustomPeriodInDays),
            ClientId = planData.Product.ClientId,
            ProductId = planData.Product.Id,
            PlanId = planData.PlanId,
            PlanPriceId = planData.PlanPriceId,
            CustomPeriodInDays = planData.CustomPeriodInDays,
            PriceExclTax = planData.Price * quantity,
            PriceInclTax = planData.Price * quantity,
            UnitPriceExclTax = planData.Price,
            UnitPriceInclTax = planData.Price,
            Quantity = quantity,
            Name = $"{planData.Product.Name}--{planData.PlanName}--{tenantName}",
            DisplayName = $"[Product: {planData.Product.DisplayName}], [Plan: {planData.PlanDisplayName}], [Tenant: {tenantDisplayName}]",
            Specifications = planData.Features.Select(x => new TenantCreationRequestItemSpecification
            {
                FeatureId = x.FeatureId,
                Name = $"{x.FeatureName}-" +
                                $"{(x.Limit.HasValue ? x.Limit : string.Empty)}-" +
                                $"{(x.FeatureUnit.HasValue ? x.FeatureUnit.ToString() : string.Empty)}-" +
                                $"{(x.FeatureReset != FeatureReset.NonResettable ? x.FeatureReset.ToString() : string.Empty)}"
                                .Replace("---", "-")
                                .Replace("--", "-")
                                .TrimEnd('-'),
            }).ToList()
        }).ToList();


        return new TenantCreationRequest()
        {
            Id = Guid.NewGuid(),
            CreatedByUserId = _identityContextService.GetActorId(),
            ModifiedByUserId = _identityContextService.GetActorId(),
            CreationDate = _date,
            ModificationDate = _date,
            SubtotalExclTax = items.Select(x => x.PriceExclTax).Sum(),
            SubtotalInclTax = items.Select(x => x.PriceInclTax).Sum(),
            Total = items.Select(x => x.PriceInclTax).Sum(),
            Items = items,
            DisplayName = tenantDisplayName,
            Name = tenantName,
        };
    }
    private List<TenantName> BuildTenantNameEntities(string tenantName, List<Guid> productIdS, Guid? tenantId = null)
    {
        return productIdS.Select(productId =>
                                new TenantName()
                                {
                                    Id = Guid.NewGuid(),
                                    ProductId = productId,
                                    TenantId = tenantId,
                                    Name = tenantName,
                                }).ToList();
    }

    #endregion
}
