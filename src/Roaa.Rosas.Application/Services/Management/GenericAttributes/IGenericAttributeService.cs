using Roaa.Rosas.Domain.Entities;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.GenericAttributes
{
    public interface IGenericAttributeService
    {
        Task DeleteAttributeAsync(Guid attributeId, CancellationToken cancellationToken = default);

        Task DeleteAttributesAsync(List<Guid> attributesIds, CancellationToken cancellationToken = default);

        Task SaveAttributeAsync<TPropType>(BaseEntity entity, string key, TPropType value, CancellationToken cancellationToken = default);

        Task<List<GenericAttribute>> GetAttributesForEntityAsync(Guid entityId, string keyGroup, CancellationToken cancellationToken = default);

        Task<TPropType> GetAttributeAsync<TPropType>(BaseEntity entity, string key, TPropType defaultValue = default, CancellationToken cancellationToken = default);

        Task<TPropType> GetAttributeAsync<TEntity, TPropType>(Guid entityId, string key, TPropType defaultValue = default, CancellationToken cancellationToken = default) where TEntity : BaseEntity;
    }
}
