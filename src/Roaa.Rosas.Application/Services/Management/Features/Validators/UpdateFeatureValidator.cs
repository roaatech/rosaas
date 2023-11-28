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
            RuleFor(x => x.Name).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

            RuleFor(x => x.Name).Matches(@"^[a-zA-Z0-9?><;,{}[\]\-_]*$").WithError(CommonErrorKeys.InvalidParameters, identityContextService.Locale);

            RuleFor(x => x.Title).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

            RuleFor(x => x.Type).IsInEnum().WithError(CommonErrorKeys.InvalidParameters, identityContextService.Locale);


        }
    }
}
