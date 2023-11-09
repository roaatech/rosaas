using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Products;
using Roaa.Rosas.Application.Services.Management.Tenants.Service;
using Roaa.Rosas.Application.SystemMessages;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Events.Management;
using Roaa.Rosas.Domain.Models;
using Roaa.Rosas.Domain.Models.ExternalSystems;
using Roaa.Rosas.Domain.Models.TenantProcessHistoryData;
using System.Linq.Expressions;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.ResetSubscription;

public class ResetSubscriptionCommandHandler : IRequestHandler<ResetSubscriptionCommand, Result>
{
    #region Props 
    private readonly ILogger<ResetSubscriptionCommandHandler> _logger;
    private readonly IIdentityContextService _identityContextService;
    private readonly IExternalSystemAPI _externalSystemAPI;
    private readonly IProductService _productService;
    private readonly ITenantService _tenantService;
    private readonly IRosasDbContext _dbContext;
    private readonly IPublisher _publisher;
    #endregion



    #region Corts
    public ResetSubscriptionCommandHandler(IIdentityContextService identityContextService,
                                            IExternalSystemAPI externalSystemAPI,
                                            IProductService productService,
                                            ITenantService tenantService,
                                            IRosasDbContext dbContext,
                                            IPublisher publisher,
                                            ILogger<ResetSubscriptionCommandHandler> logger)
    {
        _identityContextService = identityContextService;
        _externalSystemAPI = externalSystemAPI;
        _productService = productService;
        _tenantService = tenantService;
        _dbContext = dbContext;
        _publisher = publisher;
        _logger = logger;
    }
    #endregion


    #region Handler   
    public async Task<Result> Handle(ResetSubscriptionCommand command, CancellationToken cancellationToken)
    {

        var subscription = await _dbContext.Subscriptions
                                               .Where(x => x.ProductId == command.ProductId &&
                                                           x.TenantId == command.TenantId)
                                               .SingleOrDefaultAsync();

        if (subscription is null)
        {
            return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
        }

        // External System's url preparation
        Expression<Func<Product, ProductApiModel>> selector = x => new ProductApiModel(x.ApiKey, x.SubscriptionResetUrl);

        var urlItemResult = await _productService.GetProductEndpointByIdAsync(command.ProductId, selector, cancellationToken);

        if (!urlItemResult.Success || (urlItemResult.Success && string.IsNullOrWhiteSpace(urlItemResult.Data.Url)))
        {
            return Result.Fail(ErrorMessage.RestUrlNotExist, _identityContextService.Locale);
        }


        // Unique Name tenant retrieving  
        Expression<Func<Tenant, string>> tenantSelector = x => x.UniqueName;

        var tenantResult = await _tenantService.GetByIdAsync(command.TenantId, tenantSelector, cancellationToken);




        // External System calling to reset the tenant resorces 
        var callingResult = await _externalSystemAPI.ResetTenantAsync(
            new ExternalSystemRequestModel<ResetTenantModel>
            {
                BaseUrl = urlItemResult.Data.Url,
                ApiKey = urlItemResult.Data.ApiKey,
                TenantId = command.TenantId,
                Data = new()
                {
                    TenantName = tenantResult.Data,
                }
            },
            cancellationToken);





        var processedData = new ProcessedDataOfResetTenantModel(
                                        new DispatchedRequestModel(
                                            callingResult.Data.DurationInMillisecond,
                                            callingResult.Data.Url,
                                            callingResult.Data.SerializedResponseContent)
                                                               ).Serialize();
        var tenantProcessingCompletedEvent = new TenantProcessingCompletedEvent(
                                                processType: callingResult.Success ? TenantProcessType.SubscriptionReset : TenantProcessType.SubscriptionResetFailure,
                                                enabled: true,
                                                processedData: processedData,
                                                comment: command.Comment,
                                                systemComment: string.Empty,
                                                processId: out _,
                                                subscription);
        if (callingResult.Success)
        {
            var date = DateTime.UtcNow;

            subscription.LastResetDate = date;

            subscription.AddDomainEvent(tenantProcessingCompletedEvent);

            await _dbContext.SaveChangesAsync();

            return Result.Successful();
        }
        else
        {
            await _publisher.Publish(tenantProcessingCompletedEvent);

            return Result.Fail(CommonErrorKeys.OperationFaild, _identityContextService.Locale);
        }




    }
    #endregion
}

