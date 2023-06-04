using FluentValidation;
using Roaa.Rosas.Application.Services.Management.Tenants.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Validators
{
    public partial class CreateTenantValidator
    {
        public class UpdateTenantStatusValidator : AbstractValidator<UpdateTenantStatusModel>
        {
            public UpdateTenantStatusValidator(IIdentityContextService identityContextService)
            {

                RuleFor(x => x.Id).NotEmpty().WithError(CommonErrorKeys.IdIsRequired, identityContextService.Locale);
            }
        }
    }
}
