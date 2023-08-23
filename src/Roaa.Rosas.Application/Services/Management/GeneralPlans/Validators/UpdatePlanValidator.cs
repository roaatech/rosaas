using FluentValidation;
using Roaa.Rosas.Application.Services.Management.GeneralPlans.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;

namespace Roaa.Rosas.Application.Services.Management.GeneralPlans.Validators
{
    public class UpdatePlanValidator : AbstractValidator<UpdatePlanModel>
    {
        public UpdatePlanValidator(IIdentityContextService identityContextService)
        {
            RuleFor(x => x.Name).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);
        }
    }
}
