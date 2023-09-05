﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Application.Extensions;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Tenants.HealthCheckStatus;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;
using Roaa.Rosas.Domain.Entities.Management;
using static Roaa.Rosas.Domain.Entities.Management.TenantProcessData;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.UpdateTenant;

public class UpdateTenantCommandHandler : IRequestHandler<UpdateTenantCommand, Result>
{
    #region Props 
    private readonly IRosasDbContext _dbContext;
    private readonly IIdentityContextService _identityContextService;
    private readonly BackgroundServicesStore _backgroundServicesStore;
    #endregion

    #region Corts
    public UpdateTenantCommandHandler(
        IRosasDbContext dbContext,
        IIdentityContextService identityContextService,
        BackgroundServicesStore backgroundServicesStore)
    {
        _dbContext = dbContext;
        _identityContextService = identityContextService;
        _backgroundServicesStore = backgroundServicesStore;
    }

    #endregion


    #region Handler   
    public async Task<Result> Handle(UpdateTenantCommand request, CancellationToken cancellationToken)
    {

        #region Validation

        var tenant = await _dbContext.Tenants.Where(x => x.Id == request.Id).SingleOrDefaultAsync();
        if (tenant is null)
        {
            return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
        }

        //var productsIds = await _dbContext.ProductTenants.Where(x => x.TenantId == request.Id).Select(x => x.ProductId).ToListAsync();
        //if (!await EnsureUniqueNameAsync(productsIds, request.UniqueName, request.Id))
        //{
        //    return Result.Fail(ErrorMessage.NameAlreadyUsed, _identityContextService.Locale, nameof(request.UniqueName));
        //}
        #endregion
        Tenant tenantBeforeUpdate = tenant.DeepCopy();

        DateTime date = DateTime.UtcNow;

        //  tenant.UniqueName = request.UniqueName.ToLower();
        var processData = new TenantDataUpdatedProcessData
        {
            OldData = new TenantInfoProcessData { Title = tenant.Title },
            UpdatedData = new TenantInfoProcessData { Title = request.Title },
        };
        tenant.Title = request.Title;
        tenant.ModifiedByUserId = _identityContextService.UserId;
        tenant.ModificationDate = date;

        ////update products
        //var tenantProducts = await _dbContext.ProductTenants.Where(x => x.TenantId == x.TenantId).ToListAsync();
        //if (tenantProducts != null && tenantProducts.Any())
        //{
        //    _dbContext.ProductTenants.RemoveRange(tenantProducts);
        //}

        //_dbContext.ProductTenants.AddRange(model.ProductsIds.Select(x => new ProductTenant
        //{
        //    ProductId = x,
        //    TenantId = x.TenantId,
        //}));

        tenant.AddDomainEvent(new TenantUpdatedEvent(tenantBeforeUpdate, tenant));

        var subscriptions = await _dbContext.Subscriptions.Where(x => x.TenantId == tenant.Id).ToListAsync();

        foreach (var subscription in subscriptions)
        {
            var processHistory = new TenantProcessHistory
            {
                Id = Guid.NewGuid(),
                TenantId = subscription.TenantId,
                ProductId = subscription.ProductId,
                SubscriptionId = subscription.Id,
                Status = subscription.Status,
                OwnerId = _identityContextService.GetActorId(),
                OwnerType = _identityContextService.GetUserType(),
                ProcessDate = date,
                TimeStamp = date,
                ProcessType = TenantProcessType.DataUpdated,
                Enabled = true,
                Data = System.Text.Json.JsonSerializer.Serialize(processData),
            };
            _dbContext.TenantProcessHistory.Add(processHistory);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        foreach (var tProduct in subscriptions)
        {
            _backgroundServicesStore.RemoveTenantProcess(tProduct.TenantId, tProduct.ProductId);
        }

        return Result.Successful();
    }
    private async Task<bool> EnsureUniqueNameAsync(List<Guid> productsIds, string uniqueName, Guid id = new Guid(), CancellationToken cancellationToken = default)
    {
        return !await _dbContext.Subscriptions
                                .Where(x => x.TenantId != id && x.Tenant != null &&
                                            productsIds.Contains(x.ProductId) &&
                                            uniqueName.ToLower().Equals(x.Tenant.UniqueName))
                                .AnyAsync(cancellationToken);
    }
    #endregion
}

