using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Settings;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Settings;

namespace Roaa.Rosas.Application.Services.Management.Products.Queries.GetProductWarnings
{
    public class GetProductWarningsQueryHandler : IRequestHandler<GetProductWarningsQuery, Result<List<ProductWarningsDto>>>
    {
        #region Props 
        private readonly IRosasDbContext _dbContext;
        private readonly ISettingService _settingService;
        private readonly IIdentityContextService _identityContextService;
        #endregion


        #region Corts
        public GetProductWarningsQueryHandler(IRosasDbContext dbContext,
                                              ISettingService settingService,
                                              IIdentityContextService identityContextService)
        {
            _dbContext = dbContext;
            _settingService = settingService;
            _identityContextService = identityContextService;
        }

        #endregion


        #region Handler   
        public async Task<Result<List<ProductWarningsDto>>> Handle(GetProductWarningsQuery request, CancellationToken cancellationToken)
        {
            var product = await _dbContext.Products.AsNoTracking()
                                                 .Where(x => x.Id == request.ProductId)
                                                  .Select(x => new
                                                  {
                                                      x.ActivationUrl,
                                                      x.CreationUrl,
                                                      x.DeactivationUrl,
                                                      x.DeletionUrl,
                                                      x.DefaultHealthCheckUrl,
                                                      x.HealthStatusInformerUrl,
                                                      x.SubscriptionResetUrl,
                                                      x.SubscriptionDowngradeUrl,
                                                      x.SubscriptionUpgradeUrl,
                                                      x.ApiKey,
                                                  })
                                                  .SingleOrDefaultAsync(cancellationToken);

            if (product is null)
                return Result<List<ProductWarningsDto>>.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);


            var settings = new List<Setting>();

            var settingsResult = await _settingService.GetSettingsListAsync(typeof(ProductWarningsSettings), cancellationToken);

            if (settingsResult.Data is not null || settingsResult.Data.Any())
            {
                settings = settingsResult.Data;
            }
            var type = product.GetType();
            var results = type.GetProperties().Select(prop =>
            new ProductWarningsDto
            {
                Property = prop.Name,
                IsValid = prop.GetValue(product, null) != null,
                Setting = JsonConvert.DeserializeObject<WarningSettingModel>(settings.Where(setting => setting.ToPropertyName()
                                                                                                                    .Equals(prop.Name, StringComparison.OrdinalIgnoreCase))?
                                                                                          .FirstOrDefault()?
                                                                                          .Value ?? string.Empty)
            }).ToList();

            return Result<List<ProductWarningsDto>>.Successful(results);
        }
        #endregion
    }
    public class ProductWarningsDto
    {
        public string Property { get; set; } = string.Empty;
        public bool IsValid { get; set; }
        public WarningSettingModel? Setting { get; set; } = new();
    }
}
