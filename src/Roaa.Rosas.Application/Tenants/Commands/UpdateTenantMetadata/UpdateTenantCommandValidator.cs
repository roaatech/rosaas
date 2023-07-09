using FluentValidation;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.SystemMessages;

namespace Roaa.Rosas.Application.Tenants.Commands.UpdateTenantMetadata;

public partial class UpdateTenantMetadataCommandValidator : AbstractValidator<UpdateTenantMetadataCommand>
{
    public UpdateTenantMetadataCommandValidator(IIdentityContextService identityContextService)
    {
        RuleFor(x => x.TenantId).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);
    }
}
