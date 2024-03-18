using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.IdentityContextUtilities;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.SubscriptionTrials;
using Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant.CreateTenantCreationRequest;
using Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant.Models;
using Roaa.Rosas.Application.Services.Management.Tenants.Service;
using Roaa.Rosas.Application.SystemMessages;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;
using Roaa.Rosas.Domain.Models;

namespace Roaa.Rosas.Application.Services.Management.TenantCreationRequests
{
    public class TenantCreationRequestService : ITenantCreationRequestService
    {
        #region Props 
        private readonly ILogger<TenantCreationRequestService> _logger;
        private readonly IIdentityContextService _identityContextService;
        private readonly IRosasDbContext _dbContext;
        private readonly ITenantWorkflow _workflow;
        private readonly ITrialProcessingService _trialProcessingService;
        #endregion


        #region Corts
        public TenantCreationRequestService(ILogger<TenantCreationRequestService> logger,
                                   IIdentityContextService identityContextService,
                                   ITrialProcessingService trialProcessingService,
                                   IRosasDbContext dbContext,
                                   ITenantWorkflow workflow)
        {
            _logger = logger;
            _identityContextService = identityContextService;
            _trialProcessingService = trialProcessingService;
            _dbContext = dbContext;
            _workflow = workflow;
        }

        #endregion


        #region Services  




        public async Task<Result<List<SubscriptionPreparationModel>>> PrepareTenantCreationAsync(TenantCreationRequestModel request, Guid? tenantCreationRequestId, CancellationToken cancellationToken = default)
        {
            #region Validation  

            if (request.Subscriptions.Any(x => x.PlanId is null) || request.Subscriptions.Any(x => x.PlanPriceId is null))
            {
                // لا يمكن ان يكون معرف الخطة او معرف سعر الخطة نل في حال عدم وجود تريال للمنتج
                return Result<List<SubscriptionPreparationModel>>.Fail(
                            CommonErrorKeys.ParameterIsRequired,
                            _identityContextService.Locale,
                             request.Subscriptions.Any(x => x.PlanId is null) ? "PlanId" : "PlanPriceId");
            }

            var tenantCreationPreparationModels = await _dbContext.PlanPrices
                                .AsNoTracking()
                                .Where(x => request.Subscriptions.Select(x => x.PlanPriceId).ToList().Contains(x.Id))
                                .Select(x => new SubscriptionPreparationModel
                                {
                                    Plan = new()
                                    {
                                        Id = x.PlanId,
                                        DisplayName = x.Plan.DisplayName,
                                        SystemName = x.Plan.SystemName,
                                        TenancyType = x.Plan.TenancyType,
                                        IsPublished = x.Plan.IsPublished,
                                        TrialPeriodInDays = x.Plan.TrialPeriodInDays,
                                    },
                                    PlanPrice = new()
                                    {
                                        Id = x.Id,
                                        Price = x.Price,
                                        PlanCycle = x.PlanCycle,
                                    },
                                    Product = new ProductDataModel
                                    {
                                        Id = x.Plan.ProductId,
                                        ClientId = x.Plan.Product.ClientId,
                                        SystemName = x.Plan.Product.SystemName,
                                        DisplayName = x.Plan.Product.DisplayName,
                                        Url = x.Plan.Product.DefaultHealthCheckUrl,
                                        TrialType = x.Plan.Product.TrialType,
                                        TrialPlanId = x.Plan.Product.TrialPlanId,
                                        TrialPlanPriceId = x.Plan.Product.TrialPlanPriceId,
                                        TrialPeriodInDays = x.Plan.Product.TrialPeriodInDays,
                                    },
                                })
                                .ToListAsync(cancellationToken);

            var features = await _dbContext.PlanFeatures
                                             .AsNoTracking()
                                             .Where(x => request.Subscriptions.Select(x => x.PlanId).Contains(x.PlanId))
                                             .Select(x => new PlanFeatureInfoModel
                                             {
                                                 PlanFeatureId = x.Id,
                                                 FeatureId = x.FeatureId,
                                                 FeatureUnit = x.FeatureUnit,
                                                 PlanId = x.PlanId,
                                                 Limit = x.Limit,
                                                 FeatureDisplayName = x.Feature.DisplayName,
                                                 FeatureName = x.Feature.SystemName,
                                                 FeatureType = x.Feature.Type,
                                                 FeatureReset = x.FeatureReset,
                                             })
                                             .ToListAsync(cancellationToken);
            var specifications = await _dbContext.Specifications
                                             .Where(x => request.Subscriptions.Select(x => x.ProductId).Contains(x.ProductId) &&
                                                         x.IsPublished)
                                             .Select(x => new SpecificationInfoModel
                                             {
                                                 ProductId = x.ProductId,
                                                 SpecificationId = x.Id,
                                             })
                                             .ToListAsync();

            if (tenantCreationPreparationModels is null || !tenantCreationPreparationModels.Any())
            {
                return Result<List<SubscriptionPreparationModel>>.Fail(CommonErrorKeys.ParameterIsRequired, _identityContextService.Locale, nameof(request.Subscriptions));
            }



            int sequenceNum = 1;
            foreach (var creationModel in tenantCreationPreparationModels)
            {
                var subscriptionModel = request.Subscriptions.Where(x => x.ProductId == creationModel.Product.Id).FirstOrDefault();


                if (subscriptionModel is null)
                {
                    return Result<List<SubscriptionPreparationModel>>.Fail(CommonErrorKeys.InvalidParameters, _identityContextService.Locale, nameof(request.Subscriptions));
                }


                if (request.Subscriptions.Where(x => x.ProductId == creationModel.Product.Id).Count() > 1)
                {
                    return Result<List<SubscriptionPreparationModel>>.Fail(CommonErrorKeys.InvalidParameters, _identityContextService.Locale, nameof(request.Subscriptions));
                }


                if (creationModel.Plan.TenancyType == TenancyType.Planed && !creationModel.Plan.IsPublished)
                {
                    return Result<List<SubscriptionPreparationModel>>.Fail(CommonErrorKeys.InvalidParameters, _identityContextService.Locale, "PlanId");
                }


                if (creationModel.PlanPrice.PlanCycle == PlanCycle.Custom && subscriptionModel.CustomPeriodInDays is null && creationModel.Plan.TenancyType != TenancyType.Unlimited)
                {
                    return Result<List<SubscriptionPreparationModel>>.Fail(CommonErrorKeys.ParameterIsRequired, _identityContextService.Locale, nameof(subscriptionModel.CustomPeriodInDays));
                }

                creationModel.SequenceNum = subscriptionModel.GetSequenceNum() ?? sequenceNum++;
                creationModel.PlanPrice.CustomPeriodInDays = subscriptionModel.CustomPeriodInDays;
                creationModel.Features = features.Where(x => x.PlanId == creationModel.Plan.Id).ToList();
                creationModel.HasTrial = subscriptionModel.UserEnabledTheTrial ? _trialProcessingService.HasTrial(creationModel) : false;
                creationModel.Specifications = specifications.Where(x => x.ProductId == creationModel.Product.Id).Select(x => new SpecificationInfoModel
                {
                    ProductId = x.ProductId,
                    SpecificationId = x.SpecificationId,
                    Value = subscriptionModel.Specifications.Where(s => s.SpecificationId == x.SpecificationId).SingleOrDefault()?.Value
                }).ToList();

            }

            var systemNameIsUnique = await EnsureSystemNameIsUniqueAsync(request.Subscriptions.Select(x => x.ProductId).ToList(), request.SystemName, tenantCreationRequestId ?? Guid.NewGuid());

            if (!string.IsNullOrWhiteSpace(request.SystemName) && !systemNameIsUnique)
            {
                return Result<List<SubscriptionPreparationModel>>.Fail(ErrorMessage.NameAlreadyUsed, _identityContextService.Locale, nameof(request.SystemName));
            }
            var initialProcess = await _workflow.GetNextStageAsync(expectedResourceStatus: ExpectedTenantResourceStatus.None,
                                                                        currentStatus: TenantStatus.None,
                                                                        currentStep: TenantStep.None,
                                                                        userType: _identityContextService.GetUserType());
            if (initialProcess is null)
            {
                return Result<List<SubscriptionPreparationModel>>.Fail(CommonErrorKeys.UnAuthorizedAction, _identityContextService.Locale, nameof(request.SystemName));
            }
            #endregion
            return Result<List<SubscriptionPreparationModel>>.Successful(tenantCreationPreparationModels);
        }

