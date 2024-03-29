﻿using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Domain.Entities;
using Roaa.Rosas.Infrastructure.Common;

public class IdentityServerConfigurationDbContext : ConfigurationDbContext<IdentityServerConfigurationDbContext>, IIdentityServerConfigurationDbContext
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

    #region Ctors    
    public IdentityServerConfigurationDbContext(DbContextOptions<IdentityServerConfigurationDbContext> options, ConfigurationStoreOptions storeOptions)
       : base(options, storeOptions)
    {

    }
    #endregion


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ClientCustomDetail>().ToTable($"{Consts.IdS4gPrefix}ClientCustomDetails".ToTableNamingStrategy());
        builder.Entity<ClientCustomDetail>().HasKey(x => x.ClientId);
        builder.Entity<ClientCustomDetail>().Property(r => r.ProductId).IsRequired(true);
        builder.Entity<ClientCustomDetail>().Property(r => r.ProductOwnerClientId).IsRequired(true);
    }
}