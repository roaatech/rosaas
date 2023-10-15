using MediatR;
using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Tenants.HealthCheckStatus;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Management;

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



        #endregion

        DateTime date = DateTime.UtcNow;



        #region update specifications


        var specificationsIds = await _dbContext.Specifications
                                         .Where(x => x.ProductId == request.ProductId &&
                                                     x.IsPublished)
                                         .Select(x => new SpecificationModel
                                         {
                                             SpecificationId = x.Id,
                                             SpecificationName = x.Name,
                                         })
                                         .ToListAsync();


        var tenantSpecificationsValues = await _dbContext.SpecificationValues
                                                   .Where(x => x.TenantId == request.Id &&
                                                               x.Subscription.ProductId == request.ProductId)
                                                   .ToListAsync();

        foreach (var specification in specificationsIds.Where(x => !tenantSpecificationsValues.Select(x => x.SpecificationId).Contains(x.SpecificationId)).ToList())
        {
            _dbContext.SpecificationValues.Add(new SpecificationValue
            {
                Id = Guid.NewGuid(),
                TenantId = request.Id,
                SpecificationId = specification.SpecificationId,
                Value = request.Specifications.Where(x => x.SpecificationId == specification.SpecificationId)
                                              .SingleOrDefault()?
                                              .Value,
                CreatedByUserId = _identityContextService.UserId,
                ModifiedByUserId = _identityContextService.UserId,
                CreationDate = date,
                ModificationDate = date,
            });
        }

        foreach (var specificationValue in tenantSpecificationsValues.Where(x => request.Specifications.Select(s => s.SpecificationId).Contains(x.SpecificationId)))
        {
            specificationValue.Value = request.Specifications.Where(x => x.SpecificationId == specificationValue.SpecificationId).SingleOrDefault()?.Value;
            specificationValue.ModifiedByUserId = _identityContextService.UserId;
            specificationValue.ModificationDate = date;
        }
        #endregion


        await _dbContext.SaveChangesAsync(cancellationToken);


        return Result.Successful();
    }
    #endregion
}

