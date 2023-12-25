﻿using MediatR;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Payment.Models;
using Roaa.Rosas.Application.Payment.Services;
using Roaa.Rosas.Application.Services.Management.Orders;
using Roaa.Rosas.Application.Services.Management.Products;
using Roaa.Rosas.Application.Services.Management.Tenants.Service;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant.CreateTenantCreationRequest;

public partial class TenantCreationRequestCommandHandler : IRequestHandler<TenantCreationRequestCommand, Result<TenantCreationRequestResultDto>>
{
    #region Props 
    private readonly IPublisher _publisher;
    private readonly ISender _mediator;
    private readonly IRosasDbContext _dbContext;
    private readonly ITenantWorkflow _workflow;
    private readonly IProductService _productService;
    private readonly IOrderService _orderService;
    private readonly ITenantService _tenantService;
    private readonly IPaymentService _paymentService;
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
        IOrderService orderService,
         ITenantService tenantService,
         IPaymentService paymentService,
        IExternalSystemAPI externalSystemAPI,
        IIdentityContextService identityContextService,
        ILogger<TenantCreationRequestCommandHandler> logger)
    {
        _publisher = publisher;
        _mediator = mediator;
        _dbContext = dbContext;
        _workflow = workflow;
        _productService = productService;
        _orderService = orderService;
        _tenantService = tenantService;
        _paymentService = paymentService;
        _externalSystemAPI = externalSystemAPI;
        _identityContextService = identityContextService;
        _logger = logger;
    }

    #endregion


    #region Handler   
    public async Task<Result<TenantCreationRequestResultDto>> Handle(TenantCreationRequestCommand request, CancellationToken cancellationToken)
    {
        var preparationsResult = await _tenantService.PrepareTenantCreationAsync(request, null, cancellationToken);

        if (!preparationsResult.Success)
        {
            return Result<TenantCreationRequestResultDto>.Fail(preparationsResult.Messages);
        }

        var order = _orderService.BuildOrderEntity(request.SystemName, request.DisplayName, preparationsResult.Data);

        var tenantCreationRequestEntity = _tenantService.BuildTenantCreationRequestEntity(
                                                                              order.Id,
                                                                              request.SystemName,
                                                                              request.DisplayName,
                                                                              request.Subscriptions
                                                                             .SelectMany(x => x.Specifications
                                                                                .Select(spec => new TenantCreationRequestSpecification
                                                                                {
                                                                                    ProductId = x.ProductId,
                                                                                    SpecificationId = spec.SpecificationId,
                                                                                    Value = spec.Value
                                                                                }))
                                                                             .ToList());

        var tenantNameEntities = _tenantService.BuildTenantSystemNameEntities(request.SystemName,
                                                                                 request.Subscriptions
                                                                                .Select(x => x.ProductId)
                                                                                .ToList(),
                                                                                 tenantCreationRequestEntity.Id);
        _dbContext.Orders.Add(order);

        _dbContext.TenantSystemNames.AddRange(tenantNameEntities);

        _dbContext.TenantCreationRequests.AddRange(tenantCreationRequestEntity);

        await _dbContext.SaveChangesAsync(cancellationToken);

        string? navigationUrl = null;

        if (request.CreationByOneClick)
        {
            var result = await _paymentService.HandelPaymentProcessAsyncAsync(
                                                      new CheckoutModel
                                                      {
                                                          OrderId = order.Id,
                                                          PaymentMethod = preparationsResult.Data.Any(x => x.PlanPrice.Price > 0) ? PaymentMethodType.Stripe : null,
                                                      },
                                                      cancellationToken);
            if (!result.Success)
            {
                return Result<TenantCreationRequestResultDto>.Fail(result.Messages);
            }

            navigationUrl = result.Data.NavigationUrl;
        }

        return Result<TenantCreationRequestResultDto>.Successful(new TenantCreationRequestResultDto(order.Id, order.OrderTotal > 0, navigationUrl));

    }

    #endregion 
}
