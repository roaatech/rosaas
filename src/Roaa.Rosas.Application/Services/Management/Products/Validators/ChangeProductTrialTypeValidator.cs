using FluentValidation;
using Roaa.Rosas.Application.Services.Management.Products.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.Products.Validators
{
    public class ChangeProductTrialTypeValidator : AbstractValidator<ChangeProductTrialTypeModel>
    {
        public ChangeProductTrialTypeValidator(IIdentityContextService identityContextService)
        {
            RuleFor(x => x.TrialType).IsInEnum().WithError(CommonErrorKeys.InvalidParameters, identityContextService.Locale);

            When(model => model.TrialType == ProductTrialType.ProductHasTrialPlan, () =>
            {
                RuleFor(x => x.TrialPeriodInDays).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);
                RuleFor(x => x.TrialPlanId).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);
            });

        }
    }
}
