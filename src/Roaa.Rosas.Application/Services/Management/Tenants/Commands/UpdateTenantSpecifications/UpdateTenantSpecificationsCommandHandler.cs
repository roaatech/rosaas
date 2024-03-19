using MediatR;
using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Application.IdentityContextUtilities;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.TenantHealthChecks;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Events.Management;
using Roaa.Rosas.Domain.Models.TenantProcessHistoryData;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.UpdateTenantSpecifications;

public class UpdateTenantSpecificationsCommandHandler : IRequestHandler<UpdateTenantSpecificationsCommand, Result>
{
    #region Props 
    private readonly IRosasDbContext _dbContext;
    private readonly IIdentityContextService _identityContextService;
    private readonly BackgroundServicesStore _backgroundServicesStore;
    #endregion

    #region Corts
    public UpdateTenantSpecificationsCommandHandler(
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
    public async Task<Result> Handle(UpdateTenantSpecificationsCommand request, CancellationToken cancellationToken)
    {

        #region Validation
        var subscription = await _dbContext.Subscriptions
                                             .Where(x => _identityContextService.IsSuperAdmin() ||
                                                        _dbContext.EntityAdminPrivileges
                                                                .Any(a =>
                                                                    a.UserId == _identityContextService.UserId &&
                                                                    a.EntityId == x.TenantId &&
                                                                    a.EntityType == EntityType.Tenant
                                                                    )
                                                    )
                                              .Where(x => x.ProductId == request.ProductId &&
                                                           x.TenantId == request.TenantId)
                                              .SingleOrDefaultAsync(cancellationToken);

        if (subscription is null)
        {
            return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
        }
        #endregion

        DateTime date = DateTime.UtcNow;



        #region update specifications


        var specificationsIds = await _dbContext.Specifications
                                         .Where(x => x.ProductId == request.ProductId &&
                                                     x.IsPublished)
                                         .Select(x => new SpecificationModel
                                         {
                                             SpecificationId = x.Id,
                                             SpecificationName = x.SystemName,
                                         })
                                         .ToListAsync();


        var tenantSpecificationsValues = await _dbContext.SpecificationValues
                                                   .Where(x => x.TenantId == request.TenantId &&
                                                               x.Subscription.ProductId == request.ProductId)
                                                   .ToListAsync();
        var data = new List<ProcessedTenantSpecificationValueModel>();
        var newSpecificationsValues = new List<SpecificationValue>();

        // set tenant's specifications values to specifications that previously had no value
        foreach (var specification in specificationsIds.Where(x => !tenantSpecificationsValues.Select(x => x.SpecificationId).Contains(x.SpecificationId)).ToList())
        {
            var specificationValue = new SpecificationValue
            {
                Id = Guid.NewGuid(),
                TenantId = request.TenantId,
                SpecificationId = specification.SpecificationId,
                Value = request.Specifications.Where(x => x.SpecificationId == specification.SpecificationId)
                                               .SingleOrDefault()?
                                               .Value,
                CreatedByUserId = _identityContextService.UserId,
                ModifiedByUserId = _identityContextService.UserId,
                CreationDate = date,
                ModificationDate = date,
            };
            newSpecificationsValues.Add(specificationValue);

            data.Add(new ProcessedTenantSpecificationValueModel
            {
                Name = specification.SpecificationName,
                UpdatedValue = specificationValue.Value,
            });
        }


        // update tenant's specifications values.
        foreach (var specificationValue in tenantSpecificationsValues.Where(x => request.Specifications.Select(s => s.SpecificationId).Contains(x.SpecificationId)))
        {
            var updatedValue = request.Specifications.Where(x => x.SpecificationId == specificationValue.SpecificationId).SingleOrDefault()?.Value;
            data.Add(new ProcessedTenantSpecificationValueModel
            {
                Name = specificationsIds.Where(x => x.SpecificationId == specificationValue.SpecificationId).Select(s => s.SpecificationName).SingleOrDefault(),
                UpdatedValue = updatedValue,
                PreviousValue = specificationValue.Value,
            });
            specificationValue.Value = updatedValue;
            specificationValue.ModifiedByUserId = _identityContextService.UserId;
            specificationValue.ModificationDate = date;

        }
        #endregion

        var processingCompletedEvent = new TenantProcessingCompletedEvent(
                                                            processType: TenantProcessType.SpecificationsUpdated,
                                                            enabled: true,
                                                            processedData: new ProcessedDataOfTenantSpecificationsModel(data).Serialize(),
                                                            processId: out _,
                                                            subscriptions: subscription);

        if (newSpecificationsValues.Any())
        {

            _dbContext.SpecificationValues.AddRange(newSpecificationsValues);
            newSpecificationsValues[0].AddDomainEvent(processingCompletedEvent);
        }
        else
        {
            tenantSpecificationsValues[0].AddDomainEvent(processingCompletedEvent);
        }


        await _dbContext.SaveChangesAsync(cancellationToken);


        return Result.Successful();
    }
    #endregion
}
