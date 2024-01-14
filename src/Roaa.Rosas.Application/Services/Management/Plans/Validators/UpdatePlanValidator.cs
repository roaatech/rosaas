using FluentValidation;
using Roaa.Rosas.Application.Services.Management.Plans.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;

namespace Roaa.Rosas.Application.Services.Management.Plans.Validators
{
    public class UpdatePlanValidator : AbstractValidator<UpdatePlanModel>
    {
        public UpdatePlanValidator(IIdentityContextService identityContextService)
        {
            RuleFor(x => x.SystemName).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

            RuleFor(x => x.SystemName).Matches(@"^[a-zA-Z0-9?><;,{}[\]\-_]*$").WithError(CommonErrorKeys.InvalidParameters, identityContextService.Locale);

            RuleFor(x => x.DisplayName).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

            When(model => model.AlternativePlanId is not null, () =>
            {
                RuleFor(x => x.AlternativePlanPriceId).NotNull().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);
                RuleFor(x => x.AlternativePlanPriceId).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);
            });
        }
    }
}
