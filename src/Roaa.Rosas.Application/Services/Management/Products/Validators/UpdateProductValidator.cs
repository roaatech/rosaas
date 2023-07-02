using FluentValidation;
using Roaa.Rosas.Application.Services.Management.Products.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;

namespace Roaa.Rosas.Application.Services.Management.Products.Validators
{
    public class UpdateProductValidator : AbstractValidator<UpdateProductModel>
    {
        public UpdateProductValidator(IIdentityContextService identityContextService)
        {

            RuleFor(x => x.Name).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

            RuleFor(x => x.Url).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

            RuleFor(x => x.Url).Must(url => ValidateUri(url)).WithError(CommonErrorKeys.InvalidParameters, identityContextService.Locale);
        }
        public bool ValidateUri(string uri)
        {
            if (string.IsNullOrEmpty(uri))
            {
                return false;
            }
            return Uri.TryCreate(uri, UriKind.Absolute, out _);
        }
    }
}
