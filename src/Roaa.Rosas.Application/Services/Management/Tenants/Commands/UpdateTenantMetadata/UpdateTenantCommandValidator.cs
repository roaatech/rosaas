using FluentValidation;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.SystemMessages;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.UpdateTenantMetadata;

public partial class UpdateTenantMetadataCommandValidator : AbstractValidator<UpdateTenantMetadataCommand>
{
    public UpdateTenantMetadataCommandValidator(IIdentityContextService identityContextService)
    {
        RuleFor(x => x.TenantName).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);
    }
}
