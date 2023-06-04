using FluentValidation;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Common.SystemMessages;

namespace Roaa.Rosas.Application.Validators
{
    public class DeleteResourcesListValidator<T> : AbstractValidator<DeleteResourcesListModel<T>>
    {
        public DeleteResourcesListValidator(IIdentityContextService identityContextService)
        {
            RuleFor(x => x.Ids).NotEmpty().WithError(CommonErrorKeys.IdIsRequired, identityContextService.Locale);
        }
    }
}
