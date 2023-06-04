using FluentValidation;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Common.SystemMessages;

namespace Roaa.Rosas.Application.Validators
{
    public class DeleteResourceValidator<T> : AbstractValidator<DeleteResourceModel<T>>
    {
        public DeleteResourceValidator(IIdentityContextService identityContextService)
        {
            RuleFor(x => x.Id).NotEmpty().WithError(CommonErrorKeys.IdIsRequired, identityContextService.Locale);
        }
    }
}
