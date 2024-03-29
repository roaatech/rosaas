﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Domain.Entities.Identity;
using Roaa.Rosas.Infrastructure.Common;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Identity
{
    public class IdentityRoleConfiguration : IEntityTypeConfiguration<Role>
    {
        #region Configure 
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTableName("IdentityRoles");
        }
        #endregion
    }
}
