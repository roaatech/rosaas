using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Common.Utilities;
using Roaa.Rosas.Domain.Entities;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Application.Services.Management.GenericAttributes
{
    public partial class GenericAttributeService : IGenericAttributeService
    {
        #region Props 
        private readonly ILogger<GenericAttributeService> _logger;
        private readonly IRosasDbContext _dbContext;
        #endregion


        #region Corts
        public GenericAttributeService(
            ILogger<GenericAttributeService> logger,
            IRosasDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        #endregion


        #region Services   

        public async Task DeleteAttributeAsync(Guid attributeId, CancellationToken cancellationToken = default)
        {
            var gAttribute = await _dbContext.GenericAttributes
                                            .Where(x => x.Id == attributeId)
                                            .SingleOrDefaultAsync(cancellationToken);

            await DeleteAttributeAsync(gAttribute);
        }


        public async Task DeleteAttributeAsync(GenericAttribute attribute, CancellationToken cancellationToken = default)
        {
            if (attribute is null)
                return;

            _dbContext.GenericAttributes.Remove(attribute);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAttributeAsync(IBaseEntity entity, string key, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var keyGroup = entity.GetType().Name;
            key = key.ToUpper();
            var prop = await _dbContext.GenericAttributes
                                       .Where(x => x.EntityId == entity.Id &&
                                       x.KeyGroup.Equals(keyGroup) &&
                                       EF.Functions.Like(x.Key, key))
                                       .FirstOrDefaultAsync(cancellationToken);

            if (prop == null || string.IsNullOrEmpty(prop.Value))
                return;

            await DeleteAttributeAsync(prop, cancellationToken);
        }

        public virtual async Task DeleteAttributesAsync(List<Guid> attributesIds, CancellationToken cancellationToken = default)
        {

            var gAttributes = await _dbContext.GenericAttributes
                                            .Where(x => attributesIds.Contains(x.Id))
                                            .ToListAsync(cancellationToken);

            await DeleteAttributesAsync(gAttributes);
        }

        public virtual async Task DeleteAttributesAsync(List<GenericAttribute> attributes, CancellationToken cancellationToken = default)
        {
            if (attributes is null || !attributes.Any())
                return;

            _dbContext.GenericAttributes.RemoveRange(attributes);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task InsertAttributeAsync(GenericAttribute attribute, CancellationToken cancellationToken = default)
        {
            if (attribute == null)
                throw new ArgumentNullException(nameof(attribute));

            attribute.Id = Guid.NewGuid();
            attribute.CreationDate = DateTime.UtcNow;
            attribute.ModificationDate = DateTime.UtcNow;

            _dbContext.GenericAttributes.Add(attribute);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task UpdateAttributeAsync(GenericAttribute attribute, CancellationToken cancellationToken = default)
        {
            if (attribute == null)
                throw new ArgumentNullException(nameof(attribute));

            attribute.ModificationDate = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(cancellationToken);
        }


        public async Task<List<GenericAttribute>> GetAttributesForEntityAsync(Guid entityId, string keyGroup, CancellationToken cancellationToken = default)
        {
            var gAttributes = await _dbContext.GenericAttributes
                                          .Where(x => x.EntityId == entityId &&
                                                      x.KeyGroup.Equals(keyGroup))
                                          .ToListAsync(cancellationToken);


            return gAttributes;
        }


        public async Task SaveAttributeAsync<TPropType>(IBaseEntity entity, string key, TPropType value, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var keyGroup = entity.GetType().Name;

            var props = (await GetAttributesForEntityAsync(entity.Id, keyGroup)).ToList();

            var prop = props.FirstOrDefault(ga =>
                ga.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase)); //should be culture invariant

            var valueStr = string.Empty;

            if (typeof(TPropType).IsClass)
            {
                valueStr = JsonConvert.SerializeObject(value);
            }
            else
            {
                valueStr = Helpers.To<string>(value);
            }


            if (prop != null)
            {
                if (string.IsNullOrWhiteSpace(valueStr))
                    //delete
                    await DeleteAttributeAsync(prop, cancellationToken);
                else
                {
                    //update
                    prop.Value = valueStr;
                    await UpdateAttributeAsync(prop, cancellationToken);
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(valueStr))
                    return;

                //insert
                prop = new GenericAttribute
                {
                    EntityId = entity.Id,
                    Key = key,
                    KeyGroup = keyGroup,
                    Value = valueStr,
                };

                await InsertAttributeAsync(prop, cancellationToken);
            }
        }

        public async Task SaveAttributeAsync<TEntity, TPropType>(Guid entityId, string key, TPropType value, CancellationToken cancellationToken = default) where TEntity : IBaseEntity
        {
            var entity = (TEntity)Activator.CreateInstance(typeof(TEntity));

            entity.Id = entityId;

            await SaveAttributeAsync(entity, key, value, cancellationToken);
        }

        public virtual async Task<TPropType> GetAttributeAsync<TPropType>(IBaseEntity entity, string key, TPropType defaultValue = default, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var keyGroup = entity.GetType().Name;
            key = key.ToUpper();
            var prop = await _dbContext.GenericAttributes
                                       .Where(x => x.EntityId == entity.Id &&
                                       x.KeyGroup.Equals(keyGroup) &&
                                       EF.Functions.Like(x.Key, key))
                                       .FirstOrDefaultAsync(cancellationToken);

            if (prop == null || string.IsNullOrEmpty(prop.Value))
                return defaultValue;

            var dd = typeof(TenantStep?);
            if (typeof(TPropType).IsClass)
            {
                return JsonConvert.DeserializeObject<TPropType>(prop.Value);
            }

            return Helpers.To<TPropType>(prop.Value);
        }


        public virtual async Task<TPropType> GetAttributeAsync<TEntity, TPropType>(Guid entityId, string key, TPropType defaultValue = default, CancellationToken cancellationToken = default)
            where TEntity : IBaseEntity
        {
            var entity = (TEntity)Activator.CreateInstance(typeof(TEntity));

            entity.Id = entityId;

            return await GetAttributeAsync(entity, key, defaultValue, cancellationToken);
        }

        #endregion
    }
}
