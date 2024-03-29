﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Application.IdentityContextUtilities;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Queries.GetTenantProcessesByTenantId
{
    public class GetTenantProcessesByTenantIdQueryHandler : IRequestHandler<GetTenantProcessesByTenantIdQuery, PaginatedResult<TenantProcessDto>>
    {
        #region Props 
        private readonly IRosasDbContext _dbContext;
        private readonly IIdentityContextService _identityContextService;
        #endregion


        #region Corts
        public GetTenantProcessesByTenantIdQueryHandler(
            IRosasDbContext dbContext,
            IIdentityContextService identityContextService)
        {
            _dbContext = dbContext;
            _identityContextService = identityContextService;
        }

        #endregion


        #region Handler   
        public async Task<PaginatedResult<TenantProcessDto>> Handle(GetTenantProcessesByTenantIdQuery request, CancellationToken cancellationToken)
        {

            var query = _dbContext.TenantProcessHistory
                                        .AsNoTracking()
                                        .Where(x => _identityContextService.IsSuperAdmin() ||
                                                    _dbContext.EntityAdminPrivileges
                                                                .Any(a =>
                                                                    a.UserId == _identityContextService.UserId &&
                                                                    a.EntityId == x.TenantId &&
                                                                    a.EntityType == EntityType.Tenant
                                                                    )
                                                )
                                        .Where(x => x.TenantId == request.TenantId && x.ProductId == request.ProductId)
                                        .Select(x => new TenantProcessDto
                                        {
                                            TenantId = x.TenantId,
                                            ProductId = x.ProductId,
                                            SubscriptionId = x.SubscriptionId,
                                            Status = x.Status,
                                            Step = x.Step,
                                            OwnerId = x.OwnerId,
                                            OwnerType = x.OwnerType,
                                            Data = x.Data ?? string.Empty,
                                            //Notes = x.Notes.Select(x => new ProcessNoteModel
                                            //{
                                            //    OwnerType = x.OwnerType,
                                            //    Text = x.Text,
                                            //}),
                                            Notes = x.Notes,
                                            ProcessDate = x.TimeStamp,
                                            ProcessType = x.ProcessType,
                                            UpdatesCount = x.UpdatesCount,
                                        })
                                        .OrderByDescending(x => x.ProcessDate);

            var pagedUsers = await query.ToPagedResultAsync(request.PaginationInfo, cancellationToken);

            return pagedUsers;
        }
        #endregion
    }
}
