using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Auditing.Models;

namespace Roaa.Rosas.Auditing.Contexts;
public class AuditEntityConfiguration : IEntityTypeConfiguration<AuditEntity>
{
    #region Configure 
    public virtual void Configure(EntityTypeBuilder<AuditEntity> builder)
    {
        ConfigureTable(builder);
        ConfigurePrimaryKey(builder);
        ConfigureAction(builder);
        ConfigureMethod(builder);
        ConfigureCreatedDate(builder);
        ConfigureDuration(builder);
        ConfigureTimeStamp(builder);
        ConfigureUserId(builder);
        ConfigureUserType(builder);
        ConfigureJsonData(builder);
    }

    public virtual void ConfigureTable(EntityTypeBuilder<AuditEntity> builder)
    {
        builder.ToTable(AuditConfigurations.Config.TableName);
    }
    public virtual void ConfigurePrimaryKey(EntityTypeBuilder<AuditEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(r => r.Id).HasColumnName(AuditConfigurations.Config.IdColumnName);
    }
    public virtual void ConfigureAction(EntityTypeBuilder<AuditEntity> builder)
    {
        builder.Property(r => r.Action).HasMaxLength(1000).HasColumnName(AuditConfigurations.Config.ActionColumnName);
    }
    public virtual void ConfigureMethod(EntityTypeBuilder<AuditEntity> builder)
    {
        builder.Property(r => r.Method).HasMaxLength(2000).HasColumnName(AuditConfigurations.Config.MethodColumnName);
    }
    public virtual void ConfigureCreatedDate(EntityTypeBuilder<AuditEntity> builder)
    {
        builder.Property(r => r.CreatedDate).HasColumnName(AuditConfigurations.Config.CreatedDateColumnName);
    }
    public virtual void ConfigureDuration(EntityTypeBuilder<AuditEntity> builder)
    {
        builder.Property(r => r.Duration).HasColumnName(AuditConfigurations.Config.DurationColumnName);
    }
    public virtual void ConfigureTimeStamp(EntityTypeBuilder<AuditEntity> builder)
    {
        builder.Property(r => r.TimeStamp).HasColumnName(AuditConfigurations.Config.TimeStampColumnName);
    }
    public virtual void ConfigureUserId(EntityTypeBuilder<AuditEntity> builder)
    {
        builder.Property(r => r.UserId).HasDefaultValue(Guid.Empty).HasColumnName(AuditConfigurations.Config.UserIdColumnName);
    }
    public virtual void ConfigureUserType(EntityTypeBuilder<AuditEntity> builder)
    {
        builder.Property(r => r.UserType).HasDefaultValue(0).HasColumnName(AuditConfigurations.Config.UserTypeColumnName);
    }
    public virtual void ConfigureJsonData(EntityTypeBuilder<AuditEntity> builder)
    {
        builder.Property(r => r.JsonData).HasColumnName(AuditConfigurations.Config.JsonDataColumnName);
    }
    #endregion
}
