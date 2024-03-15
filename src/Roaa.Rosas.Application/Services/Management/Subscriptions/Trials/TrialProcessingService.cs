using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Subscriptions.Trials;
using Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.Subscriptions
{
    public class TrialProcessingService : ITrialProcessingService
    {
        #region Props 
        private readonly ILogger<TrialProcessingService> _logger;
        private readonly IIdentityContextService _identityContextService;
        private readonly IRosasDbContext _dbContext;
        private DateTime _date;
        #endregion


        #region Corts
        public TrialProcessingService(ILogger<TrialProcessingService> logger,
                                   IIdentityContextService identityContextService,
                                   IRosasDbContext dbContext)
        {
            _logger = logger;
            _identityContextService = identityContextService;
            _dbContext = dbContext;
            _date = DateTime.UtcNow;
        }
        #endregion


        #region   
        public int? FeatchTrialPeriodInDays(SubscriptionPreparationModel model)
        {
            int? trialPeriodInDays = null;

            if (model.HasTrial)
            {
                if (model.Product.TrialType == ProductTrialType.ProductHasTrialPlan)
                {
                    trialPeriodInDays = model.Product.TrialPeriodInDays;
                }
                else
                {
                    trialPeriodInDays = model.Plan.TrialPeriodInDays;
                }
            }
            return trialPeriodInDays;
        }

        public bool HasTrial(SubscriptionPreparationModel model)
        {
            return (model.Product.TrialType == ProductTrialType.ProductHasTrialPlan &&
                    model.Product.TrialPeriodInDays > 0 &&
                    model.Product.TrialPlanId is not null &&
                    model.Product.TrialPlanPriceId is not null)
                 ||
                   (model.Product.TrialType == ProductTrialType.EachPlanHasOptinalTrialPeriod &&
                    model.Plan.TrialPeriodInDays > 0);
        }

        public SubscriptionTrialPeriod? BuildSubscriptionTrialPeriodEntity(SubscriptionPreparationModel model)
        {
            if (model.HasTrial)
            {
                int TrialPeriodInDays;
                Guid TrialPlanId;
                Guid TrialPlanPriceId;

                if (model.Product.TrialType == ProductTrialType.ProductHasTrialPlan)
                {
                    TrialPeriodInDays = model.Product.TrialPeriodInDays;
                    TrialPlanId = model.Product.TrialPlanId.Value;
                    TrialPlanPriceId = model.Product.TrialPlanPriceId.Value;
                }
                else
                {
                    TrialPeriodInDays = model.Plan.TrialPeriodInDays;
                    TrialPlanId = model.Plan.Id;
                    TrialPlanPriceId = model.PlanPrice.Id;
                }


                return new SubscriptionTrialPeriod()
                {
                    Id = Guid.NewGuid(),
                    StartDate = _date,
                    EndDate = _date.AddDays(TrialPeriodInDays),
                    TrialPeriodInDays = TrialPeriodInDays,
                    TrialPlanId = TrialPlanId,
                    TrialPlanPriceId = TrialPlanPriceId,
                    SelectedPlanId = model.Plan.Id,
                    SelectedPlanPriceId = model.PlanPrice.Id,
                };
            }
            return null;
        }

        #endregion
    }
}
