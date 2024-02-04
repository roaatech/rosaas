using FluentValidation;
using Roaa.Rosas.Application.Services.Management.PlanPrices.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;

namespace Roaa.Rosas.Application.Services.Management.PlanPrices.Validators
{
    public class CreatePlanPriceValidator : AbstractValidator<CreatePlanPriceModel>
    {
        public CreatePlanPriceValidator(IIdentityContextService identityContextService)
        {
            RuleFor(x => x.SystemName).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

            RuleFor(x => x.SystemName).Matches(@"^[a-zA-Z0-9?><;,{}[\]\-_]*$").WithError(CommonErrorKeys.InvalidParameters, identityContextService.Locale);

            RuleFor(x => x.PlanId).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

            RuleFor(x => x.Cycle).IsInEnum().WithError(CommonErrorKeys.InvalidParameters, identityContextService.Locale);
        }
    }
}
