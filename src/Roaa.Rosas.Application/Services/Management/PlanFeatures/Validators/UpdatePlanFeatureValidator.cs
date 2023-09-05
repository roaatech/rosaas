using FluentValidation;
using Roaa.Rosas.Application.Services.Management.PlanFeatures.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;

namespace Roaa.Rosas.Application.Services.Management.PlanFeatures.Validators
{
    public class UpdatePlanFeatureValidator : AbstractValidator<UpdatePlanFeatureModel>
    {
        public UpdatePlanFeatureValidator(IIdentityContextService identityContextService)
        {
            When(model => model.Limit is not null, () =>
            {
                RuleFor(x => x.Limit).NotEqual(0).WithError(CommonErrorKeys.InvalidParameters, identityContextService.Locale);
            });

            When(model => model.Unit is not null, () =>
            {
                RuleFor(x => x.Unit).IsInEnum().WithError(CommonErrorKeys.InvalidParameters, identityContextService.Locale);
            });
        }
    }
}
