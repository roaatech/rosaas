using FluentValidation;
using Roaa.Rosas.Application.Services.Management.PlanPrices.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;

namespace Roaa.Rosas.Application.Services.Management.PlanPrices.Validators
{
    public class UpdatePlanPriceValidator : AbstractValidator<UpdatePlanPriceModel>
    {
        public UpdatePlanPriceValidator(IIdentityContextService identityContextService)
        {

            RuleFor(x => x.Cycle).IsInEnum().WithError(CommonErrorKeys.InvalidParameters, identityContextService.Locale);
        }
    }
}
