using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Auditing.Models;

namespace Roaa.Rosas.Auditing.Contexts;
public interface IAuditDbContext
{
    #region DbSets      
    public DbSet<AuditEntity> Audits { get; }
    #endregion




}