        public async Task EnableAutoRenewalAsync(Guid orderId, bool autoRenewalIsEnabled, CancellationToken cancellationToken = default)
        {
            var tenantCreationRequest = await _dbContext.TenantCreationRequests
                                                      .Where(x => x.OrderId == orderId)
                                                      .SingleOrDefaultAsync(cancellationToken);
            if (tenantCreationRequest != null)
            {
                tenantCreationRequest.AutoRenewalIsEnabled = autoRenewalIsEnabled;
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

        }
        public TenantCreationRequest BuildTenantCreationRequestEntity(Guid orderId, List<Guid> productIds, string systemName, string displayName, List<TenantCreationRequestSpecification> specifications)
        {
            return new TenantCreationRequest
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                ProductIds = productIds,
                NormalizedSystemName = systemName.ToUpper(),
                DisplayName = displayName,
                CreatedByUserId = _identityContextService.GetActorId(),
                ModifiedByUserId = _identityContextService.GetActorId(),
                CreatedByUserType = _identityContextService.GetUserType(),
                CreationDate = DateTime.UtcNow,
                ModificationDate = DateTime.UtcNow,
                Specifications = specifications.Select(spec =>
                                    new TenantCreationRequestSpecification()
                                    {
                                        Id = Guid.NewGuid(),
                                        ProductId = spec.ProductId,
                                        SpecificationId = spec.SpecificationId,
                                        Value = spec.Value

                                    }).ToList(),
            };
        }

        #endregion

        #region Utilities     

        private async Task<bool> EnsureSystemNameIsUniqueAsync(List<Guid> productsIds, string systemName, Guid tenantRequestId = new Guid(), CancellationToken cancellationToken = default)
        {
            var ids = (await _dbContext.TenantCreationRequests
                                      .Where(x => x.Id != tenantRequestId &&
                                                  systemName.ToUpper().Equals(x.NormalizedSystemName))
                                      .Select(x => x.ProductIds).ToListAsync(cancellationToken)).SelectMany(x => x).ToList();

            return !productsIds.Any(pId => ids.Contains(pId));
        }



        #endregion


    }
}
