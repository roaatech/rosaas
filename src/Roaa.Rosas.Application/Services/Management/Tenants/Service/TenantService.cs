using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Extensions;
using Roaa.Rosas.Application.IdentityContextUtilities;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Tenants.Commands.ChangeTenantStatus;
using Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant.CreateTenantCreationRequest;
using Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant.Models;
using Roaa.Rosas.Application.Services.Management.Tenants.HealthCheckStatus;
using Roaa.Rosas.Application.Services.Management.Tenants.Service.Models;
using Roaa.Rosas.Application.SystemMessages;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;
using Roaa.Rosas.Domain.Events.Management;
using Roaa.Rosas.Domain.Models;
using System.Linq.Expressions;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Service
{
    public partial class TenantService : ITenantService
    {
        #region Props 
        private readonly ILogger<TenantService> _logger;
        private readonly IRosasDbContext _dbContext;
        private readonly IWebHostEnvironment _environment;
        private readonly IIdentityContextService _identityContextService;
        private readonly ITenantWorkflow _workflow;
        private readonly IPublisher _publisher;
        private readonly BackgroundServicesStore _backgroundServicesStore;
        #endregion


        #region Corts
        public TenantService(
            ILogger<TenantService> logger,
            IRosasDbContext dbContext,
            IWebHostEnvironment environment,
            ITenantWorkflow workflow,
            IPublisher publisher,
            IIdentityContextService identityContextService,
            BackgroundServicesStore backgroundServicesStore)
        {
            _logger = logger;
            _dbContext = dbContext;
            _environment = environment;
            _workflow = workflow;
            _publisher = publisher;
            _identityContextService = identityContextService;
            _backgroundServicesStore = backgroundServicesStore;
        }

        #endregion


        #region Services   
        public async Task<Result<T>> GetByIdAsync<T>(Guid tenantId, Expression<Func<Tenant, T>> selector, CancellationToken cancellationToken = default)
        {
            var result = await _dbContext.Tenants
                                         .Where(x => x.Id == tenantId)
                                         .Select(selector)
                                         .SingleOrDefaultAsync(cancellationToken);

            return Result<T>.Successful(result);
        }



        public async Task<Result<List<TenantStatusChangedResultDto>>> SetTenantNextStatusAsync(Guid tenantId,
                                                                                               TenantStatus status,
                                                                                               Guid? productId,
                                                                                               WorkflowAction action,
                                                                                               ExpectedTenantResourceStatus? expectedResourceStatus,
                                                                                               string comment,
                                                                                               dynamic? receivedRequestBody,
                                                                                               CancellationToken cancellationToken = default)
        {
            // var stepStatus = await _workflow.GetStepStatusAsync(status, cancellationToken);


            var result = await SetTenantNextStatusAsync(new SetTenantNextStatusModel
            {
                UserType = _identityContextService.GetUserType(),
                Status = status,
                Step = null,
                Action = action,
                TenantId = tenantId,
                ProductId = productId,
                Comment = comment,
                ExpectedResourceStatus = expectedResourceStatus,
                EditorBy = _identityContextService.GetActorId(),
                ReceivedRequest = receivedRequestBody is null ? null : new ReceivedRequestModel(receivedRequestBody),
            });

            if (!result.Success)
            {
                return Result<List<TenantStatusChangedResultDto>>.Fail(result.Messages);
            }


            var dtos = (await Task.WhenAll(result.Data.Select(async item =>
            {
                var action = (await _workflow.GetNextStagesAsync(item.Subscription.ExpectedResourceStatus, item.Subscription.Status, item.Subscription.Step, _identityContextService.GetUserType())).ToActionsResults();
                return new TenantStatusChangedResultDto(item.Subscription.ProductId, item.Subscription.Status, action);
            })))
            .Where(result => result != null)
            .ToList();


            return Result<List<TenantStatusChangedResultDto>>.Successful(dtos);
        }



        public async Task<Result<List<SetTenantNextStatusResult>>> SetTenantNextStatusAsync(SetTenantNextStatusModel model, CancellationToken cancellationToken = default)
        {
            #region Validation

            // Preparing to Retrieve Subscriptions 
            Expression<Func<Subscription, bool>> predicate = x => x.TenantId == model.TenantId;
            if (model.ProductId is not null)
            {
                predicate = x => x.TenantId == model.TenantId && x.ProductId == model.ProductId;
            }


            // Subscriptions retrieving  
            var subscriptions = await _dbContext.Subscriptions.Where(predicate).ToListAsync(cancellationToken);


            // Subscriptions validating  
            if (subscriptions is null || !subscriptions.Any())
            {
                return Result<List<SetTenantNextStatusResult>>.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
            }


            // Getting the next status of the subscriptions' workflow 

            var workflows = (await Task.WhenAll(subscriptions.Select(async subscription =>
            {
                return await _workflow.GetNextStageAsync(expectedResourceStatus: subscription.ExpectedResourceStatus, currentStatus: subscription.Status,
                                                                 currentStep: subscription.Step,
                                                                 nextStatus: model.Status,
                                                                 userType: model.UserType,
                                                                 action: model.Action);
            })))
            .Where(result => result != null)
            .ToList();

            if (workflows is null || !workflows.Any())
            {
                return Result<List<SetTenantNextStatusResult>>.Fail(ErrorMessage.NotAllowedChangeStatus, _identityContextService.Locale);
            }
            #endregion

            List<SetTenantNextStatusResult> results = new();

            DateTime date = DateTime.UtcNow;

            foreach (var subscription in subscriptions)
            {
                var workflow = workflows.Where(x => x.CurrentStatus == subscription.Status).FirstOrDefault();

                if (workflow is not null)
                {
                    subscription.Status = workflow.NextStatus;
                    subscription.Step = workflow.NextStep;
                    subscription.ModifiedByUserId = model.EditorBy;
                    subscription.ModificationDate = date;
                    subscription.Comment = model.Comment;
                    subscription.ExpectedResourceStatus = model.ExpectedResourceStatus.HasValue ? model.ExpectedResourceStatus.Value : subscription.ExpectedResourceStatus;



                    results.Add(new SetTenantNextStatusResult(subscription, workflow));

                    subscription.AddDomainEvent(new TenantStatusUpdatedEvent(subscription: subscription,
                                                                             workflow: workflow,
                                                                             previousStatus: workflow.CurrentStatus,
                                                                             previousStep: workflow.CurrentStep,
                                                                             comment: model.Comment,
                                                                             systemComment: workflow.Message,
                                                                             dispatchedRequest: model.DispatchedRequest,
                                                                             receivedRequest: model.ReceivedRequest));
                }

            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<List<SetTenantNextStatusResult>>.Successful(results);
        }


        public List<TenantSystemName> BuildTenantSystemNameEntities(string systemName, List<Guid> productIdS, Guid tenantCreationRequestId, Guid? tenantId = null)
        {
            return productIdS.Select(productId =>
                                    new TenantSystemName()
                                    {
                                        Id = Guid.NewGuid(),
                                        ProductId = productId,
                                        TenantId = tenantId,
                                        TenantCreationRequestId = tenantCreationRequestId,
                                        TenantNormalizedSystemName = systemName.ToUpper(),
                                        // DisplayName = displayName,
                                    }).ToList();
        }


        public TenantCreationRequest BuildTenantCreationRequestEntity(Guid orderId, string systemName, string displayName, List<TenantCreationRequestSpecification> specifications)
        {
            return new TenantCreationRequest
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
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


        public async Task<Result<List<TenantCreationPreparationModel>>> PrepareTenantCreationAsync(TenantCreationRequestModel request, Guid? tenantCreationRequestId, CancellationToken cancellationToken = default)
        {
            #region Validation  

            if (request.Subscriptions.Any(x => x.PlanId is null) || request.Subscriptions.Any(x => x.PlanPriceId is null))
            {
                foreach (var sub in request.Subscriptions)
                {
                    var product = await _dbContext.Products
                                                 .AsNoTracking()
                                                 .Where(x => x.Id == sub.ProductId)
                                                 .SingleOrDefaultAsync(cancellationToken);

                    if (product.TrialType != ProductTrialType.ProductHasTrialPlan ||
                        product.TrialPlanId is null ||
                        product.TrialPlanPriceId is null)
                    {
                        return Result<List<TenantCreationPreparationModel>>.Fail(CommonErrorKeys.ParameterIsRequired,
                                                                                 _identityContextService.Locale,
                                                                                 sub.PlanId is null ? nameof(sub.PlanId) : nameof(sub.PlanPriceId));
                    }

                    if (sub.PlanId is not null && sub.PlanId != product.TrialPlanId)
                    {
                        return Result<List<TenantCreationPreparationModel>>.Fail(CommonErrorKeys.ParameterIsRequired,
                                                                                 _identityContextService.Locale,
                                                                                 nameof(sub.PlanPriceId));
                    }
                    sub.PlanId = product.TrialPlanId;
                    sub.PlanPriceId = product.TrialPlanPriceId;
                }

            }


            var productsIds = request.Subscriptions.Select(x => x.ProductId).ToList();
            var products = await _dbContext.Products
                                             .AsNoTracking()
                                             .Where(x => productsIds.Contains(x.Id))
                                             .ToListAsync(cancellationToken);


            var planPriceIds = request.Subscriptions.Select(x => x.PlanPriceId).ToList();

            var planDataList = await _dbContext.PlanPrices
                                            .AsNoTracking()
                                            .Where(x => planPriceIds.Contains(x.Id))
                                            .Select(x => new TenantCreationPreparationModel
                                            {
                                                Plan = new()
                                                {
                                                    Id = x.PlanId,
                                                    DisplayName = x.Plan.DisplayName,
                                                    SystemName = x.Plan.SystemName,
                                                    TenancyType = x.Plan.TenancyType,
                                                    IsPublished = x.Plan.IsPublished,
                                                    TrialPeriodInDays = x.Plan.TrialPeriodInDays,
                                                    AlternativePlanId = x.Plan.AlternativePlanId,
                                                    AlternativePlanPriceId = x.Plan.AlternativePlanPriceId,
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

            if (planDataList is null || !planDataList.Any())
            {
                return Result<List<TenantCreationPreparationModel>>.Fail(CommonErrorKeys.ParameterIsRequired, _identityContextService.Locale, nameof(request.Subscriptions));
            }


            foreach (var item in planDataList)
            {
                var req = request.Subscriptions.Where(x => x.ProductId == item.Product.Id).FirstOrDefault();
                if (req is null)
                {
                    return Result<List<TenantCreationPreparationModel>>.Fail(CommonErrorKeys.InvalidParameters, _identityContextService.Locale, nameof(request.Subscriptions));
                }

                if (request.Subscriptions.Where(x => x.ProductId == item.Product.Id).Count() > 1)
                {
                    return Result<List<TenantCreationPreparationModel>>.Fail(CommonErrorKeys.InvalidParameters, _identityContextService.Locale, nameof(request.Subscriptions));
                }

                if (item.Plan.TenancyType == TenancyType.Planed && !item.Plan.IsPublished)
                {
                    return Result<List<TenantCreationPreparationModel>>.Fail(CommonErrorKeys.InvalidParameters, _identityContextService.Locale, "PlanId");
                }

                if (item.PlanPrice.PlanCycle == PlanCycle.Custom && req.CustomPeriodInDays is null && item.Plan.TenancyType != TenancyType.Unlimited)
                {
                    return Result<List<TenantCreationPreparationModel>>.Fail(CommonErrorKeys.ParameterIsRequired, _identityContextService.Locale, nameof(req.CustomPeriodInDays));
                }
            }


            if (!string.IsNullOrWhiteSpace(request.SystemName) && !await EnsureSystemNameIsUniqueAsync(request.Subscriptions.Select(x => x.ProductId).ToList(), request.SystemName, tenantCreationRequestId ?? Guid.NewGuid()))
            {
                return Result<List<TenantCreationPreparationModel>>.Fail(ErrorMessage.NameAlreadyUsed, _identityContextService.Locale, nameof(request.SystemName));
            }



            var initialProcess = await _workflow.GetNextStageAsync(expectedResourceStatus: ExpectedTenantResourceStatus.None,
                                                                        currentStatus: TenantStatus.None,
                                                                        currentStep: TenantStep.None,
                                                                        userType: _identityContextService.GetUserType());

            if (initialProcess is null)
            {
                return Result<List<TenantCreationPreparationModel>>.Fail(CommonErrorKeys.UnAuthorizedAction, _identityContextService.Locale, nameof(request.SystemName));
            }

            #endregion

            planDataList = await PreparePlanDataListAsync(request, planDataList, cancellationToken);

            return Result<List<TenantCreationPreparationModel>>.Successful(planDataList);
        }


        #endregion


        #region Utilities     
        private bool HasTrial(TenantCreationPreparationModel model)
        {
            return (model.Product.TrialType == ProductTrialType.ProductHasTrialPlan &&
                    model.Product.TrialPeriodInDays > 0 &&
                    model.Product.TrialPlanId is not null &&
                    model.Product.TrialPlanPriceId is not null &&
                    model.Plan.Id == model.Product.TrialPlanId)
                 ||
                   (model.Product.TrialType == ProductTrialType.EachPlanHasOptinalTrialPeriod &&
                    model.Plan.TrialPeriodInDays > 0);
        }

        private async Task<List<TenantCreationPreparationModel>> PreparePlanDataListAsync(TenantCreationRequestModel request, List<TenantCreationPreparationModel> planDataList, CancellationToken cancellationToken = default)
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
                                                    FeatureName = x.Feature.SystemName,
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
                item.PlanPrice.CustomPeriodInDays = req.CustomPeriodInDays;
                item.Features = featuresInfo.Where(x => x.PlanId == item.Plan.Id).ToList();
                item.Specifications = specifications.Where(x => x.ProductId == item.Product.Id).ToList();
                item.HasTrial = HasTrial(item);
            }

            return planDataList;
        }
        private async Task<bool> EnsureSystemNameIsUniqueAsync(List<Guid> productsIds, string systemName, Guid tenantCreationRequestId = new Guid(), CancellationToken cancellationToken = default)
        {
            var any = await _dbContext.TenantSystemNames
                                      .Where(x => x.TenantCreationRequestId != tenantCreationRequestId &&
                                                  productsIds.Contains(x.ProductId) &&
                                                  systemName.ToUpper().Equals(x.TenantNormalizedSystemName))
                                      .AnyAsync(cancellationToken);

            return !any && !await _dbContext.Subscriptions
                                    .Where(x =>
                                                productsIds.Contains(x.ProductId) &&
                                                systemName.ToLower().Equals(x.Tenant.SystemName))
                                    .AnyAsync(cancellationToken);
        }



        #endregion



    }
}
