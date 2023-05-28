using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using Roaa.Rosas.Domain.Entities;
using System.Linq.Expressions;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Shared
{
    public abstract class BaseEntityConfiguration<TEntity> where TEntity : BaseEntity
    {
        protected const string? Schema = null;


        #region Configure 
        public void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.HasKey(x => x.Id);
        }
        #endregion


        public Expression<Func<T?, string>> ConvertLocalizedStringToJson<T>() =>
          v => JsonConvert.SerializeObject(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        public Expression<Func<string, T?>> ConvertJsonToLocalizedString<T>() =>
            v => JsonConvert.DeserializeObject<T>(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
    }
}
