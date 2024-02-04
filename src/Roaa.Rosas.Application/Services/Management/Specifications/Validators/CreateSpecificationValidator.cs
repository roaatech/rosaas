using FluentValidation;
using Roaa.Rosas.Application.Services.Management.Specifications.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;

namespace Roaa.Rosas.Application.Services.Management.Specifications.Validators
{
    public class CreateSpecificationValidator : AbstractValidator<CreateSpecificationModel>
    {
        public CreateSpecificationValidator(IIdentityContextService identityContextService)
        {
            RuleFor(x => x.SystemName).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

            RuleFor(x => x.DisplayName).Must(localizedName => ValidationHelper.ValidateLocalizedName(localizedName)).WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

            RuleFor(x => x.InputType).IsInEnum().WithError(CommonErrorKeys.InvalidParameters, identityContextService.Locale);

            RuleFor(x => x.DataType).IsInEnum().WithError(CommonErrorKeys.InvalidParameters, identityContextService.Locale);

        }
    }
}

