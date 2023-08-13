using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Extensions;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Products;
using Roaa.Rosas.Application.Services.Management.Products.Models;
using Roaa.Rosas.Application.Services.Management.Tenants.Service;
using Roaa.Rosas.Application.SystemMessages;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;
using Roaa.Rosas.Domain.Models.ExternalSystems;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant;

public partial class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, Result<TenantCreatedResultDto>>
{
    #region Props 
    private readonly IPublisher _publisher;
    private readonly IRosasDbContext _dbContext;
    private readonly ITenantWorkflow _workflow;
    private readonly IProductService _productService;
    private readonly IExternalSystemAPI _externalSystemAPI;
    private readonly IIdentityContextService _identityContextService;
    private readonly ILogger<CreateTenantCommandHandler> _logger;
    private readonly DateTime _date = DateTime.UtcNow;
    #endregion

    #region Corts
    public CreateTenantCommandHandler(
        IPublisher publisher,
        IRosasDbContext dbContext,
        ITenantWorkflow workflow,
        IProductService productService,
        IExternalSystemAPI externalSystemAPI,
        IIdentityContextService identityContextService,
        ILogger<CreateTenantCommandHandler> logger)
    {
        _publisher = publisher;
        _dbContext = dbContext;
        _workflow = workflow;
        _productService = productService;
        _externalSystemAPI = externalSystemAPI;
        _identityContextService = identityContextService;
        _logger = logger;
    }

    #endregion


    #region Handler   
    public async Task<Result<TenantCreatedResultDto>> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
    {
        #region Validation 
        if (!await EnsureUniqueNameAsync(request.ProductsIds, request.UniqueName))
        {
            return Result<TenantCreatedResultDto>.Fail(ErrorMessage.NameAlreadyUsed, _identityContextService.Locale, nameof(request.UniqueName));
        }
        #endregion

        // first status
        var tenant = await CreateTenantInDBAsync(request, cancellationToken);


        var updatedStatuses = await _dbContext.ProductTenants
                                         .Where(x => x.TenantId == tenant.Id)
                                         .Select(x => new { x.Status, x.ProductId })
                                         .ToListAsync(cancellationToken);

        List<ProductTenantCreatedResultDto> productTenantCreatedResults = new();
        foreach (var item in updatedStatuses)
        {
            productTenantCreatedResults.Add(new ProductTenantCreatedResultDto(
            item.ProductId,
            item.Status,
            (await _workflow.GetProcessActionsAsync(item.Status, _identityContextService.GetUserType()))
                            .ToActionsResults()));
        }



        return Result<TenantCreatedResultDto>.Successful(new TenantCreatedResultDto(tenant.Id, productTenantCreatedResults));
    }

    #endregion


    #region Utilities   
    private async Task<Result<ExternalSystemResultModel<dynamic>>> CallExternalSystemToCreateTenantResourecesAsync(Tenant tenant, ProductUrlListItem item, CancellationToken cancellationToken)
    {
        return await _externalSystemAPI.CreateTenantAsync(new ExternalSystemRequestModel<CreateTenantModel>
        {
            BaseUrl = item.Url,
            TenantId = tenant.Id,
            Data = new()
            {
                TenantName = tenant.UniqueName,
            }
        }, cancellationToken);
    }
    private async Task<Tenant> CreateTenantInDBAsync(CreateTenantCommand request, CancellationToken cancellationToken = default)
    {
        var initialProcess = await _workflow.GetNextProcessActionAsync(TenantStatus.None, _identityContextService.GetUserType());

        var defaultHealthCheckUrlOfProducts = await _dbContext.Products
                                                            .Where(x => request.ProductsIds.Contains(x.Id))
                                                            .Select(x => new ProductUrlListItem
                                                            {
                                                                Id = x.Id,
                                                                Url = x.DefaultHealthCheckUrl
                                                            })
                                                            .ToListAsync(cancellationToken);

        var tenant = BuildTenantEntity(request, defaultHealthCheckUrlOfProducts, initialProcess);

        var processes = BuildTenantProcessEntities(tenant.Id, request.ProductsIds, initialProcess);

        var healthStatuses = BuildProductTenantHealthStatusEntities(tenant.Products);

        tenant.AddDomainEvent(new TenantCreatedInStoreEvent(tenant, tenant.Products.First().Status));

        _dbContext.Tenants.Add(tenant);

        _dbContext.TenantProcesses.AddRange(processes);

        _dbContext.ProductTenantHealthStatuses.AddRange(healthStatuses);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return tenant;
    }
    private Tenant BuildTenantEntity(CreateTenantCommand model, List<ProductUrlListItem> defaultHealthCheckUrlOfProducts, Process initialProcess)
    {


        var id = Guid.NewGuid();

        return new Tenant
        {
            Id = id,
            UniqueName = model.UniqueName.ToLower(),
            Title = model.Title,
            CreatedByUserId = _identityContextService.GetActorId(),
            EditedByUserId = _identityContextService.GetActorId(),
            Created = _date,
            Edited = _date,
            Products = defaultHealthCheckUrlOfProducts.Select(productUrl => new ProductTenant
            {
                Id = Guid.NewGuid(),
                TenantId = id,
                ProductId = productUrl.Id,
                Status = initialProcess.NextStatus,
                EditedByUserId = _identityContextService.GetActorId(),
                Edited = _date,
                HealthCheckUrl = productUrl.Url,
                HealthCheckUrlIsOverridden = false,
            }).ToList(),
        };
    }
    private IEnumerable<ProductTenantHealthStatus> BuildProductTenantHealthStatusEntities(ICollection<ProductTenant> Products)
    {
        return Products.Select(item => new ProductTenantHealthStatus
        {
            Id = item.Id,
            TenantId = item.TenantId,
            ProductId = item.ProductId,
            LastCheckDate = item.Edited,
            CheckDate = item.Edited,
            IsHealthy = false,
        });
    }

    private IEnumerable<TenantProcess> BuildTenantProcessEntities(Guid tenantId, List<Guid> ProductsIds, Process initialProcess)
    {


        return ProductsIds.Select(productId => new TenantProcess
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            ProductId = productId,
            Status = initialProcess.NextStatus,
            PreviousStatus = initialProcess.CurrentStatus,
            OwnerId = _identityContextService.GetActorId(),
            OwnerType = _identityContextService.GetUserType(),
            Created = _date,
            Message = initialProcess.Message
        });
    }
    private async Task<bool> EnsureUniqueNameAsync(List<Guid> productsIds, string uniqueName, Guid id = new Guid(), CancellationToken cancellationToken = default)
    {
        return !await _dbContext.ProductTenants
                                .Where(x => x.TenantId != id && x.Tenant != null &&
                                            productsIds.Contains(x.ProductId) &&
                                            uniqueName.ToLower().Equals(x.Tenant.UniqueName))
                                .AnyAsync(cancellationToken);
    }


    #endregion
}
