﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Application.Extensions;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Tenants.Service;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Tenants.Queries.GetTenantById
{
    public class GetTenantByIdQueryHandler : IRequestHandler<GetTenantByIdQuery, Result<TenantDto>>
    {
        #region Props 
        private readonly IRosasDbContext _dbContext;
        private readonly IIdentityContextService _identityContextService;
        private readonly ITenantWorkflow _workflow;
        #endregion


        #region Corts
        public GetTenantByIdQueryHandler(
            IRosasDbContext dbContext,
        ITenantWorkflow workflow,
            IIdentityContextService identityContextService)
        {
            _dbContext = dbContext;
            _workflow = workflow;
            _identityContextService = identityContextService;
        }

        #endregion


        #region Handler   
        public async Task<Result<TenantDto>> Handle(GetTenantByIdQuery request, CancellationToken cancellationToken)
        {
            var tenant = await _dbContext.Tenants.AsNoTracking()
                                                 .Include(x => x.Products)
                                                 .ThenInclude(x => x.Product)
                                                  .Where(x => x.Id == request.Id)
                                                  .Select(tenant => new TenantDto
                                                  {
                                                      Id = tenant.Id,
                                                      UniqueName = tenant.UniqueName,
                                                      Title = tenant.Title,
                                                      Products = tenant.Products.Select(x => new LookupItemDto<Guid>(x.ProductId, x.Product.UniqueName)),
                                                      Status = tenant.Status,
                                                      CreatedDate = tenant.Created,
                                                      EditedDate = tenant.Edited,
                                                  })
                                                  .SingleOrDefaultAsync(cancellationToken);

            if (tenant is not null)
            {
                var flows = await _workflow.GetProcessActionsAsync(tenant.Status, _identityContextService.GetUserType());
                tenant.Actions = flows.ToActionsResults();
            }

            return Result<TenantDto>.Successful(tenant);
        }
        #endregion
    }
}
