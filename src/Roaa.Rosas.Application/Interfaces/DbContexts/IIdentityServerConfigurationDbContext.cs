using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Interfaces;
using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Domain.Entities;

namespace Roaa.Rosas.Application.Interfaces.DbContexts
{
    public interface IIdentityServerConfigurationDbContext : IConfigurationDbContext
    {
        #region DbSets  
        public DbSet<ApiResourceProperty> ApiResourceProperties { get; set; }

        public DbSet<IdentityResourceProperty> IdentityResourceProperties { get; set; }

        public DbSet<ApiResourceSecret> ApiSecrets { get; set; }

        public DbSet<ApiScopeClaim> ApiScopeClaims { get; set; }

        public DbSet<IdentityResourceClaim> IdentityClaims { get; set; }

        public DbSet<ApiResourceClaim> ApiResourceClaims { get; set; }

        public DbSet<ClientGrantType> ClientGrantTypes { get; set; }

        public DbSet<ClientScope> ClientScopes { get; set; }

        public DbSet<ClientSecret> ClientSecrets { get; set; }

        public DbSet<ClientPostLogoutRedirectUri> ClientPostLogoutRedirectUris { get; set; }

        public DbSet<ClientIdPRestriction> ClientIdPRestrictions { get; set; }

        public DbSet<ClientRedirectUri> ClientRedirectUris { get; set; }

        public DbSet<ClientClaim> ClientClaims { get; set; }

        public DbSet<ClientProperty> ClientProperties { get; set; }

        public DbSet<ApiScopeProperty> ApiScopeProperties { get; set; }

        public DbSet<ApiResourceScope> ApiResourceScopes { get; set; }

        public DbSet<ClientCustomDetail> ClientCustomDetails { get; set; }
        #endregion

        #region Services     
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        #endregion

    }
}
