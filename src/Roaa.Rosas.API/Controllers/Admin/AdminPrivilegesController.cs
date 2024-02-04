using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Application.Services.Management.EntityAdminPrivileges;
using Roaa.Rosas.Application.Services.Management.EntityAdminPrivileges.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Framework.Controllers.Common;

namespace Roaa.Rosas.Framework.Controllers.Admin
{

    [Route(PrefixSuperAdminMainApiRoute)]
    public class AdminPrivilegesController : BaseManagementApiController
    {
        #region Props 
        private readonly ILogger<AdminPrivilegesController> _logger;
        private readonly IEntityAdminPrivilegeService _entityAdminPrivilegeService;
        #endregion

        #region Corts
        public AdminPrivilegesController(ILogger<AdminPrivilegesController> logger,
                                                IEntityAdminPrivilegeService entityAdminPrivilegeService)
        {
            _logger = logger;
            _entityAdminPrivilegeService = entityAdminPrivilegeService;
        }
        #endregion


        #region Actions   
        public record CreateAdminPrivilegeByEmailModel
        {
            public string Email { get; set; } = string.Empty;
            public bool IsMajor { get; set; }
        }

        [Authorize(Policy = AuthPolicy.Identity.TeneatAdminUser, AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
        [HttpPost("Tenant/{tenantId}/[controller]")]
        public async Task<IActionResult> CreateTenantAdminPrivilegeByEmailAsync(CreateAdminPrivilegeByEmailModel model, [FromRoute] Guid tenantId, CancellationToken cancellationToken)
        {
            var result = await _entityAdminPrivilegeService.CreateEntityAdminPrivilegeByUserEmailAsync(new CreateEntityAdminPrivilegeByUserEmailModel
            {
                Email = model.Email,
                EntityId = tenantId,
                EntityType = EntityType.Tenant,
                IsMajor = model.IsMajor,
            }, cancellationToken);

            return ItemResult(result);
        }


        [Authorize(Policy = AuthPolicy.Identity.ProductAdminUser, AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
        [HttpPost("Product/{productId}/[controller]")]
        public async Task<IActionResult> CreateProductAdminPrivilegeByEmailAsync(CreateAdminPrivilegeByEmailModel model, [FromRoute] Guid productId, CancellationToken cancellationToken)
        {
            var result = await _entityAdminPrivilegeService.CreateEntityAdminPrivilegeByUserEmailAsync(new CreateEntityAdminPrivilegeByUserEmailModel
            {
                Email = model.Email,
                EntityId = productId,
                EntityType = EntityType.Product,
                IsMajor = model.IsMajor,
            }, cancellationToken);

            return ItemResult(result);
        }


        [Authorize(Policy = AuthPolicy.Management.Tenants, AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
        [HttpPost("Client/{clientId}/[controller]")]
        public async Task<IActionResult> CreateClientAdminPrivilegeByEmailAsync(CreateAdminPrivilegeByEmailModel model, [FromRoute] Guid clientId, CancellationToken cancellationToken)
        {
            var result = await _entityAdminPrivilegeService.CreateEntityAdminPrivilegeByUserEmailAsync(new CreateEntityAdminPrivilegeByUserEmailModel
            {
                Email = model.Email,
                EntityId = clientId,
                EntityType = EntityType.Client,
                IsMajor = model.IsMajor,
            }, cancellationToken);

            return ItemResult(result);
        }


        [Authorize(Policy = AuthPolicy.Management.Tenants, AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
        [HttpGet("Entity/{entityId}/[controller]")]
        public async Task<IActionResult> GetEntityAdminPrivilegesListByEntityIdAsync([FromRoute] Guid entityId, CancellationToken cancellationToken = default)
        {
            return ListResult(await _entityAdminPrivilegeService.GetEntityAdminPrivilegesListByEntityIdAsync(entityId, cancellationToken));
        }


        [Authorize(Policy = AuthPolicy.Management.Tenants, AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
        [HttpDelete("[controller]/{id}")]
        public async Task<IActionResult> DeleteEntityAdminPrivilegeAsync([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _entityAdminPrivilegeService.DeleteEntityAdminPrivilegeAsync(id, cancellationToken));
        }

        #endregion


    }
}
