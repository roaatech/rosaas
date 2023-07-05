using FluentValidation;
using Roaa.Rosas.Application.Services.Management.PlanFeatures.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;

namespace Roaa.Rosas.Application.Services.Management.PlanFeatures.Validators
{
    public class CreatePlanFeatureValidator : AbstractValidator<CreatePlanFeatureModel>
    {
        public CreatePlanFeatureValidator(IIdentityContextService identityContextService)
        {
            RuleFor(x => x.PlanId).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

            RuleFor(x => x.FeatureId).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

            When(model => model.Limit is not null, () =>
            {
                RuleFor(x => x.Limit).NotEqual(0).WithError(CommonErrorKeys.InvalidParameters, identityContextService.Locale);
            });
        }
    }
}
