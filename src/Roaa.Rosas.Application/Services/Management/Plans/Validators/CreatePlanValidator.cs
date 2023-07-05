using FluentValidation;
using Roaa.Rosas.Application.Services.Management.Plans.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;

namespace Roaa.Rosas.Application.Services.Management.Plans.Validators
{
    public class CreatePlanValidator : AbstractValidator<CreatePlanModel>
    {
        public CreatePlanValidator(IIdentityContextService identityContextService)
        {
            RuleFor(x => x.Name).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);
        }
    }
}
