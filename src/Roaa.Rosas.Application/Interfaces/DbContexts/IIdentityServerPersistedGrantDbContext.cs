using IdentityServer4.EntityFramework.Interfaces;
using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Domain.Entities;

namespace Roaa.Rosas.Application.Interfaces.DbContexts
{
    public interface IIdentityServerPersistedGrantDbContext : IPersistedGrantDbContext
    {
        #region DbSets      
        DbSet<PersistedUserGrant> UserPersistedGrants { get; set; }
        #endregion
    }
}
