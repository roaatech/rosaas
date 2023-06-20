using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Auditing.Models;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Identity
{
    public class AuditEntityConfiguration : Roaa.Rosas.Auditing.Contexts.AuditEntityConfiguration, IEntityTypeConfiguration<AuditEntity>
    {

    }
}
