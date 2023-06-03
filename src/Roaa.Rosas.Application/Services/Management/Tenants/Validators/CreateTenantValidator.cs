using FluentValidation;
using Roaa.Rosas.Application.Services.Management.Tenants.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Validators
{
    public class CreateTenantValidator : AbstractValidator<CreateTenantModel>
    {
        public CreateTenantValidator(IIdentityContextService identityContextService)
        {
            // RuleFor(x => x.ProductId).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

            RuleFor(x => x.UniqueName).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);
        }
    }
}
