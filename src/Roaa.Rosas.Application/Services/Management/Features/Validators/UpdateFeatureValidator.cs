using FluentValidation;
using Roaa.Rosas.Application.Services.Management.Features.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;

namespace Roaa.Rosas.Application.Services.Management.Features.Validators
{
    public class UpdateFeatureValidator : AbstractValidator<UpdateFeatureModel>
    {
        public UpdateFeatureValidator(IIdentityContextService identityContextService)
        {
            RuleFor(x => x.SystemName).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

            RuleFor(x => x.SystemName).Matches(@"^[a-zA-Z0-9?><;,{}[\]\-_]*$").WithError(CommonErrorKeys.InvalidParameters, identityContextService.Locale);

            RuleFor(x => x.DisplayName).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

            RuleFor(x => x.Type).IsInEnum().WithError(CommonErrorKeys.InvalidParameters, identityContextService.Locale);


        }
    }
}
